using System;
using System.Threading;

namespace ClientAI
{
    public partial class Client
    {
        private readonly Random rnd = new Random();

        // Implement your AI here
        // Use these functions to communicate with server:
        //    SetName(name) - Sets your name to appear in the simulator
        //    GetState() - returns the current GameState object
        //    Wind(x, y) - applies a wind in the given direction (returns true if OK, false if IGNORED)
        public void RunAI()
        {
            SetName("StupidAI");

            while (Connected)
            {
                // Poll the game state
                GameState state = GetState();
                if (state == null) break;

                // Ignore game state and do something random!
                Wind((float) rnd.NextDouble() * 25 - 50, (float) rnd.NextDouble() * 25 - 50);

                Thread.Sleep(500);
            }
        }
    }
}