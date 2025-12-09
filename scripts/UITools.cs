using Godot;
using System;

public partial class UITools : Node
{
    public static UITools Instance;
    
    public override void _Ready()
    {
        Instance = this;
        SetAsTopLevel(true);
    }
    
    public Label CreateTooltip(string message)
    {
        var tooltip = new Label();
        tooltip.Text = message;
        return tooltip;
    }
}
