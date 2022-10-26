internal class Camera {

    public Vec3 Position;
    public Vec3 Direction;

    public Camera() {
        this.Position = new(0, 0, 3);
        this.Direction = new(0, 0, -1);
    }

}
