// connection project connection.go
package connection

import (
	"fmt"
	"io"
	"net"
	"protocol"
)

type ConnData struct {
	handler     net.Conn
	send_queue  chan protocol.Protocol
	is_close    bool
	send_seq    uint32
	recieve_seq uint32
}

func CreateConnData(self net.Conn) *ConnData {
	data := new(ConnData)
	data.handler = self
	data.send_queue = make(chan protocol.Protocol, 100)
	data.is_close = false
	data.recieve_seq = 1
	return data
}

func (self *ConnData) SendRequestWithSeq(seq uint32, request protocol.Protocol) {
	go func() {
		request.Seq = seq
		self.send_queue <- request
	}()
}

func (self *ConnData) Read(b []byte) (int, error) {
	n, err := self.handler.Read(b)
	if err != nil && err != io.EOF {
		self.is_close = true
	}
	return n, err
}

func (self *ConnData) Write(b []byte) (int, error) {

	n, err := self.handler.Write(b)
	if err != nil && err != io.EOF {
		self.is_close = true
	}
	return n, err
}

func (self *ConnData) SendRequest(request protocol.Protocol) {
	seq := self.GetSendSeq()
	self.SendRequestWithSeq(seq, request)
}

func (self *ConnData) SetRecieveSeq(seq uint32) {
	if seq > self.send_seq {
		self.send_seq = seq
	}
}

func (self *ConnData) GetSendSeq() uint32 {
	self.send_seq++
	return self.send_seq
}

func (self *ConnData) GetSendQueue() chan protocol.Protocol {
	return self.send_queue
}

func (self *ConnData) IsClose() bool {
	fmt.Println("is close:", self.is_close)
	return self.is_close
}

func (self *ConnData) Close() {
	self.is_close = true
	self.handler.Close()
	close(self.send_queue)
}
