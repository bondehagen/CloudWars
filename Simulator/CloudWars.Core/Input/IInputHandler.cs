using System;

namespace CloudWars.Core.Input
{
    public interface IInputHandler : IDisposable
    {
        void Start();
        void Update(Thunderstorm thunderstorm, World world, int iteration);
    }
}