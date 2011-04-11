#ifndef __COMM_H
#define __COMM_H

#pragma comment(lib, "Ws2_32.lib")

#include <winsock2.h>
#include "state.h"
#include "vecmath.h"

class comm
{
	SOCKET _socket;
	void _send(const char* const buf, int len);
	int  _recv(char* const buf, int max);
	int  _readline(char* const buf, int max);
public:
	comm(const char * const hostname);
	~comm();

	bool alive() const { return _socket != INVALID_SOCKET; }

	void ready(const char* const playername);
	void send_wind(const vec2& wind);
	void recv_state(state& s);
};

#endif