using System.Diagnostics;

namespace SharpRay;

internal sealed class Application : IDisposable {

    private readonly Renderer renderer;

    public int Width { get; private set; }
    public int Height { get; private set; }

    /// <summary>
    /// Raw pixel buffer in RGBA format
    /// </summary>
    public uint[] PixelBuffer { get; private set; }

    private readonly object lockObj = new object();
    private volatile Thread? thread = null;

    internal Application(int width, int height) {
        this.renderer = new Renderer(new Scene(), new Camera());
        this.PixelBuffer = this.Resize(width, height);
    }

    public void Start() {
        this.thread = new Thread(this.Run);
        this.thread.Start();
    }

    private void Run() {
        Stopwatch timer = new Stopwatch();
        uint[] lastPixelBuffer = this.PixelBuffer;
        while (this.thread is not null) {
            uint[] currentPixelBuffer;
            int width, height;
            lock (this.lockObj) {
                currentPixelBuffer = this.PixelBuffer;
                width = this.Width;
                height = this.Height;
            }
            if (currentPixelBuffer != lastPixelBuffer) {
                lastPixelBuffer = currentPixelBuffer;
                continue;
            }
            float aspectRatio = (float) width / (float) height;
            timer.Restart();
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    Vec2 coord = new((float) x / (float) width, (float) y / (float) height);
                    coord = coord * 2.0f - Vec2.One; // -1 to 1 (opengl-style)
                    coord = coord with { X = coord.X * aspectRatio }; // fix squishing
                    Vec4 color = this.renderer.PerPixel(coord);
                    currentPixelBuffer[y * width + x] = Application.ConvertToRGBA(Clamp(color, Vec4.Zero, Vec4.One));
                }
            }
            Console.WriteLine("Rendered in: {0}ms", timer.Elapsed.TotalMilliseconds);
        }
    }

    public uint[] Resize(int width, int height) {
        this.Width = width;
        this.Height = height;
        this.PixelBuffer = new uint[width * height];
        return this.PixelBuffer;
    }

    public void Dispose() {
        var thread = this.thread;
        if (thread is null) {
            return;
        }
        this.thread = null;
        thread.Interrupt();
        thread.Join();
    }

    private static uint ConvertToRGBA(in Vec4 v) {
        uint r = (uint)(v.X * 255.0f);
        uint g = (uint)(v.Y * 255.0f);
        uint b = (uint)(v.Z * 255.0f);
        uint a = (uint)(v.W * 255.0f);
        return (r << 24) | (g << 16) | (b << 8) | a;
    }

}
