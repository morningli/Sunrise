// protocol project protocol.go
package protocol

//协议起始/结束符
const (
	BEGIN = 0x02
	END   = 0x03
)

//交互协议
type Protocol struct {
	Seq           uint32      `json:"seq"`
	Time          uint32      `json:"time"`
	Key           string      `json:"key"`
	Command       uint8       `json:"command"`
	Request       string      `json:"request"`
	RequestStruct interface{} `json:"-"`
	Respone       bool        `json:"respone"`
	Ret           int16       `json:"ret"`
	Msg           string      `json:"msg"`
}
