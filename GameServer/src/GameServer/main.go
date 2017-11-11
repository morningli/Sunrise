// GameServer project main.go
package main

import (
	"action"
	"fmt"
	"net"
)

func main() {
	fmt.Println("Hello World!")
	listen, err := net.Listen("tcp", ":8888")
	if err != nil {
		fmt.Println("listen error: ", err)
		return
	}
	for {
		conn, err := listen.Accept()
		if err != nil {
			fmt.Println("accept error: ", err)
			break
		}
		fmt.Printf("accept.ip:%s\n", conn.RemoteAddr().String())

		// start a new goroutine to handle the new connection
		go action.HandleConn(conn)
	}
}
