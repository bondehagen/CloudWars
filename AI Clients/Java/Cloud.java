public class Cloud {
    public Vector Position;
    public Vector Velocity;
    public float Vapor;
    private float radius;

    public float Radius() {
        return (float) Math.sqrt(Vapor);
    }

    public Cloud(float x, float y, float vx, float vy, float vapor) {
        this.Position = new Vector(x, y);
        this.Velocity = new Vector(vx, vy);
        this.Vapor = vapor;
    }
}
