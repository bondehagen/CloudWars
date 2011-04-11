#ifndef __VECMATH_H
#define __VECMATH_H

#include <math.h>

class vec2
{
public:
	union
	{
		float v[2];
		struct
		{
			float x;
			float y;
		};
	};
	vec2() { ; }
	vec2(float x, float y) : x(x), y(y) { ; }

	vec2& operator+=(const vec2& b) { x += b.x; y += b.y; }
	vec2& operator-=(const vec2& b) { x -= b.x; y -= b.y; }
	
	vec2& operator*=(float c) { x*= c; y*= c; }
	vec2& operator/=(float c) { x/= c; y/= c; }

	float square() const { return x*x + y*y; }
	float length() const { return sqrt(square()); }
	vec2& normalize() { *this /= length(); return *this; }
	inline vec2 norm() const;
};
inline vec2 operator+(const vec2& a, const vec2& b) 
{
	// in MSVS (and with some other compilers?) one can do stuff like this to speed things up a bit
	/*vec2 result;
	__m128 ra;
	__m128 rb;
	ra = _mm_loadl_pi(ra,(__m64*)&a);
	rb = _mm_loadl_pi(rb,(__m64*)&b);
	ra = _mm_add_ps(ra, rb);
	_mm_storel_pi((__m64*)&result,ra);
	return result;*/
	return vec2(a.x+b.x,a.y+b.y);
}
inline vec2 operator-(const vec2& a, const vec2& b) { return vec2(a.x-b.x,a.y-b.y); }
inline vec2 operator*(const vec2& a, float c) { return vec2(a.x*c, a.y*c); }
inline vec2 operator*(float c, const vec2& a) { return a*c; }
inline vec2 operator/(const vec2& a, float c) { return vec2(a.x/c, a.y/c); }
vec2 vec2::norm() const { return (*this)/length(); }

#endif