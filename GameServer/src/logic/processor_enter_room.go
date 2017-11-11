// proceesor_login
package logic

import (
	"errors"
	"fmt"
	"protocol"
	"sence"
)

type ProcessorEnterRoom struct {
	ProcessorBase
}

func (self *ProcessorEnterRoom) GetCommand() int {
	return protocol.ENTER_ROOM
}

func (self *ProcessorEnterRoom) Unpack() (protocol.DoEnterRoomReq, error) {

	fmt.Println("enter Unpack")
	fmt.Println(self._request)

	data, ok := self._request.RequestStruct.(protocol.DoEnterRoomReq)
	if !ok {
		return protocol.DoEnterRoomReq{}, errors.New("请求出错")
	}
	return data, nil
}

func (self *ProcessorEnterRoom) process_logic() error {

	request, err := self.Unpack()
	if err != nil {
		return err
	}

	sence_id, err := sence.GetSenceHandler().FindSence(request.Name)
	if err != nil {
		return err
	}

	err = sence.GetSenceHandler().JoinSence(request.Name, sence_id, self._context.ConnectionSeq)
	if err != nil {
		return err
	}

	self.Pack(nil)

	return nil
}

func (self *ProcessorEnterRoom) Process() {
	err := self.process_logic()
	if err != nil {
		self.PackErr(-1, err.Error())
	}
}
