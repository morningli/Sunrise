// test_sence project main.go
package main

import (
	"fmt"
	"sence"
)

func main() {
	fmt.Println("Hello World!")
	sence.SenceManagerInstance.CreateSence("test")
}
