// test_map project main.go
package main

import (
	"fmt"
)

type TestSub struct {
	data map[string]string
}

func (test *TestSub) Remove(key string) {
	delete(test.data, key)
}

func (test *TestSub) Get(key string) string {
	return test.data[key]
}

func (test *TestSub) Set(key string, data string) {
	test.data[key] = data
}

type Test struct {
	data map[string]TestSub
}

func (test *Test) Remove(key string) {
	delete(test.data, key)
}

func (test *Test) Get(key string) TestSub {
	return test.data[key]
}

func (test *Test) Set(key string, data TestSub) {
	test.data[key] = data
}

func main() {
	fmt.Println("Hello World!")
	test := Test{data: map[string]TestSub{}}
	test.Set("1", TestSub{data: map[string]string{"1": "1", "2": "2"}})
	fmt.Println(test)
	test_sub := test.Get("1")
	fmt.Println(test_sub)
	test_sub.Remove("1")
	fmt.Println(test_sub)
	fmt.Println(test)
}
