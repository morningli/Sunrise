// protocol project protocol_logic.go
package protocol

//请求命令
const (
	HERTBEAT = iota
	LOGIN
	REGISTER
	MOVE
	FIRE
	EXIT
	ENTER_ROOM
	SYNCHRONIZE
	GET_ROOM_LIST
	GAME_START
	END_ROOM
)

//三维向量
type Vector3 struct {
	X float32 `json:"x"`
	Y float32 `json:"y"`
	Z float32 `json:"z"`
}

type PlayerStatus struct {
	Type       int     `json:"type"`
	Position   Vector3 `json:"position"`
	Direction  Vector3 `json:"direction"`
	Radius     float32 `json:"radius"`
	Experience int     `json:"experience"`
}

//控制模式//////////////////////////
type DoHertBeat struct {
	Name      string
	Position  Vector3
	Direction Vector3
}

type DoMove struct {
	Name        string  `json:"name"`
	Destination Vector3 `json:"destination"`
}

const (
	FireType_Normal = 0
)

type DoFire struct {
	Name   string
	Target string
	Type   int16
}

//服务器 >> 客户端 频率:帧数
type DoSynchronize struct {
	Room    string                  `json:"room"`
	Status  map[string]PlayerStatus `json:"status"`
	Expired []string                `json:"expired"`
	Survive int                     `json:"survive"`
}

type DoStart struct {
	Room    string                  `json:"room"`
	Status  map[string]PlayerStatus `json:"status"`
	Survive int                     `json:"survive"`
}

type DoEnd struct {
	Room   string                  `json:"room"`
	Status map[string]PlayerStatus `json:"status"`
}

//交互模式//////////////////////////
type DoLoginReq struct {
	Name     string
	Password string
}

type DoLoginRsp struct {
	Key string
}

type DoRegisterReq struct {
	Name     string
	Password string
}

type DoExitReq struct {
	Name string
}

type DoEnterRoomReq struct {
	Name string `json:"name"`
	Room string `json:"room"`
}

type Room struct {
	ID   string `json:"id"`
	Name string `json:"name"`
}
type GetRoomListReq struct {
}

type GetRoomListRsp struct {
	RoomList []Room `json:"roomlist"`
}
