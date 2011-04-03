namespace CloudWars.Core.Graphic
{
    public interface IGraphicManager
    {
        dynamic InsertText(string text);
        dynamic InsertShape(ShapeType type);
        void RemoveShape(dynamic shape);
        void UpdateShape(dynamic shape, double x, double y, double radius);
    }
}