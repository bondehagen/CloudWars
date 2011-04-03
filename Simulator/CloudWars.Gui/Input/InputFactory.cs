using System.Windows;
using CloudWars.Core.Input;
using CloudWars.Core.Settings;
using CloudWars.Network;

namespace CloudWars.Input
{
    public class InputFactory : IInputFactory
    {
        private readonly SocketManager socketManager;
        private readonly UIElement window;

        public InputFactory(UIElement window, SocketManager socketManager)
        {
            this.window = window;
            this.socketManager = socketManager;
        }


        public IInputHandler NewInputHandler(CloudType type)
        {
            return type == CloudType.Human ? new MouseHandler(window) : socketManager.GetNetworkPlayer();
        }
    }
}