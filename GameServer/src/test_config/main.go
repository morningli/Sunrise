// test_config project main.go
package main

import (
	"fmt"
	"storage"
)

func main() {
	fmt.Println("Hello World!")
	fmt.Printf("config:%s\n", storage.GetConfig("test", "test"))
}
