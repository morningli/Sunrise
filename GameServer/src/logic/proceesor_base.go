// proceesor_base
package logic

import (
	"fmt"
	"protocol"
)

type Processor interface {
	GetCommand() int
	SetRequset(request protocol.Protocol)
	Process()
	GetResponse() protocol.Protocol
	SetContext(context Context)
}

type Context struct {
	ConnectionSeq uint64
}

type ProcessorBase struct {
	_context Context
	_request protocol.Protocol
	_respone protocol.Protocol
}

func (self *ProcessorBase) SetContext(context Context) {
	self._context = context
}

func (self *ProcessorBase) GetResponse() protocol.Protocol {
	return self._respone
}

func (self *ProcessorBase) SetRequset(request protocol.Protocol) {
	self._request = request

}

func (self *ProcessorBase) PackComm() {
	self._respone.Seq = self._request.Seq
	self._respone.Command = self._request.Command
	self._respone.Key = self._request.Key
	self._respone.Time = self._request.Time
	self._respone.Ret = int16(0)
	self._respone.Msg = "succ"
	self._respone.Respone = true
}

func (self *ProcessorBase) PackErr(ret int, msg string) {
	self.PackComm()
	self._respone.Ret = int16(ret)
	self._respone.Msg = msg
}

func (self *ProcessorBase) Pack(data interface{}) {
	self.PackComm()
	if data != nil {
		self._respone.RequestStruct = data
	}
}

var processor_map = map[uint8]Processor{
	protocol.LOGIN:         new(ProcessorLogin),
	protocol.REGISTER:      new(ProcessorRegister),
	protocol.GET_ROOM_LIST: new(ProcessorGetRoomList),
	protocol.ENTER_ROOM:    new(ProcessorEnterRoom),
	protocol.MOVE:          new(ProcessorMove),
	protocol.FIRE:          new(ProcessorFire),
}

func GetProcess(request protocol.Protocol, context Context) protocol.Protocol {
	processor, ok := processor_map[request.Command]
	if !ok {
		fmt.Printf("cmd not exist.cmd:%d\n", request.Command)
		return protocol.Protocol{}
	}
	fmt.Printf("cmd found.cmd:%d\n", request.Command)

	processor.SetContext(context)
	processor.SetRequset(request)
	processor.Process()
	return processor.GetResponse()
}
