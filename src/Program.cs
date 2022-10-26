using SharpRay;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

Vector2D<int> lastSize = Vector2D<int>.Zero;
Application? app = null;
GL? gl = null;
uint tex = 0;
uint fbo = 0;

WindowOptions windowOptions = WindowOptions.Default;
windowOptions.Title = "SharpRay: A raytracing benchmark for C# .NET";
windowOptions.Size = new(800, 800);

using IWindow window = Window.Create(windowOptions);

window.Load += () => {
    var size = window.FramebufferSize;
    Console.WriteLine("Framebuffer size: {0}", size);
    int width = size.X;
    int height = size.Y;

    // Initialize application
    app = new Application(width, height);

    // Start up OpenGL context
    gl = window.CreateOpenGL();
    gl.ClearColor(1.0f, 0.0f, 0.0f, 1.0f);

    // Create 2D texture
    gl.GenTextures(1, out tex);
    gl.BindTexture(TextureTarget.Texture2D, tex);
    gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba8, (uint) width, (uint) height, 0, PixelFormat.Rgba, PixelType.UnsignedInt8888, (ReadOnlySpan<uint>) app.PixelBuffer.AsSpan());

    // Create FBO (Framebuffer Object)
    gl.GenFramebuffers(1, out fbo);
    gl.BindFramebuffer(FramebufferTarget.ReadFramebuffer, fbo);
    gl.FramebufferTexture2D(FramebufferTarget.ReadFramebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, tex, 0);

    // Start up application
    app.Start();

    lastSize = size;
};
window.Closing += () => {
    app?.Dispose();
};
window.Render += (double delta) => {
    if (app is null || gl is null) {
        return;
    }
    gl.Clear(ClearBufferMask.ColorBufferBit);

    var size = window.FramebufferSize;
    int width = size.X;
    int height = size.Y;

    // Check if texture needs to be resized
    if (lastSize != size) {
        var newPixelBuffer = app.Resize(width, height);
        // Resize texture
        gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint) width, (uint) height, 0, PixelFormat.Rgba, PixelType.UnsignedInt8888, (ReadOnlySpan<uint>) newPixelBuffer.AsSpan());
        lastSize = size;
    } else {
        // Copy pixel buffer to texture
        gl.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, (uint) width, (uint) height, PixelFormat.Rgba, PixelType.UnsignedInt8888, (ReadOnlySpan<uint>) app.PixelBuffer.AsSpan());
    }

    // Blit texture framebuffer to screen framebuffer
    gl.BlitFramebuffer(0, 0, width, height, 0, 0, width, height, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest);
};

window.Run();
