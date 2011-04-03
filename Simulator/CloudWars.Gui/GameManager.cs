using System;
using System.Windows.Media;
using CloudWars.Core;
using CloudWars.Graphics;
using CloudWars.Helpers;
using CloudWars.Input;
using CloudWars.Network;

namespace CloudWars
{
    public class GameManager
    {
        private readonly GameSettings settings;
        private GameLoop gameLoop;
        private GameWindow gameWindow;
        private SocketManager socketManager;
        private World world;
        public delegate void CloseGameHandler();
        public event CloseGameHandler Close;
        public bool IsClosed;

        public GameManager(GameSettings settings)
        {
            this.settings = settings;
        }

        private void SocketCancel()
        {
            Stop();
        }


        public void Start()
        {
            IsClosed = false;
            gameLoop = new DispatcherTimerGameLoop(1000 / 100);
            gameLoop.Update += Update;

            gameWindow = new GameWindow(settings);
            gameWindow.Closed += WindowClosed;
            

            SocketFactory socketGame = new SocketFactory(settings);
            socketGame.Cancel += SocketCancel;
            socketManager = socketGame.CreateSocketManager();

            GraphicManager graphicManager = new GraphicManager(gameWindow.Canvas);
            InputFactory inputFactory = new InputFactory(gameWindow, socketManager);
            
            if(IsClosed)
                return;

            world = new World(settings, inputFactory, graphicManager);
            world.Start();
            CompositionTarget.Rendering += Draw;
            gameWindow.Show();
            gameLoop.Start();
        }

        private void Update(TimeSpan elapsed)
        {
            // Update game logic
            world.Update(elapsed);

            if (world.IsFinished) Stop();
        }

        private void Draw(object sender, EventArgs e)
        {
            world.Draw();
        }

        public void Stop()
        {
            IsClosed = true;
            if (socketManager != null)
                socketManager.Stop();

            gameLoop.Stop();

            if (gameWindow.IsEnabled)
                gameWindow.Close();

            if (Close != null)
                Close();
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            Stop();
        }
    }
}