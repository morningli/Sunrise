// message project message.go
package message

import (
	"fmt"
	"tool"
)

type Message interface {
	Decode(data []byte) interface{}
	EnCode(data interface{}) ([]byte, error)
	Check(data []byte) int
}

//解析网络包
func CheckDataArride(handler Message) func([]byte) interface{} {

	buff := []byte{}
	hdl := handler

	return func(data []byte) interface{} {

		buff = tool.Merge(buff, data)

		fmt.Println(buff)

		msg_len := hdl.Check(buff)
		if msg_len > 0 {

			fmt.Println("len:", msg_len)

			result := hdl.Decode(buff[:msg_len])
			buff = tool.Remove(buff, msg_len)

			fmt.Println(buff)

			return result
		}
		return nil
	}
}
