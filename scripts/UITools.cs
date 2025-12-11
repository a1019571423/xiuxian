using Godot;
using System;

public partial class UITools : Node
{
    public static UITools Instance;
    
    public override void _Ready()
    {
        Instance = this;
        // 在Godot 4.x中，SetAsTopLevel方法已被移除
        // 使用Owner = null可以实现类似效果
        Owner = null;
    }
    
    public Label CreateTooltip(string message)
    {
        var tooltip = new Label();
        tooltip.Text = message;
        return tooltip;
    }
}
