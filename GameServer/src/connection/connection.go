// connection project connection.go
package connection

import (
	//"fmt"
	"net"
	"protocol"
)

type ConnData struct {
	Handler    net.Conn
	SendQueue  chan protocol.Protocol
	Shutdown   chan bool
	SendSeq    uint32
	RecieveSeq uint32
}

func (conn *ConnData) SendRequestWithSeq(seq uint32, request protocol.Protocol) {
	go func() {
		//fmt.Println("enter SendRequest")
		request.Seq = seq
		//fmt.Println(request)
		conn.SendQueue <- request
	}()
}

func (conn *ConnData) SendRequest(request protocol.Protocol) {
	seq := conn.GetSendSeq()
	conn.SendRequestWithSeq(seq, request)
}

func (conn *ConnData) SetRecieveSeq(seq uint32) {
	if seq > conn.SendSeq {
		conn.SendSeq = seq
	}
}

func (conn *ConnData) GetSendSeq() uint32 {
	conn.SendSeq++
	return conn.SendSeq
}

func (conn *ConnData) GetConnHandler() net.Conn {
	return conn.Handler
}

func (conn *ConnData) GetSendQueue() chan protocol.Protocol {
	return conn.SendQueue
}

func (conn *ConnData) GetShutdownSignal() chan bool {
	return conn.Shutdown
}

func (conn *ConnData) Close() {
	conn.Handler.Close()
	close(conn.SendQueue)
}

func CreateConnData(conn net.Conn) *ConnData {
	data := new(ConnData)
	data.Handler = conn
	data.SendQueue = make(chan protocol.Protocol, 100)
	return data
}
