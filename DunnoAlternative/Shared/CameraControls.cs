using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace DunnoAlternative.Shared;

internal class CameraControls : IDisposable
{
    private readonly RenderWindow window;
    private readonly Camera camera;

    public CameraControls(RenderWindow window, Camera camera)
    {
        this.window = window;
        this.camera = camera;
    }

    public void SetupControls()
    {
        window.Resized += Resized;
        window.MouseMoved += MouseMoved;
        window.MouseButtonPressed += MousePressed;
        window.MouseButtonReleased += MouseReleased;
        window.MouseWheelScrolled += MouseWheelScrolled;
    }

    public void UnSetupControls()
    {
        window.Resized -= Resized;
        window.MouseMoved -= MouseMoved;
        window.MouseButtonPressed -= MousePressed;
        window.MouseButtonReleased -= MouseReleased;
        window.MouseWheelScrolled -= MouseWheelScrolled;
    }

    private void Resized(object? sender, SizeEventArgs e)
    {
        camera.Resize((int)e.Width, (int)e.Height);
    }

    private void MouseMoved(object? sender, MouseMoveEventArgs e)
    {
        var position = new Vector2i(e.X, e.Y);
        camera.MouseMoved(position);
    }

    private void MousePressed(object? sender, MouseButtonEventArgs e)
    {
        switch (e.Button)
        {
            case Mouse.Button.Right:
                camera.MoveButtonPressed();
                break;
        }
    }

    private void MouseReleased(object? sender, MouseButtonEventArgs e)
    {
        switch (e.Button)
        {
            case Mouse.Button.Right:
                camera.MoveButtonReleased();
                break;
        }
    }

    private void MouseWheelScrolled(object? sender, MouseWheelScrollEventArgs e)
    {
        camera.Scroll(e.Delta);
    }

    public void Dispose()
    {
        window.Resized -= Resized;
        window.MouseMoved -= MouseMoved;
        window.MouseButtonPressed -= MousePressed;
        window.MouseButtonReleased -= MouseReleased;
    }
}