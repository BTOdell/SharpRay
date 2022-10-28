using Silk.NET.Maths;

namespace SharpRay;

internal sealed class Renderer {

    private readonly Scene scene;
    private readonly Camera camera;

    internal Renderer(Scene scene, Camera camera) {
        this.scene = scene;
        this.camera = camera;
    }

    public Vec4 PerPixel(in Vec2 coord) {
        Ray ray = new Ray(this.camera.Position, this.camera.Direction + new Vec3(coord, 0));

        Vec3 color = new(0);
        float multiplier = 1.0f;

        int bounces = 2;
        for (int b = 0; b < bounces; b++) {
            HitResult? result = this.scene.TraceRay(ray);
            if (result is null) {
                color += this.scene.SkyColor * multiplier;
                break;
            }

            Ray normal = result.Value.Normal;
            ref readonly Sphere sphere = ref this.scene.GetSphere(result.Value.ObjectIndex);
            Vec3 sphereColor = sphere.Color * this.scene.LightIntensity(normal);
            color += sphereColor * multiplier;

            multiplier *= 0.7f;

            ray.Origin = normal.Origin + normal.Direction * 0.0001f; // TODO figure out bounces without hacky offset
            ray.Direction = Vector3D.Reflect(ray.Direction, normal.Direction);
        }

        return new Vec4(color, 1);
    }

}
