// test_chan project main.go
package main

import (
	"fmt"
	"net"
	"protocol"
	"storage"
)

func main() {
	var conn net.Conn
	seq, err := storage.AddConnection(conn)
	if err != nil {
		fmt.Println(err)
		return
	}
	fmt.Printf("seq:%d", seq)

	queue, err := storage.GetSendQueue(seq)
	if err != nil {
		fmt.Println(err)
		return
	}

	request := protocol.Protocol{Seq: 100, Command: protocol.SYNCHRONIZE}
	queue <- request

	queue2, err := storage.GetSendQueue(seq)
	if err != nil {
		fmt.Println(err)
		return
	}

	request2 := <-queue2

	fmt.Println(request2)
}
