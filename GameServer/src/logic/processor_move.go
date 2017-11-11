// proceesor_login
package logic

import (
	"errors"
	"fmt"
	"protocol"
	"sence"
)

type ProcessorMove struct {
	ProcessorBase
}

func (self *ProcessorMove) GetCommand() int {
	return protocol.REGISTER
}

func (self *ProcessorMove) Unpack() (protocol.DoMove, error) {
	data, ok := self._request.RequestStruct.(protocol.DoMove)
	if !ok {
		return protocol.DoMove{}, errors.New("请求出错")
	}
	return data, nil
}

func (self *ProcessorMove) process_logic() error {

	fmt.Println("enter process_logic")

	request, err := self.Unpack()
	if err != nil {
		return err
	}

	sence_no, err := sence.GetSenceHandler().GetSence(request.Name)
	if err != nil {
		return err
	}
	sence.GetSenceHandler().AddAction(sence_no, self._request)

	self.Pack(nil)

	return nil
}

func (self *ProcessorMove) Process() {
	err := self.process_logic()
	if err != nil {
		fmt.Println(err)
		self.PackErr(-1, err.Error())
	}
}
