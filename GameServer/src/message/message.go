// message project message.go
package message

import (
	"bytes"
	"encoding/json"
	//	"fmt"
	"protocol"
)

func DataDispatch(proto protocol.Protocol) protocol.Protocol {
	switch proto.Command {
	case protocol.HERTBEAT:
		var request protocol.DoHertBeat
		json.Unmarshal([]byte(proto.Request), &request)
		proto.RequestStruct = request
	case protocol.LOGIN:
		var request protocol.DoLoginReq
		json.Unmarshal([]byte(proto.Request), &request)
		proto.RequestStruct = request
	case protocol.MOVE:
		var request protocol.DoMove
		json.Unmarshal([]byte(proto.Request), &request)
		proto.RequestStruct = request
	case protocol.FIRE:
		var request protocol.DoFire
		json.Unmarshal([]byte(proto.Request), &request)
		proto.RequestStruct = request
	case protocol.REGISTER:
		var request protocol.DoRegisterReq
		json.Unmarshal([]byte(proto.Request), &request)
		proto.RequestStruct = request
	case protocol.ENTER_ROOM:
		var request protocol.DoEnterRoomReq
		json.Unmarshal([]byte(proto.Request), &request)
		proto.RequestStruct = request
	}
	return proto
}

func DecodeMessage(payload []byte) interface{} {
	//fmt.Println(payload)
	var request protocol.Protocol
	err := json.Unmarshal(payload, &request)
	if err != nil {
		//fmt.Println("error:", err)
	} else {
		//fmt.Println(request)
		return DataDispatch(request)
	}
	return nil
}

func EncodeMessage(request protocol.Protocol) ([]byte, error) {

	//fmt.Println("enter EncodeMessage")

	data, err := json.Marshal(request.RequestStruct)
	if err != nil {
		return nil, err
	}

	//fmt.Println(request.RequestStruct)
	//fmt.Println(data)

	request.Request = string(data)
	return json.Marshal(request)
}

//添加网络头
func PackNetData(request protocol.Protocol) ([]byte, error) {

	//fmt.Println("enter PackNetData")

	data, err := EncodeMessage(request)
	if err != nil {
		return nil, err
	}

	//fmt.Println(request)
	//fmt.Println(data)

	result := []byte{protocol.BEGIN}
	result = append(result, data...)
	result = append(result, protocol.END)
	return result, nil
}

//解析网络包
func CheckDataArride() func([]byte, int) interface{} {
	buff := new(bytes.Buffer)
	return func(data []byte, len_read int) interface{} {
		buff.Write(data[0:len_read])
		for i := 0; i < len_read; i++ {
			if data[i] == protocol.BEGIN {
				buff.Next(buff.Len() - len_read + i)
			} else {
				if data[i] == protocol.END {
					tcp_package := buff.Next(buff.Len() - len_read + i + 1)
					return DecodeMessage(tcp_package[1 : len(tcp_package)-1])
				}
			}
		}
		return nil
	}
}
