// action project action.go
package action

import (
	"connection"
	"fmt"
	"logic"
	"message"
	"net"
	"online"
	"protocol"
)

func HandleSend(conn *connection.ConnData) {
	for {
		select {
		case <-conn.GetShutdownSignal():
			break
		case data := <-conn.GetSendQueue():

			fmt.Println("发送给客户端的数据:", data)

			payload, err := message.GetSimpleMessageInstance().EnCode(data)
			if err != nil {
				fmt.Println(err)
				continue
			}
			//fmt.Printf("payload:%s\n", string(payload[:]))
			conn.GetConnHandler().Write(payload)
		}
	}
}

var reacive_command = map[int]bool{
	protocol.LOGIN:         true,
	protocol.REGISTER:      true,
	protocol.ENTER_ROOM:    true,
	protocol.GET_ROOM_LIST: true}

func HandleConn(conn net.Conn) {

	fmt.Printf("connection.ip:%s\n", conn.RemoteAddr().String())

	//添加进链接管理模块
	seq, err := online.AddConnection(conn)
	if err != nil {
		fmt.Println(err)
		conn.Close()
		return
	}

	//获取链接信息
	var connData connection.ConnData
	err = online.GetConnection(seq, &connData)
	if err != nil {
		fmt.Println(err)
		return
	}

	//释放链接
	defer online.RemoveConnection(seq)
	defer func() { connData.GetShutdownSignal() <- true }()
	go HandleSend(&connData)

	data := make([]byte, 1024)
	check := message.CheckDataArride(message.GetSimpleMessageInstance())
	for {
		i, err := connData.GetConnHandler().Read(data)
		if err != nil {
			fmt.Println("读取客户端数据错误:", err.Error())
			break
		}

		//校验数据是否完整
		request, ok := check(data[:i]).(protocol.Protocol)
		fmt.Println("客户端发来数据:", request)

		if ok {
			fmt.Println(request)

			//更新当前连接序列号
			connData.SetRecieveSeq(request.Seq)

			var context logic.Context
			context.ConnectionSeq = seq
			respone := logic.GetProcess(request, context)

			_, ok := reacive_command[int(request.Command)]
			if ok {
				connData.SendRequestWithSeq(request.Seq, respone)
				fmt.Println("回给客户端数据:", respone)
			}
		}
	}
}
