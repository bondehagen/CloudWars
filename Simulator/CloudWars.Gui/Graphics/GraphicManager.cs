using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CloudWars.Core.Graphic;

namespace CloudWars.Graphics
{
    public class GraphicManager : IGraphicManager
    {
        private readonly Canvas canvas;
        private readonly string[] colors;
        private int shapeCounter;

        public GraphicManager(Canvas canvas)
        {
            this.canvas = canvas;
            colors = new[] { "blue", "orange", "purple", "red" };
            shapeCounter = -1;
        }

        public dynamic InsertText(string text)
        {
            TextBlock textBlock = new TextBlock
                                      {
                                          Text = text,
                                          RenderTransform = new TranslateTransform()
                                      };
            InsertShape(textBlock);
            return textBlock;
        }

        public dynamic InsertShape(ShapeType type)
        {
            dynamic sprite = CreateShape(type);
            InsertShape(sprite);
            return sprite;
        }

        public void RemoveShape(dynamic sprite)
        {
            canvas.Children.Remove((FrameworkElement) sprite);
        }

        public void UpdateShape(dynamic dsprite, double x, double y, double radius)
        {
            FrameworkElement element = (FrameworkElement) dsprite;
            if (element == null)
                return;

            TranslateTransform translateTransform = element.RenderTransform as TranslateTransform;
            if (translateTransform == null)
                return;


            if (element is Ellipse)
            {
                double width = radius * 2.6;
                double height = radius * 2.6;

                translateTransform.X = x - (width / 2);
                translateTransform.Y = y - (height / 2);

                element.Width = width;
                element.Height = height;
            }
            if (element is TextBlock)
            {
                TextBlock textBlock = (TextBlock) element;
                textBlock.Foreground = new SolidColorBrush(Colors.White);
                translateTransform.X = x - radius;
                translateTransform.Y = y + radius * 2;
            }
        }

        public dynamic CreateShape(ShapeType type)
        {
            switch (type)
            {
                case ShapeType.RainCloud:
                    return createEllipse("gray");
                case ShapeType.ThunderStorm:
                    IncrementShapeCounter();
                    return createEllipse(colors[shapeCounter]);
            }
            throw new Exception("Invalid ShapeType");
        }

        private void IncrementShapeCounter()
        {
            shapeCounter++;
            if (shapeCounter > colors.Length)
                shapeCounter = 0;
        }

        private static Ellipse createEllipse(string color)
        {
            BitmapImage imageSource = new BitmapImage(new Uri(@"sprites\" + color + ".png", UriKind.Relative));
            ImageBrush image = new ImageBrush
                                   {
                                       ImageSource = imageSource,
                                       Stretch = Stretch.Fill,
                                       RelativeTransform = new ScaleTransform(1, 1, 0.61, 0.58)
                                   };
            return new Ellipse
                       {
                           Fill = image,
                           RenderTransform = new TranslateTransform(),
                           RenderTransformOrigin = new Point(0.5, 0.5)
                       };
        }

        public void InsertShape(dynamic sprite)
        {
            canvas.Children.Add(sprite);
        }
    }
}