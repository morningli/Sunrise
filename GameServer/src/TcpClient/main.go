// TcpClient project main.go
package main

import (
	"fmt"
	"net"
	"time"
)

func main() {
	fmt.Println("Hello World!")
	fmt.Println("begin dial...")
	conn, err := net.DialTimeout("tcp", ":8888", 2*time.Second)
	if err != nil {
		fmt.Println("dial error:", err)
		return
	}
	defer conn.Close()
	fmt.Println("dial ok")

	sms := make([]byte, 128)
	for {
		fmt.Print("请输入要发送的消息:")
		_, err := fmt.Scan(&sms)
		if err != nil {
			fmt.Println("数据输入异常:", err.Error())
		}
		conn.Write([]byte{0x02})
		conn.Write(sms)
		conn.Write([]byte{0x03})

		buf := make([]byte, 1024)
		c, err := conn.Read(buf)
		if err != nil {
			fmt.Println("读取服务器数据异常:", err.Error())
		}
		fmt.Println(c)
		fmt.Println(buf[0:c])
		fmt.Println(string(buf[0:c]))
	}
}
