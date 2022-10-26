using System.Runtime.InteropServices;

namespace SharpRay;

internal sealed class Scene {

    private readonly Vec3 lightDirection = Normalize(new Vec3(-1, -1, -1));

    private readonly List<Sphere> spheres;

    internal Scene() {
        this.spheres = new List<Sphere>();
        {
            Sphere sphere = new(new(0), 0.5f);
            this.spheres.Add(sphere);
        }
        {
            Sphere sphere = new(new(1, 0, -5), 1.5f);
            sphere.Color = new(0.2f, 0.3f, 1.0f);
            this.spheres.Add(sphere);
        }
    }

    public HitResult? TraceRay(in Ray ray) {
        float a = Dot(ray.Direction, ray.Direction); // never negative
        float _2a = 2.0f * a;

        int objectIndex = -1;
        float hitDistance = float.MaxValue;

        int i = 0;
        ReadOnlySpan<Sphere> spheres = CollectionsMarshal.AsSpan(this.spheres);
        foreach (ref readonly Sphere sphere in spheres) {
            Vec3 rel = ray.Origin - sphere.Origin;

            float b = 2.0f * Dot(rel, ray.Direction);
            float c = Dot(rel, rel) - (sphere.Radius * sphere.Radius);

            float bb = b * b;
            float discriminant = bb - (4.0f * a * c);
            if (discriminant >= 0.0f) {
                // TODO how to avoid sqrt before checking closestT > 0 condition
                // maybe check bb > discriminant?
                float closestT = (-b - MathF.Sqrt(discriminant)) / _2a; // smallest value, closest hit
                if (closestT > 0.0f && closestT < hitDistance) {
                    hitDistance = closestT;
                    objectIndex = i;
                }
            }

            i++;
        }

        if (objectIndex < 0) {
            return null;
        }

        ref readonly Sphere closestSphere = ref spheres[objectIndex];

        Vec3 hitPoint = ray.Origin + ray.Direction * hitDistance;
        Vec3 normal = Normalize(hitPoint - closestSphere.Origin);

        return new HitResult(new Ray(hitPoint, normal), objectIndex);
    }

    public float LightIntensity(in Ray ray) {
        return MathF.Max(0.0f, Dot(ray.Direction, -this.lightDirection));
    }

    public Vec3 ObjectColor(int objectIndex) {
        return this.spheres[objectIndex].Color;
    }

}
