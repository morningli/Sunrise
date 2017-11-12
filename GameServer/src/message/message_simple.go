// message project message.go
package message

import (
	"encoding/json"
	"errors"
	"protocol"
)

type SimpleMessage struct {
}

var g_simple_message_instance Message

func GetSimpleMessageInstance() Message {
	if g_simple_message_instance == nil {
		g_simple_message_instance = new(SimpleMessage)
	}
	return g_simple_message_instance
}

func (self *SimpleMessage) Decode(data []byte) interface{} {
	return self.DecodeMessage(data[1 : len(data)-1])
}

func (self *SimpleMessage) EnCode(data interface{}) ([]byte, error) {
	protocol, ok := data.(protocol.Protocol)
	if !ok {
		return []byte{}, errors.New("协议错误")
	}
	return self.PackNetData(protocol)
}

func (self *SimpleMessage) Check(data []byte) int {
	for k, v := range data {
		if v == protocol.END {
			return k + 1
		}
	}
	return 0
}

func (self *SimpleMessage) DataDispatch(proto protocol.Protocol) protocol.Protocol {
	switch proto.Command {
	case protocol.HERTBEAT:
		var request protocol.DoHertBeat
		json.Unmarshal([]byte(proto.Request), &request)
		proto.RequestStruct = request
	case protocol.LOGIN:
		var request protocol.DoLoginReq
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
	case protocol.MOVE:
		var request protocol.DoMove
		json.Unmarshal([]byte(proto.Request), &request)
		proto.RequestStruct = request
	}
	return proto
}

func (self *SimpleMessage) DecodeMessage(payload []byte) interface{} {
	var request protocol.Protocol
	err := json.Unmarshal(payload, &request)
	if err != nil {
	} else {
		return self.DataDispatch(request)
	}
	return nil
}

func (self *SimpleMessage) EncodeMessage(request protocol.Protocol) ([]byte, error) {

	data, err := json.Marshal(request.RequestStruct)
	if err != nil {
		return nil, err
	}

	request.Request = string(data)
	return json.Marshal(request)
}

//添加网络头
func (self *SimpleMessage) PackNetData(request protocol.Protocol) ([]byte, error) {

	data, err := self.EncodeMessage(request)
	if err != nil {
		return nil, err
	}

	result := []byte{protocol.BEGIN}
	result = append(result, data...)
	result = append(result, protocol.END)
	return result, nil
}
