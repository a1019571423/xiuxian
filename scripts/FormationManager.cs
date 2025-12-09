using Godot;
using System;
using System.Collections.Generic;

public partial class FormationManager : Node
{
    private List<Node2D> shapes = new List<Node2D>();
    
    public override void _Ready()
    {
        GD.Print("FormationManager initialized");
    }
    
    public void AddShape(Node2D shape)
    {
        shapes.Add(shape);
    }
    
    public void RemoveShape(Node2D shape)
    {
        shapes.Remove(shape);
    }
}
