// 场景管理 sence project sence.go
package sence

import (
	"fmt"
	"protocol"
)

type Sence interface {

	//场景控制///////////
	Init(senceid string)
	Run()
	Close()
	Destroy()

	//玩家操作///////////
	Join(name string, conn_seq uint64)
	Exit(name string)

	//场景状态///////////
	IsActive() bool
	IsClose() bool

	//消息
	Push(name string, request *protocol.Protocol) error
	Broadcast(request *protocol.Protocol) error
}

type SenceController interface {
	//添加事件
	AddAction(senceid string, action protocol.Protocol)
	//查找可用场景ID，如果已加入场景返回已加入场景ID
	FindSence(name string) (string, error)
	//创建场景
	CreateSence(senceid string) error
	//销毁场景
	DestroySence(sence_id string)

	//加入场景
	JoinSence(name string, scene_id string, conn_seq uint64) error
	//获取玩家所在场景
	GetSence(name string) (string, error)
	//退出场景
	ExitSence(name string)
}

//全局唯一
var sence_manager_instance SenceController

func GetSenceHandler() SenceController {
	if sence_manager_instance == nil {
		sence_manager_instance = CreateSenceMgr()
		fmt.Println("create sence mrg")
	}
	return sence_manager_instance
}
