internal readonly struct HitResult {

    public readonly Ray Normal;
    public readonly int ObjectIndex;

    public HitResult(Ray normal, int objectIndex) {
        this.Normal = normal;
        this.ObjectIndex = objectIndex;
    }

}
