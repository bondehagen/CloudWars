#include "comm.h"


int main(int argc, char* argv[])
{
	comm c("127.0.0.1");
	if (c.alive())
	{
		c.ready("MY AI");
		while (c.alive()) // simple main loop
		{
			Sleep(1000);
			c.recv_state(state());
			// insert stuff here to do the boogie
			//c.send_wind(vec2(10,10)); 
		}
	}
	Sleep(4000);
	return 0;
}

