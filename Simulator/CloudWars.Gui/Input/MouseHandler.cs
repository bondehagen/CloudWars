using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CloudWars.Core;
using CloudWars.Core.Input;
using CloudWars.Helpers;

namespace CloudWars.Input
{
    public class MouseHandler : IInputHandler
    {
        private readonly Queue<Vector> eventQueue;
        private readonly UIElement uiElement;

        public MouseHandler(UIElement uiElement)
        {
            this.uiElement = uiElement;
            eventQueue = new Queue<Vector>();
        }

        public void Start()
        {
            uiElement.MouseDown += OnMouseDown;
        }

        public void Update(Thunderstorm thunderstorm, World world, int iteration)
        {
            while (eventQueue.Any())
            {
                Vector position = eventQueue.Dequeue();
                Vector wind = (position - thunderstorm.position);
                wind.Normalize();
                wind *= 50;
                thunderstorm.Wind(wind);
            }
        }

        public void Dispose() {}

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            eventQueue.Enqueue(e.GetPosition((UIElement) sender).ToVector());
        }
    }
}