namespace SharpRay;

internal sealed class Renderer {

    private readonly Scene scene;
    private readonly Camera camera;

    internal Renderer(Scene scene, Camera camera) {
        this.scene = scene;
        this.camera = camera;
    }

    public Vec4 PerPixel(in Vec2 coord) {
        Vec3 rayDirection = this.camera.Direction + new Vec3(coord, 0);
        HitResult? result = this.scene.TraceRay(new Ray(this.camera.Position, rayDirection));
        if (result is null) {
            return new Vec4(0, 0, 0, 1);
        }
        Ray ray = result.Value.Normal;
        Vec3 sphereColor = this.scene.ObjectColor(result.Value.ObjectIndex);
        sphereColor *= this.scene.LightIntensity(ray);
        return new Vec4(sphereColor, 1);
    }

}
