#ifndef __STATE_H
#define __STATE_H

#include <list>
using std::list;
#include "vecmath.h"

class cloud
{
public:
	vec2 pos;
	vec2 vel;
	float vap;
	cloud() : pos(), vel(), vap(0) { ; }
	cloud(vec2 pos, vec2 vel, float vap) : pos(pos), vel(vel), vap(vap) { ; }
};

class state
{
public:
	state() { ; };
	int turn;
	cloud me;
	list<cloud> tc;
	list<cloud> rc;

};

#endif