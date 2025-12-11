using Godot;

/// <summary>
/// 洞府场景
/// </summary>
public partial class DojoScene : Node2D
{
    /// <summary>
    /// 场景准备就绪时调用
    /// </summary>
    public override void _Ready()
    {
        // 连接按钮信号
        GetNode<Button>("MenuContainer/EnterCityButton").Pressed += OnEnterCityButtonPressed;
        GetNode<Button>("MenuContainer/PracticeButton").Pressed += OnPracticeButtonPressed;
        GetNode<Button>("MenuContainer/InventoryButton").Pressed += OnInventoryButtonPressed;
        GetNode<Button>("MenuContainer/SettingsButton").Pressed += OnSettingsButtonPressed;
        
        GD.Print("洞府场景加载完成");
    }
    
    /// <summary>
    /// 处理进入城区按钮点击
    /// </summary>
    private void OnEnterCityButtonPressed()
    {
        GD.Print("点击进入城区");
        
        // 通过游戏管理器切换到城区场景
        if (GameManager.Instance != null && GameManager.Instance.SceneManager != null)
        {
            // 切换到城区场景
            GameManager.Instance.SceneManager.ChangeScene(SceneManager.SceneType.Town);
        }
    }
    
    /// <summary>
    /// 处理修炼按钮点击
    /// </summary>
    private void OnPracticeButtonPressed()
    {
        // TODO: 实现修炼功能
        GD.Print("点击修炼打坐");
        ShowMessage("开始修炼，感悟天地灵气...");
    }
    
    /// <summary>
    /// 处理物品按钮点击
    /// </summary>
    private void OnInventoryButtonPressed()
    {
        // TODO: 实现物品系统
        GD.Print("点击查看物品");
        ShowMessage("物品系统正在开发中...");
    }
    
    /// <summary>
    /// 处理设置按钮点击
    /// </summary>
    private void OnSettingsButtonPressed()
    {
        // TODO: 实现设置系统
        GD.Print("点击设置");
        ShowMessage("设置系统正在开发中...");
    }
    
    /// <summary>
    /// 显示临时消息
    /// </summary>
    private void ShowMessage(string message)
    {
        // 创建临时消息标签
        Label messageLabel = new Label();
        messageLabel.Position = new Vector2(512, 300);
        messageLabel.Size = new Vector2(300, 50);
        messageLabel.Text = message;
        messageLabel.HorizontalAlignment = HorizontalAlignment.Center;
        messageLabel.VerticalAlignment = VerticalAlignment.Center;
        messageLabel.Modulate = new Color(1, 1, 1, 0);
        
        AddChild(messageLabel);
        
        // 创建淡出动画
        Tween tween = GetTree().CreateTween();
        tween.TweenProperty(messageLabel, "modulate:a", 1.0, 0.5);
        tween.TweenInterval(1.5);
        tween.TweenProperty(messageLabel, "modulate:a", 0.0, 0.5);
        tween.Finished += () => messageLabel.QueueFree();
    }
    
    /// <summary>
    /// 处理输入事件
    /// </summary>
    public override void _Input(InputEvent @event)
    {
        // 处理返回/退出输入
        if (@event.IsActionPressed("menu"))
        {
            GD.Print("按下菜单键");
            // 这里可以打开菜单或返回上一级
        }
    }
}
