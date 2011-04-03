using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CloudWars.Core;
using CloudWars.Helpers;

namespace CloudWars.Graphics
{
    public class GameWindow : Window
    {
        private readonly GameSettings settings;

        public GameWindow(GameSettings settings)
        {
            Loaded += WindowLoaded;
            KeyDown += WindowKeyDown;
            
            this.settings = settings;
            InitializeComponents();
        }

        public Canvas Canvas { get; set; }

       // public event EventHandler Draw;


        private void InitializeComponents()
        {
            BitmapImage image = new BitmapImage(new Uri(@"sprites\bg.png", UriKind.Relative));
            ImageBrush bg = new ImageBrush(image) { Stretch = Stretch.Fill };
            Canvas = new Canvas
                         {
                             Height = settings.Height,
                             Width = settings.Width,
                             Background = bg
                         };

            Background = new SolidColorBrush(Colors.Black);

            Viewbox viewbox = new Viewbox { Child = Canvas };
            AddChild(viewbox);
        }

        protected void WindowKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F)
            {
                if (this.IsFullscreen())
                {
                    MaxWidth = settings.Width;
                    MaxHeight = settings.Height;
                    WindowStyle = WindowStyle.SingleBorderWindow;
                    ResizeMode = ResizeMode.CanResize;
                    WindowState = WindowState.Normal;
                    SizeToContent = SizeToContent.Width;
                }
                else
                    this.SetFullscreen();
            }
            if (e.Key == Key.Escape)
                Close();
        }

        protected void WindowLoaded(object sender, RoutedEventArgs e)
        {
            if (settings.Fullscreen)
                this.SetFullscreen();
            else
            {
                Width = settings.Width;
                Height = settings.Height;
                SizeToContent = SizeToContent.Height;
            }
            BringIntoView();
            Activate();
        }
    }
}