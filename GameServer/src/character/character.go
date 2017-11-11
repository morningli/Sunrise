// character project character.go
package character

import (
	"fmt"
	"protocol"
	"storage"
	"strconv"
	"strings"
)

func get_level_conf_key(lvl int, key string) string {
	return "lvl_" + strconv.Itoa(lvl) + "_" + key
}

const (
	Type_Player = iota
	Type_Food
)

type Character struct {
	_name       string
	_direction  protocol.Vector3
	_position   protocol.Vector3
	_blood      int
	_experience int
	_type       int
	//_level      int
}

func (character *Character) GetExperience() int {
	return character._experience
}

func (character *Character) SetName(name string) {
	character._name = name
}

func (character *Character) GetName() string {
	return character._name
}

func (character *Character) InitCharacter(name string, charater_type int) {
	character._name = name
	character._type = charater_type
	if charater_type == Type_Food {
		character._experience = 1
	}

	character._position = protocol.Vector3{}
	character._direction = protocol.Vector3{}

	//初始位置
	rand_init_x_min := storage.GetConfigFloat("sence", "rand_init_x_min")
	rand_init_x_max := storage.GetConfigFloat("sence", "rand_init_x_max")
	rand_init_y_min := storage.GetConfigFloat("sence", "rand_init_y_min")
	rand_init_y_max := storage.GetConfigFloat("sence", "rand_init_y_max")
	rand_init_z_min := storage.GetConfigFloat("sence", "rand_init_z_min")
	rand_init_z_max := storage.GetConfigFloat("sence", "rand_init_z_max")

	axis_init_x_is_fix := storage.GetConfigInt("sence", "axis_init_x_is_fix")
	axis_fix_init_x := storage.GetConfigFloat("sence", "axis_fix_init_x")
	axis_init_y_is_fix := storage.GetConfigInt("sence", "axis_init_y_is_fix")
	axis_fix_init_y := storage.GetConfigFloat("sence", "axis_fix_init_y")
	axis_init_z_is_fix := storage.GetConfigInt("sence", "axis_init_z_is_fix")
	axis_fix_init_z := storage.GetConfigFloat("sence", "axis_fix_init_z")

	character._position.Random(rand_init_x_min, rand_init_x_max, rand_init_y_min, rand_init_y_max, rand_init_z_min, rand_init_z_max)

	if axis_init_x_is_fix == 1 {
		character._position.X = axis_fix_init_x
	}
	if axis_init_y_is_fix == 1 {
		character._position.Y = axis_fix_init_y
	}
	if axis_init_z_is_fix == 1 {
		character._position.Z = axis_fix_init_z
	}

	//初始方向
	axis_x_is_fix := storage.GetConfigInt("sence", "axis_x_is_fix")
	axis_y_is_fix := storage.GetConfigInt("sence", "axis_y_is_fix")
	axis_z_is_fix := storage.GetConfigInt("sence", "axis_z_is_fix")

	character._direction.Random(0, 1, 0, 1, 0, 1)

	if axis_x_is_fix == 1 {
		character._direction.X = 0
	}
	if axis_y_is_fix == 1 {
		character._direction.Y = 0
	}
	if axis_z_is_fix == 1 {
		character._direction.Z = 0
	}

	//玩家初始状态
	blood := storage.GetConfigInt("character", get_level_conf_key(character.GetLevel(), "blood"))

	//character._level = 0
	character._blood = blood
}

func (character *Character) Move(des protocol.Vector3, time float32) {
	//限制方向
	axis_x_is_fix := storage.GetConfigInt("sence", "axis_x_is_fix")
	axis_y_is_fix := storage.GetConfigInt("sence", "axis_y_is_fix")
	axis_z_is_fix := storage.GetConfigInt("sence", "axis_z_is_fix")

	//坐标值限制
	axis_x_min := storage.GetConfigFloat("sence", "axis_x_min")
	axis_x_max := storage.GetConfigFloat("sence", "axis_x_max")
	axis_y_min := storage.GetConfigFloat("sence", "axis_y_min")
	axis_y_max := storage.GetConfigFloat("sence", "axis_y_max")
	axis_z_min := storage.GetConfigFloat("sence", "axis_z_min")
	axis_z_max := storage.GetConfigFloat("sence", "axis_z_max")

	if axis_x_is_fix == 1 {
		des.X = 0
	}
	if axis_y_is_fix == 1 {
		des.Y = 0
	}
	if axis_z_is_fix == 1 {
		des.Z = 0
	}
	//计算方向
	character._direction = *des.Minus(&character._position)
	character._direction = *character._direction.Normalize()

	fmt.Println("enter move")
	fmt.Println(character._direction)

	//计算移动后偏移量
	distance := character.GetSpeed() * time

	fmt.Printf("distance:%f,time:%f\n", distance, time)

	//计算移动后位置
	offset := character._direction.Multiply(distance)
	fmt.Println(offset)
	fmt.Println(character._position)
	character._position = *character._position.Add(offset)

	//位置修正
	if character._position.X < axis_x_min {
		character._position.X = axis_x_min
	}
	if character._position.X > axis_x_max {
		character._position.X = axis_x_max
	}
	if character._position.Y < axis_y_min {
		character._position.Y = axis_y_min
	}
	if character._position.Y > axis_y_max {
		character._position.Y = axis_y_max
	}
	if character._position.Z < axis_z_min {
		character._position.Z = axis_z_min
	}
	if character._position.Z > axis_z_max {
		character._position.Z = axis_z_max
	}

	fmt.Println(character._position)
}

func (character *Character) Meet(des *Character) {
	//判断距离
	dist := character.GetRadius() // + des.GetRadius()
	dist *= dist
	dist_r2 := character._position.Minus(&des._position).Magnitude2()
	if dist_r2 > dist {
		return
	}

	fmt.Println("pos:", character._position)
	fmt.Println("des pos:", des._position)
	fmt.Println("dist:", dist_r2)
	fmt.Println("radius:", dist)

	//对象为食物则随便吃
	if des._type == Type_Food {
		character.Eat(des)
	}
	//等级大的吃掉等级小的，相同等级无变化
	if character.GetLevel() > des.GetLevel() {
		character.Eat(des)
	} else if character.GetLevel() < des.GetLevel() {
		des.Eat(character)
	}
}

func (character *Character) Eat(des *Character) {
	des._blood = 0
	character._experience += des._experience

	fmt.Printf("eat|exp:%d,lvl:%d\n", character._experience, character.GetLevel())
}

func (character *Character) IsDead() bool {
	return character._blood == 0
}

var _exp_list = []int{}

func (character *Character) GetLevel() int {
	//return character._level
	if len(_exp_list) == 0 {
		exp_conf := storage.GetConfig("character", "level_exp")
		exp_list := strings.Split(exp_conf, ",")
		for k := range exp_list {
			exp, _ := strconv.Atoi(exp_list[k])
			_exp_list = append(_exp_list, exp)
		}
	}

	for i := len(_exp_list); i > 0; i -= 1 {
		if character._experience > _exp_list[i-1] {
			return i
		}
	}
	return 0
}
func (character *Character) GetSpeed() float32 {
	speed := storage.GetConfigFloat("character", get_level_conf_key(character.GetLevel(), "speed"))
	fmt.Println("speed:", speed)
	return speed
}
func (character *Character) GetMass() int {
	mass := storage.GetConfigInt("character", get_level_conf_key(character.GetLevel(), "mass"))
	return mass
}
func (character *Character) GetBlood() int {
	return character._blood
}
func (character *Character) GetPosition() protocol.Vector3 {
	return character._position
}
func (character *Character) GetDirection() protocol.Vector3 {
	return character._direction
}
func (character *Character) GetRadius() float32 {
	radius := storage.GetConfigFloat("character", get_level_conf_key(character.GetLevel(), "radius"))
	return radius
}
func (character *Character) GetType() int {
	return character._type
}
