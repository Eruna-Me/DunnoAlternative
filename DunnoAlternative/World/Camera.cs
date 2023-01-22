using SFML.Graphics;
using SFML.System;
using DunnoAlternative.Shared;

namespace DunnoAlternative.World;

internal class Camera : IDisposable
{
    private readonly View worldView;
    private View uiView;

    private Vector2f pivotStartView;
    private Vector2f pivotClickStart;
    private bool pivotSet = false;

    private float zoom = 1;
    private Vector2i mousePosition;

    private readonly Vector2i initialSize;

    public Camera(RenderWindow window)
    {
        initialSize = new Vector2i((int)window.Size.X, (int)window.Size.Y);
        worldView = new(new FloatRect(0, 0, window.Size.X, window.Size.Y));
        uiView = new(new FloatRect(0, 0, window.Size.X, window.Size.Y));
    }    

    public Vector2f ScreenToWorld(Vector2i position)
    {
        return ScreenToWorld(position.ToVector2f());
    }

    public Vector2f ScreenToWorld(Vector2f position)
    {
        Vector2f viewPosition = worldView.Center - worldView.Size / 2;
        return viewPosition + position * zoom;
    }

    public Vector2f WorldToScreen(Vector2f position)
    {
        Vector2f viewPosition = worldView.Center - worldView.Size / 2;
        return (position - viewPosition) / zoom;
    }

    public View GetWorldView() => worldView;

    public View GetUiView() => uiView;

    public void Resize(int width, int height)
    {
        worldView.Size = new Vector2f(width, height);
        
        //Use width instead of initalSize.width same for height etc, then you can make the ui scale properly
        uiView = new View(new FloatRect(0, 0, (float)initialSize.X, (float)initialSize.Y)); 
        
        worldView.Zoom(zoom);
    }

    public void MoveButtonPressed()
    {
        pivotClickStart = mousePosition.ToVector2f();
        pivotStartView = worldView.Center;
        pivotSet = true;
    }

    public void MoveButtonReleased() => pivotSet = false;
    
    public void Scroll(float delta)
    {
        float deltaZoom = 1 - delta * 0.1f;
        
        var before = ScreenToWorld(mousePosition);
        var pivotWorld = ScreenToWorld(pivotClickStart);
        
        worldView.Zoom(deltaZoom);
        zoom *= deltaZoom;

        var after = ScreenToWorld(mousePosition);

        pivotClickStart = WorldToScreen(pivotWorld);;
        worldView.Move(before - after);
    }

    public void MouseMoved(Vector2i mouse)
    {
        mousePosition = mouse;

        if (pivotSet)
        {
            var delta = pivotClickStart - mousePosition.ToVector2f();
            worldView.Center = pivotStartView + delta * zoom;
        }
    }

    public void Dispose()
    {
        worldView.Dispose();
        uiView.Dispose();
    }
}
