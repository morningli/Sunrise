// proceesor_login
package logic

import (
	"errors"
	"fmt"
	"protocol"
	"sence"
)

type ProcessorFire struct {
	ProcessorBase
}

func (self *ProcessorFire) GetCommand() int {
	return protocol.REGISTER
}

func (self *ProcessorFire) Unpack() (protocol.DoFire, error) {
	data, ok := self._request.RequestStruct.(protocol.DoFire)
	if !ok {
		return protocol.DoFire{}, errors.New("请求出错")
	}
	return data, nil
}

func (self *ProcessorFire) process_logic() error {

	request, err := self.Unpack()
	if err != nil {
		return err
	}

	sence_no, err := sence.GetSenceHandler().GetSence(request.Name)
	if err != nil {
		fmt.Println(err)
		return err
	}
	sence.GetSenceHandler().AddAction(sence_no, self._request)

	self.Pack(nil)

	return nil
}

func (self *ProcessorFire) Process() {
	err := self.process_logic()
	if err != nil {
		self.PackErr(-1, err.Error())
	}
}
