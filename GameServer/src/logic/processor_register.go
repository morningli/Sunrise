// proceesor_login
package logic

import (
	"errors"
	"protocol"
	"storage"
)

type ProcessorRegister struct {
	ProcessorBase
}

func (self *ProcessorRegister) GetCommand() int {
	return protocol.REGISTER
}

func (self *ProcessorRegister) Unpack() (protocol.DoRegisterReq, error) {
	data, ok := self._request.RequestStruct.(protocol.DoRegisterReq)
	if !ok {
		return protocol.DoRegisterReq{}, errors.New("请求出错")
	}
	return data, nil
}

func (self *ProcessorRegister) process_logic() error {

	request, err := self.Unpack()
	if err != nil {
		return err
	}

	succ := storage.Set(request.Name, request.Password)
	if !succ {
		return errors.New("注册失败")
	}

	self.Pack(nil)

	return nil
}

func (self *ProcessorRegister) Process() {
	err := self.process_logic()
	if err != nil {
		self.PackErr(-1, err.Error())
	}
}
