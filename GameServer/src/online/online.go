//在线状态 online.go
package online

import (
	"connection"
	"errors"
	"fmt"
	"net"
	"time"
	"tool"
)

//在线用户数据

var login_map = map[string]string{}
var login_map_r = map[string]string{}

var des_key = []byte{'G', 'A', 'M', 'E', 'U', 'S', 'E', 'R'}

func Login(name string) (string, error) {

	Logout(name)

	now := time.Now().Unix()
	for i := int64(0); i < 10; i++ {
		key_raw := fmt.Sprintf("%s_%d", name, now+i)

		key_out, err := tool.DesEncrypt([]byte(key_raw), des_key)
		if err != nil {
			return "", err
		}

		fmt.Println(key_out)
		key_str := tool.Convert(key_out)
		fmt.Println(key_str)

		_, ok := login_map[key_str]
		if !ok {
			login_map[key_str] = name
			login_map_r[name] = key_str
		}
		return key_str, nil
	}

	return "", errors.New("登录失败")
}

func Logout(name string) error {
	value, ok := login_map_r[name]
	if ok {
		value_r, ok_r := login_map[value]
		if ok_r && value_r == name {
			delete(login_map, value)
		}
		delete(login_map_r, name)
	}
	return nil
}

//连接管理

var connections = map[uint64]connection.ConnData{}
var connect_sequece = uint64(0)

func AddConnection(conn net.Conn) (uint64, error) {
	connect_sequece++
	_, ok := connections[connect_sequece]
	if ok {
		return 0, errors.New("无法添加链接")
	}
	data := *connection.CreateConnData(conn)
	connections[connect_sequece] = data
	return connect_sequece, nil
}

func GetConnection(seq uint64, data *connection.ConnData) error {
	conn, ok := connections[seq]
	if !ok {
		return errors.New("连接不存在")
	}
	*data = conn
	return nil
}

func RemoveConnection(seq uint64) {
	conn, ok := connections[seq]
	if !ok {
		fmt.Println("连接不存在")
		return
	}
	conn.Close()
	delete(connections, seq)
}
