// 场景管理 sence project sence_logic.go
package sence

import (
	"character"
	"connection"
	"errors"
	"fmt"
	"online"
	"protocol"
	"storage"
	"strconv"
	"time"
)

const (
	SenceStatus_Wait = iota
	SenceStatus_Active
	SenceStatus_Close
)

const (
	Millisecond_interval = 30
)

func ConvertCharater(player *character.Character) protocol.PlayerStatus {
	var result protocol.PlayerStatus
	result.Position = player.GetPosition()
	result.Direction = player.GetDirection()
	result.Radius = player.GetRadius()
	result.Type = player.GetType()
	result.Experience = player.GetExperience()
	return result
}

type SenceInfo struct {
	id           string
	create_time  int64
	status       int
	action_queue chan protocol.Protocol
	food_seq     int
	players      map[string]*character.Character //玩家
	foods        map[string]*character.Character
	conns        map[string]uint64 //包含玩家及观察者
	expired      []string          //已经移除的物品
	change       []string          //有变更的物品
	time         uint32            //时钟
}

func (sence_info *SenceInfo) Init(senceid string) {
	sence_info.id = senceid
	sence_info.create_time = time.Now().Unix()
	sence_info.status = SenceStatus_Wait
	sence_info.action_queue = make(chan protocol.Protocol, 100)
	sence_info.players = make(map[string]*character.Character)
	sence_info.change = []string{}
	sence_info.foods = make(map[string]*character.Character)
	sence_info.food_seq = 0
	sence_info.conns = make(map[string]uint64)
	sence_info.expired = []string{}

	//食物初始化
	food_num := storage.GetConfigInt("sence", "food_max")
	for i := 0; i < food_num; i++ {
		sence_info.AddFood()
	}
}

func (sence_info *SenceInfo) IsActive() bool {
	return sence_info.status == SenceStatus_Active
}

func (sence_info *SenceInfo) IsClose() bool {
	return sence_info.status == SenceStatus_Close
}

func (sence_info *SenceInfo) GetPlayerNum() int {
	return len(sence_info.players)
}

func (sence_info *SenceInfo) AddAction(action protocol.Protocol) {
	sence_info.action_queue <- action
}

func (sence_info *SenceInfo) Destroy() {
	fmt.Printf("senceid:%s destroy", sence_info.id)
}

func (sence_info *SenceInfo) AddFood() *character.Character {
	key_food := "food_" + strconv.Itoa(sence_info.food_seq)
	sence_info.food_seq += 1

	food := new(character.Character)
	food.InitCharacter(key_food, character.Type_Food)

	sence_info.foods[key_food] = food

	return food
}

func (sence_info *SenceInfo) DelFood(name string) {
	delete(sence_info.foods, name)
	//新增食物
	food := sence_info.AddFood()
	//新增消息
	sence_info.change = append(sence_info.change, food.GetName())
}

//旁观模式
func (sence_info *SenceInfo) AddObserver(name string, conn_seq uint64) error {
	sence_info.conns[name] = conn_seq
	return nil
}

func (sence_info *SenceInfo) Exit(name string) {
	_, ok := sence_info.conns[name]
	if ok {
		delete(sence_info.conns, name)
	}
	_, ok = sence_info.players[name]
	if ok {
		delete(sence_info.players, name)
	}
}

func (sence_info *SenceInfo) Join(name string, conn_seq uint64) error {
	min := storage.GetConfigInt("sence", "player_min")
	max := storage.GetConfigInt("sence", "player_max")

	_, ok := sence_info.conns[name]
	if ok {
		//玩家掉线重连？
		sence_info.conns[name] = conn_seq
	} else {

		if len(sence_info.players) > max {
			return errors.New("当前场景已满无法加入")
		}

		var ch character.Character
		ch.InitCharacter(name, character.Type_Player)
		sence_info.players[name] = &ch

		sence_info.conns[name] = conn_seq

		//人数达到最低开局人数
		if len(sence_info.players) >= min {
			sence_info.Run()
		}

		//加入消息
		sence_info.change = append(sence_info.change, name)
	}

	if sence_info.IsActive() {
		//同步请求
		sence_info.Push(name, sence_info.GetStartRequest())
	}

	return nil
}

//TODO 游戏中被打败及玩家退出不相同
func (sence_info *SenceInfo) DelPlayer(name string) {
	_, ok := sence_info.players[name]
	if ok {
		delete(sence_info.players, name)
		//不支持复活，直接退出场景
		//SenceManagerInstance.ExitSence(name)
	}
}

func (sence_info *SenceInfo) GetPlayerStatus(only_player bool) map[string]protocol.PlayerStatus {
	status := make(map[string]protocol.PlayerStatus)
	for k, v := range sence_info.players {
		status[k] = ConvertCharater(v)
	}

	if !only_player {

		for k, v := range sence_info.foods {
			status[k] = ConvertCharater(v)
		}

	}
	return status
}

func (sence_info *SenceInfo) GetPlayerStatusChange() map[string]protocol.PlayerStatus {
	result := make(map[string]protocol.PlayerStatus)
	for _, k := range sence_info.change {
		status, ok := sence_info.players[k]
		if ok {
			result[k] = ConvertCharater(status)
		}
		status, ok = sence_info.foods[k]
		if ok {
			result[k] = ConvertCharater(status)
		}
	}
	return result
}

func (sence_info *SenceInfo) GetEndRequest() *protocol.Protocol {

	status := sence_info.GetPlayerStatus(true)
	request := protocol.DoEnd{Room: sence_info.id, Status: status}
	return &protocol.Protocol{Command: protocol.END_ROOM, Time: sence_info.time, RequestStruct: request}

}

func (sence_info *SenceInfo) GetStartRequest() *protocol.Protocol {

	status := sence_info.GetPlayerStatus(false)
	request := protocol.DoStart{Room: sence_info.id, Status: status, Survive: len(sence_info.players)}
	return &protocol.Protocol{Command: protocol.GAME_START, Time: sence_info.time, RequestStruct: request}

}

func (sence_info *SenceInfo) GetSyncRequest(only_change bool) *protocol.Protocol {

	status := make(map[string]protocol.PlayerStatus)
	expire := []string{}

	if only_change {
		expire = sence_info.expired
		status = sence_info.GetPlayerStatusChange()
	} else {
		status = sence_info.GetPlayerStatus(false)
	}
	if len(expire) == 0 && len(status) == 0 {
		return nil
	} else {
		request := protocol.DoSynchronize{Room: sence_info.id, Status: status, Expired: expire, Survive: len(sence_info.players)}
		return &protocol.Protocol{Command: protocol.SYNCHRONIZE, Time: sence_info.time, RequestStruct: request}
	}
}

//同步场景信息
func (sence_info *SenceInfo) Push(name string, request *protocol.Protocol) error {

	if request == nil {
		return nil
	}

	conn, ok := sence_info.conns[name]
	if !ok {
		return errors.New("玩家不在场景中")
	}

	var conn_data connection.ConnData
	err := online.GetConnection(conn, &conn_data)
	if err != nil {
		fmt.Println(err)
		return err
	}
	conn_data.SendRequest(*request)

	return nil
}

//场景广播
func (sence_info *SenceInfo) Broadcast(request *protocol.Protocol) error {

	if request == nil {
		return nil
	}

	//获取链接信息
	for _, conn := range sence_info.conns {
		var conn_data connection.ConnData
		err := online.GetConnection(conn, &conn_data)
		if err != nil {
			fmt.Println(err)
			continue
		}
		conn_data.SendRequest(*request)
	}
	return nil
}

func (sence_info *SenceInfo) StopOneCycle(time uint32) {
	request := protocol.Protocol{Time: time, Command: protocol.SYNCHRONIZE}
	sence_info.action_queue <- request
}

func (sence_info *SenceInfo) Event() {
	for k1, v1 := range sence_info.players {
		for k2, v2 := range sence_info.players {
			if k1 != k2 {
				v1.Meet(v2)
			}
		}
	}
	for k1, v1 := range sence_info.players {
		for k2, v2 := range sence_info.foods {
			if k1 != k2 {
				v1.Meet(v2)
			}
		}
	}
	for k, v := range sence_info.players {
		if v.IsDead() {
			sence_info.DelPlayer(k)
			sence_info.expired = append(sence_info.expired, k)
		}
	}
	for k, v := range sence_info.foods {
		if v.IsDead() {
			sence_info.DelFood(k)
			sence_info.expired = append(sence_info.expired, k)
		}
	}
	//游戏结束条件:玩家数为1时游戏结束
	if len(sence_info.players) == 1 {
		sence_info.status = SenceStatus_Close
		//广播结束消息
		sence_info.Broadcast(sence_info.GetEndRequest())
	}

}

func (sence_info *SenceInfo) Update() {
	var err error
	request_get := make(map[string]int)

	for {
		select {
		case request := <-sence_info.action_queue:
			//过期请求
			if request.Time < sence_info.time {
				fmt.Printf("ignore.request time:%d, time:%d.\n", request.Time, sence_info.time)
			}
			switch request.Command {
			case protocol.MOVE:
				data, ok := request.RequestStruct.(protocol.DoMove)
				if !ok {
					fmt.Println("解包失败")
					continue
				}

				num, ok := request_get[data.Name]
				if ok {
					request_get[data.Name] += 1
					fmt.Println("request repeat", num+1)
				} else {
					request_get[data.Name] = 1
				}

				player, ok := sence_info.players[data.Name]
				if !ok {
					fmt.Println("玩家不在该场景")
					continue
				}

				player.Move(data.Destination, float32(Millisecond_interval)/1000)

				//TODO 吃掉其他食物
				//TODO 是否吃掉其他玩家
				sence_info.change = append(sence_info.change, data.Name)

			case protocol.SYNCHRONIZE:
				//系统发起请求，不需要解包
				//第一阶段:接受请求，处理请求
				//第二阶段:触发事件
				sence_info.Event()
				//第三阶段:广播事件
				err = sence_info.Broadcast(sence_info.GetSyncRequest(true))
				if err != nil {
					fmt.Println(err)
				}
				sence_info.change = []string{}
				sence_info.expired = []string{}
				return
			}
		}
	}
}

func (sence_info *SenceInfo) Run() {
	sence_info.status = SenceStatus_Active
	ticker := time.NewTicker(time.Millisecond * Millisecond_interval)
	//消息队列
	//推送队列
	//时钟
	go func() {
		//推送开始消息
		sence_info.Broadcast(sence_info.GetStartRequest())

		for sence_info.time = uint32(0); ; sence_info.time++ {
			//fmt.Println("star new tick")
			select {
			case <-ticker.C:
				if sence_info.IsClose() {
					//游戏结束
					return
				}
				start := time.Nanosecond
				//终止当前时间周期
				sence_info.StopOneCycle(sence_info.time)
				//处理用户请求，更新场景信息并同步给用户
				sence_info.Update()
				end := time.Nanosecond
				if end-start > Millisecond_interval {
					fmt.Println("run time:", (end-start)/1000000)
				}
			}
		}
	}()
	sence_info.status = SenceStatus_Active
}

/////////////////场景管理//////////////////////////////

type SenceMgr struct {
	sence_map    map[string]*SenceInfo
	sence_player map[string]string
}

func (mgr *SenceMgr) DestroySence(sence_id string) {
	_, ok := mgr.sence_map[sence_id]
	if ok {
		delete(mgr.sence_map, sence_id)
	}
}

//获取可用场景
func (mgr *SenceMgr) FindSence(name string) (string, error) {

	sence_id, ok := mgr.sence_player[name]
	if ok {
		//场景是否已关闭
		sence, ok2 := mgr.sence_map[sence_id]
		if ok2 && !sence.IsClose() {
			return sence_id, nil
		} else {
			mgr.ExitSence(name)
			if ok2 {
				delete(mgr.sence_map, sence_id)
			}
		}
	}

	max := storage.GetConfigInt("sence", "player_max")
	sence_max := storage.GetConfigInt("sence", "sence_max")

	for k, v := range mgr.sence_map {
		if max > v.GetPlayerNum() {
			return k, nil
		}
	}

	if len(mgr.sence_map) >= sence_max {
		return "", errors.New("无法创建更多场景")
	}

	//没有合适的场景则创建场景
	sence_id = strconv.Itoa(len(mgr.sence_map))
	//fmt.Printf("new sence:%s\n", sence_id)

	err := mgr.CreateSence(sence_id)
	if err != nil {
		return "", err
	}

	return sence_id, nil
}

//创建场景
func (mgr *SenceMgr) CreateSence(senceid string) error {
	sence := new(SenceInfo)
	sence.Init(senceid)
	mgr.sence_map[senceid] = sence
	return nil
}

func (mgr *SenceMgr) AddAction(senceid string, action protocol.Protocol) {
	sence, ok := mgr.sence_map[senceid]
	if !ok {
		//fmt.Println("场景不存在")
		return
	}
	sence.AddAction(action)
}

//获取玩家场景号
func (mgr *SenceMgr) GetSence(name string) (string, error) {
	sence, ok := mgr.sence_player[name]
	if ok {
		return sence, nil
	}
	fmt.Println(mgr.sence_player)
	return "", errors.New("玩家未加入任何场景")
}

//玩家进入场景
func (mgr *SenceMgr) JoinSence(name string, sence_id string, conn_seq uint64) error {
	sence, ok := mgr.sence_map[sence_id]
	if !ok {
		return errors.New("场景不存在")
	}
	if sence.IsClose() {
		mgr.ExitSence(name)
		return errors.New("无法加入该场景")
	}
	//加入当前场景
	mgr.sence_player[name] = sence_id
	err := sence.Join(name, conn_seq)
	if err != nil {
		return err
	}

	fmt.Println("join succ:", name)
	fmt.Println(mgr.sence_player)

	return nil
}

//玩家退出场景
func (mgr *SenceMgr) ExitSence(name string) {
	senceid, ok := mgr.sence_player[name]
	if ok {
		sence, ok_s := mgr.sence_map[senceid]
		if ok_s {
			sence.Exit(name)
		}
	}
	delete(mgr.sence_player, name)
}

func CreateSenceMgr() *SenceMgr {
	return &SenceMgr{sence_map: make(map[string]*SenceInfo), sence_player: make(map[string]string)}
}
