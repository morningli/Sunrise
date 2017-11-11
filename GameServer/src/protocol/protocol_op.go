package protocol

import (
	"fmt"
	"math"
	"math/rand"
)

func (v *Vector3) Normalize() *Vector3 {
	length := v.Magnitude()
	fmt.Println("length:", length)
	result := Vector3{X: v.X / length, Y: v.Y / length, Z: v.Z / length}
	fmt.Println("vector:", result)
	fmt.Println("vector len:", result.Magnitude())
	return &result
}

func (v *Vector3) Magnitude() float32 {
	return float32(math.Sqrt(float64(v.X*v.X + v.Y*v.Y + v.Z*v.Z)))
}

//开方速度太慢了。。。
func (v *Vector3) Magnitude2() float32 {
	return v.X*v.X + v.Y*v.Y + v.Z*v.Z
}

func (v *Vector3) Add(s *Vector3) *Vector3 {
	return &Vector3{X: v.X + s.X, Y: v.Y + s.Y, Z: v.Z + s.Z}
}

func (v *Vector3) Multiply(s float32) *Vector3 {
	return &Vector3{X: v.X * s, Y: v.Y * s, Z: v.Z * s}
}

func (v *Vector3) Minus(s *Vector3) *Vector3 {
	return &Vector3{X: v.X - s.X, Y: v.Y - s.Y, Z: v.Z - s.Z}
}

var r = rand.New(rand.NewSource(31))

func (v *Vector3) Random(x_min float32, x_max float32, y_min float32, y_max float32, z_min float32, z_max float32) {
	v.X = r.Float32()*(x_max-x_min) + x_min
	v.Y = r.Float32()*(y_max-y_min) + y_min
	v.Z = r.Float32()*(z_max-z_min) + z_min
}
