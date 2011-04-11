#include "comm.h"
#include <stdio.h>
#include <WS2tcpip.h>

class winsock
{
public:
	winsock();
	~winsock();
	WSADATA wsa_data;
};

winsock::winsock()
{
	const int initres = WSAStartup(MAKEWORD(2,2), &wsa_data);
	printf("WSAStartup result: %d\n", initres);
}
winsock::~winsock()
{
	WSACleanup();
	printf("WSACleanup done\n");
}

winsock ws;


comm::comm(const char* const hostname)
{
	struct addrinfo hint;
	struct addrinfo* ai;
	ZeroMemory(&hint, sizeof(struct addrinfo));
	hint.ai_family = AF_UNSPEC;
	hint.ai_socktype = SOCK_STREAM;
	hint.ai_protocol = IPPROTO_TCP;

	_socket = INVALID_SOCKET;

	if (int rc = getaddrinfo(hostname, "1986", &hint, &ai))
	{
		printf("getaddrinfo failed: %d\n", rc);
		return;
	}

	_socket = socket(ai->ai_family, ai->ai_socktype, ai->ai_protocol);
	if (_socket == INVALID_SOCKET)
		printf("socket error: %ld\n", WSAGetLastError());
	else if (int rc = connect( _socket, ai->ai_addr, (int)ai->ai_addrlen))
	{
		printf("could not connect socket: %ld\n", WSAGetLastError());
		closesocket(_socket);
		_socket = INVALID_SOCKET;
	}
	else
		printf("socket up and running\n");
	freeaddrinfo(ai);
}

comm::~comm()
{
	if (_socket != INVALID_SOCKET)
		closesocket(_socket);
}

void comm::_send(const char* const buf, int len)
{
	if (_socket == INVALID_SOCKET)
		return;
	if (send(_socket, buf, len, 0)==SOCKET_ERROR)
	{
		printf("socket send failed: %ld, disabling socket\n", WSAGetLastError());
		closesocket(_socket);
		_socket = INVALID_SOCKET;
	}
}
int  comm::_recv(char* const buf, int max)
{
	if (_socket == INVALID_SOCKET)
		return 0;
	int rc = recv(_socket, buf, max, 0);
	if (rc <= 0)
	{
		if (rc == 0)
			printf("connection closed while waiting for data\n");
		else
			printf("socket recv failed: %ld, disabling socket\n");
		closesocket(_socket);
		_socket = INVALID_SOCKET;
		return 0;
	}
	else
		return rc;
}
int  comm::_readline(char* const buf, int max)
{
	if (_socket == INVALID_SOCKET)
		return 0;
	int i = 0;
	for (i = 0; i < max; ) 
	{
		const int len = recv(_socket, buf+i, 1, 0);
		if (len <= 0)
		{
			if (len == 0)
				printf("connection closed while waiting for data\n");
			else
				printf("socket recv failed: %ld, disabling socket\n");
			closesocket(_socket);
			_socket = INVALID_SOCKET;
			buf[0] = 0;
			return 0;
		}
		if (buf[i] == '\n')
		{
			buf[i] = 0;
			return i;
		}
		if (buf[i] != '\r')
			i++;
	}
}

void comm::ready(const char* const playername)
{
	const int buflen = 64;
	char buf[buflen];
	printf("sending NAME\n");
	_send(buf, sprintf(buf, "NAME %s\n", playername));
	const int recvlen = _readline(buf, buflen);
	buf[recvlen] = 0;
	printf("recieved %s\n", buf);
}

void comm::send_wind(const vec2& wind)
{
	const int buflen = 32;
	char buf[buflen];
	printf("sending WIND %f %f\n", wind.x, wind.y);
	_send(buf, sprintf(buf, "WIND %f %f\n", wind.x, wind.y));
	const int recvlen = _readline(buf,buflen);
	buf[recvlen] = 0;
	printf("recieved %s\n", buf);
}

void comm::recv_state(state& s)
{
	const int buflen = 1024;
	char buf[buflen+1];
	char* args;

	int len;
	int player = -1;
	int tci = 0;
	cloud c;

	printf("sending GET_STATE\n");
	_send(buf, sprintf(buf, "GET_STATE\n"));
	do{
		len = _readline(buf,buflen+1);
		printf("%s\n", buf);
		args = strchr(buf, ' ');
		*args = 0;
		args++;
		if (!strcmp(buf,"BEGIN_STATE"))
			sscanf(args,"%d", &s.turn);
		if (!strcmp(buf,"YOU"))
			sscanf(args,"%d", &player);
		if (!strcmp(buf,"THUNDERSTORM"))
		{
			sscanf(args,"%f %f %f %f %f", c.pos.x, c.pos.y, c.vel.x, c.vel.y, c.vap);
			if (tci == player)
				s.me = c;
			else
				s.tc.push_back(c);
			tci++;
		}
		if (!strcmp(buf,"RAINCLOUD"))
		{
			sscanf(args,"%f %f %f %f %f", c.pos.x, c.pos.y, c.vel.x, c.vel.y, c.vap);
			s.rc.push_back(c);
		}
		if (!strcmp(buf,"END_STATE"))
			break;
	} 
	while (len > 0);

}