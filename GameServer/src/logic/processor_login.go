// proceesor_login
package logic

import (
	"errors"
	"online"
	"protocol"
	"storage"
)

type ProcessorLogin struct {
	ProcessorBase
}

func (self *ProcessorLogin) GetCommand() int {
	return protocol.LOGIN
}

func (self *ProcessorLogin) Unpack() (protocol.DoLoginReq, error) {
	data, ok := self._request.RequestStruct.(protocol.DoLoginReq)
	if !ok {
		return protocol.DoLoginReq{}, errors.New("请求出错")
	}
	return data, nil
}

func (self *ProcessorLogin) process_logic() error {
	request, err := self.Unpack()
	if err != nil {
		return err
	}

	err = storage.CheckAuthInfo(request.Name, request.Password)
	if err != nil {
		return err
	}

	key, err := online.Login(request.Name)
	if err != nil {
		return err
	}

	var respone protocol.DoLoginRsp
	respone.Key = key

	self.Pack(respone)

	return nil
}

func (self *ProcessorLogin) Process() {
	err := self.process_logic()
	if err != nil {
		self.PackErr(-1, err.Error())
	}
}
