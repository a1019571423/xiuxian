using Godot;
using System;

public partial class FormationEditor : Node2D
{
    public override void _Ready()
    {
        GD.Print("FormationEditor initialized");
        AddBasicButtons();
    }
    
    private void AddBasicButtons()
    {
        // 添加圆形按钮
        var circleButton = new Button();
        circleButton.Text = "添加圆形";
        circleButton.Position = new Vector2(100, 50);
        circleButton.Pressed += OnAddCirclePressed;
        AddChild(circleButton);
        
        // 添加三角形按钮
        var triangleButton = new Button();
        triangleButton.Text = "添加三角形";
        triangleButton.Position = new Vector2(100, 100);
        triangleButton.Pressed += OnAddTrianglePressed;
        AddChild(triangleButton);
        
        // 添加清空按钮
        var clearButton = new Button();
        clearButton.Text = "清空画布";
        clearButton.Position = new Vector2(100, 150);
        clearButton.Pressed += OnClearCanvasPressed;
        AddChild(clearButton);
    }
    
    private void OnAddCirclePressed()
    {
        var circle = new ColorRect();
        circle.Position = new Vector2(300, 200);
        circle.Size = new Vector2(100, 100);
        circle.Color = Colors.Blue;
        circle.Name = "CircleShape";
        circle.InputPickable = true;
        
        var style = new StyleBoxFlat();
        style.BgColor = Colors.Blue;
        style.BorderRadiusTopLeft = 50;
        style.BorderRadiusTopRight = 50;
        style.BorderRadiusBottomLeft = 50;
        style.BorderRadiusBottomRight = 50;
        circle.AddThemeStyleboxOverride("normal", style);
        
        circle.GuiInput += (e) => HandleShapeDrag(e, circle);
        AddChild(circle);
    }
    
    private void OnAddTrianglePressed()
    {
        var triangle = new TextureRect();
        triangle.Position = new Vector2(300, 200);
        triangle.Size = new Vector2(100, 100);
        triangle.Name = "TriangleShape";
        triangle.InputPickable = true;
        
        // 创建简单的三角形图像
        var image = Image.Create(100, 100, false, Image.Format.Rgba8);
        image.Fill(Colors.Transparent);
        
        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < 100; j++)
            {
                if (IsPointInTriangle(new Vector2(i, j), 100))
                {
                    image.SetPixel(i, j, Colors.Orange);
                }
            }
        }
        
        triangle.Texture = ImageTexture.CreateFromImage(image);
        triangle.GuiInput += (e) => HandleShapeDrag(e, triangle);
        AddChild(triangle);
    }
    
    private bool IsPointInTriangle(Vector2 p, float size)
    {
        Vector2 a = new Vector2(size/2, 0);
        Vector2 b = new Vector2(0, size);
        Vector2 c = new Vector2(size, size);
        
        float s = a.Y * c.X - a.X * c.Y + (c.Y - a.Y) * p.X + (a.X - c.X) * p.Y;
        float t = a.X * b.Y - a.Y * b.X + (a.Y - b.Y) * p.X + (b.X - a.X) * p.Y;
        
        if ((s < 0) != (t < 0))
            return false;
        
        float A = -b.Y * c.X + a.Y * (c.X - b.X) + a.X * (b.Y - c.Y) + b.X * c.Y;
        
        return Math.Abs(A) < 1e-9 || (s + t) <= A;
    }
    
    private void HandleShapeDrag(InputEvent @event, Control control)
    {
        if (@event is InputEventMouseButton mb)
        {
            if (mb.ButtonIndex == MouseButton.Left && mb.Pressed)
            {
                control.Set("is_dragging", true);
                control.Set("drag_offset", mb.Position);
            }
            else if (mb.ButtonIndex == MouseButton.Left && !mb.Pressed)
            {
                control.Set("is_dragging", false);
            }
        }
        else if (@event is InputEventMouseMotion && (bool)control.Get("is_dragging"))
        {
            control.Position = GetLocalMousePosition() - (Vector2)control.Get("drag_offset");
        }
    }
    
    private void OnClearCanvasPressed()
    {
        foreach (var child in GetChildren())
        {
            var control = child as Control;
            if (control != null && 
                (control.Name == "CircleShape" || control.Name == "TriangleShape"))
            {
                control.QueueFree();
            }
        }
    }
}
