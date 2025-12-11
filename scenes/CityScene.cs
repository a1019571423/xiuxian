using Godot;

/// <summary>
/// 城区场景（简化版本）
/// </summary>
public partial class CityScene : Node2D
{
    /// <summary>
    /// 场景准备就绪时调用
    /// </summary>
    public override void _Ready()
    {
        GD.Print("城区场景加载完成");
    }
    
    /// <summary>
    /// 处理输入事件
    /// </summary>
    public override void _Input(InputEvent @event)
    {
        // 监听菜单按键
        if (@event.IsActionPressed("menu"))
        {
            // 尝试返回主场景
            if (GameManager.Instance != null && GameManager.Instance.SceneManager != null)
            {
                GameManager.Instance.SceneManager.ChangeScene(SceneManager.SceneType.Main);
            }
        }
    }
}
