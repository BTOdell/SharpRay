internal struct Sphere {

    public Vec3 Origin;
    public float Radius;
    public Vec3 Color = new(1, 0, 1);

    public Sphere(Vec3 origin, float radius) {
        this.Origin = origin;
        this.Radius = radius;
    }

}
