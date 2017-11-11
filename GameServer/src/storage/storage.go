// 持久化数据 storage.go
package storage

import (
	"config"
	"encoding/csv"
	"errors"
	"fmt"
	"os"
	"protocol"
	"strconv"
)

var data_has_load bool = false
var data_set = map[string]string{}

func StorageDataFile() error {
	csvFile, err := os.Create("data.csv")
	if err != nil {
		return err
	}

	defer csvFile.Close()

	writer := csv.NewWriter(csvFile)
	for k, v := range data_set {
		line := []string{k, v}

		err = writer.Write(line)
		if err != nil {
			return err
		}
	}
	writer.Flush()
	return nil
}

func GetDataFile() error {

	file, err := os.Open("data.csv")
	if err != nil {
		return err
	}

	defer file.Close()

	reader := csv.NewReader(file)
	reader.FieldsPerRecord = -1
	record, err := reader.ReadAll()
	if err != nil {
		return err
	}

	for _, item := range record {
		data_set[item[0]] = item[1]
	}

	return nil
}

func Get(key string) (string, bool) {
	if !data_has_load {
		GetDataFile()
		data_has_load = true
	}
	value, ok := data_set[key]
	return value, ok
}

func Set(key string, value string) bool {
	data_set[key] = value

	//保存文件
	StorageDataFile()
	return true
}

//检查账号密码
func CheckAuthInfo(name string, pwd string) error {
	password_in_db, succ := Get(name)
	if !succ {
		return errors.New("用户名不存在，请先注册")
	}
	if password_in_db != pwd {
		return errors.New("输入密码不正确，请重新输入")
	}

	return nil
}

//拉取房间信息
func GetRoomList() ([]protocol.Room, error) {

	var result []protocol.Room

	var room protocol.Room

	room.ID = "1000"
	room.Name = "房间1"
	result = append(result, room)

	room.ID = "1001"
	room.Name = "房间2"
	result = append(result, room)

	room.ID = "1002"
	room.Name = "房间3"
	result = append(result, room)

	room.ID = "1003"
	room.Name = "房间4"
	result = append(result, room)

	return result, nil
}

var _config_instance *config.Config = nil

func GetConfig(section string, key string) string {

	var err error

	if _config_instance == nil {
		_config_instance, err = config.ReadDefault("work.ini")
		if err != nil {
			fmt.Println(err.Error())
		}
	}

	if _config_instance == nil {
		return ""
	}

	value, err := _config_instance.String(section, key)
	if err != nil {
		fmt.Printf("section:%s,key:%s,value:%s\n", section, key, "")
		return ""
	} else {
		//fmt.Printf("section:%s,key:%s,value:%s\n", section, key, value)
		return value
	}
}

func GetConfigInt(section string, key string) int {
	value, err := strconv.Atoi(GetConfig(section, key))
	if err != nil {
		fmt.Println(err.Error())
		return 0
	}
	return value
}

func GetConfigFloat(section string, key string) float32 {
	value, err := strconv.ParseFloat(GetConfig(section, key), 32)
	if err != nil {
		fmt.Println(err.Error())
		return 0
	}
	return float32(value)
}

//
