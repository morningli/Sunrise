// test_timer project main.go
package main

import (
	"fmt"
	"time"
)

func main() {

	aChan := make(chan int, 1)
	ticker := time.NewTicker(time.Millisecond * 15)

	go func() {
		for {
			select {
			case <-ticker.C:
				fmt.Printf("ticked at %v\n", time.Now())
			}
		}

	}()
	//阻塞主线程
	<-aChan
}
