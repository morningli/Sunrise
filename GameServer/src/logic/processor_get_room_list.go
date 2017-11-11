// proceesor_login
package logic

import (
	"protocol"
	"storage"
)

type ProcessorGetRoomList struct {
	ProcessorBase
}

func (self *ProcessorGetRoomList) GetCommand() int {
	return protocol.GET_ROOM_LIST
}

func (self *ProcessorGetRoomList) process_logic() error {

	roomlist, err := storage.GetRoomList()
	if err != nil {
		return err
	}

	var response protocol.GetRoomListRsp
	response.RoomList = roomlist

	self.Pack(response)

	return nil
}

func (self *ProcessorGetRoomList) Process() {
	err := self.process_logic()
	if err != nil {
		self.PackErr(-1, err.Error())
	}
}
