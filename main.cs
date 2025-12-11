using Godot;
using System;
using System.IO;

/// <summary>
/// 游戏主入口
/// </summary>
public partial class Main : Node
{
    // 帧计数器
    private int _frameCount = 0;
    // 场景状态检查计时器
    private int _statusCheckCount = 0;
    /// <summary>
    /// 准备就绪时调用
    /// </summary>
    public override void _Ready()
    {
        // 初始化日志
        GD.Print("=== 修仙游戏主入口启动 ===");
        GD.Print("当前场景路径: " + GetPath());
        GD.Print("场景节点数量: " + GetChildCount());
        
        // 创建游戏管理器实例
        GD.Print("创建游戏管理器实例");
        GameManager gameManager = new GameManager();
        
        // 将游戏管理器添加到当前场景
        AddChild(gameManager);
        GD.Print("游戏管理器已添加到场景树");
        
        // 延迟一帧以确保游戏管理器完全初始化
        GD.Print("将在延迟一帧后加载第一个场景");
        CallDeferred("LoadFirstScene");
    }
    
    /// <summary>
    /// 加载第一个场景（洞府场景）
    /// </summary>
    private void LoadFirstScene()
    {
        GD.Print("=== LoadFirstScene被调用 ===");
        
        // 检查游戏管理器是否已初始化
        if (GameManager.Instance == null)
        {
            GD.PrintErr("GameManager.Instance为null，游戏管理器初始化失败");
            return;
        }
        
        GD.Print("游戏管理器实例存在");
        
        if (GameManager.Instance.SceneManager == null)
        {
            GD.PrintErr("GameManager.Instance.SceneManager为null，场景管理器未初始化");
            return;
        }
        
        GD.Print("场景管理器实例存在，准备加载场景");
        
        // 检查场景路径映射是否正确
        try
        {
            // 直接输出场景映射字典的内容
            // 注意：这里我们通过打印GameManager实例的引用确保它存在
            GD.Print($"场景管理器引用: {GameManager.Instance.SceneManager.GetHashCode()}");
        }
        catch (Exception ex)
        {
            GD.PrintErr($"访问场景管理器属性时出错: {ex.Message}");
        }
        
        // 切换到洞府场景 - 我们直接使用Home场景类型，因为根据SceneManager代码，Home对应的是DojoScene.tscn
        GD.Print("尝试加载Home场景（洞府场景）");
        GameManager.Instance.ChangeScene(SceneManager.SceneType.Home);
        
        // 添加一个计时器节点，以便在几秒后检查场景加载状态
        Timer checkTimer = new Timer();
        checkTimer.WaitTime = 0.5; // 更快的检查频率
        checkTimer.OneShot = false; // 重复检查
        AddChild(checkTimer);
        checkTimer.Connect("timeout", Callable.From(CheckSceneStatus));
        checkTimer.Start();
        GD.Print("已启动场景状态检查计时器（每0.5秒检查一次）");
    }
    
    /// <summary>
    /// 检查场景加载状态
    /// </summary>
    private void CheckSceneStatus()
    {
        _statusCheckCount++;
        GD.Print($"=== 场景状态检查 #{_statusCheckCount} ===");
        
        // 检查游戏管理器实例
        if (GameManager.Instance == null)
        {
            GD.PrintErr("游戏管理器实例丢失！");
            return;
        }
        
        // 检查场景管理器
        if (GameManager.Instance.SceneManager == null)
        {
            GD.PrintErr("场景管理器实例丢失！");
            return;
        }
        
        // 检查场景节点结构
        CheckNodeStructure();
        
        // 检查摄像机状态
        CheckCameraStatus();
        
        // 检查可见性
        CheckViewportStatus();
        
        GD.Print("=== 场景状态检查完成 ===");
        
        // 限制检查次数，避免无限日志
        if (_statusCheckCount >= 10)
        {
            GD.Print("已完成最大检查次数，停止自动检查");
            // 尝试手动重新加载当前场景进行测试
            if (Input.IsActionJustPressed("ui_cancel"))
            {
                GD.Print("用户请求重新加载当前场景进行测试");
                GameManager.Instance.ChangeScene(GameManager.Instance.SceneManager.CurrentSceneType);
            }
        }
    }
    
    private void CheckNodeStructure()
    {
        GD.Print("--- 节点结构检查 ---");
        // 输出当前场景树的根节点子节点信息
        GD.Print("根节点子节点数量: " + GetTree().Root.GetChildCount());
        
        foreach (Node node in GetTree().Root.GetChildren())
        {
            GD.Print($"根节点子节点: {node.Name} ({node.GetType().Name}) - 路径: {node.GetPath()}");
            
            // 检查子节点的子节点
            if (node.GetChildCount() > 0)
            {
                GD.Print($"  子节点: {node.Name} 包含 {node.GetChildCount()} 个子节点:");
                foreach (Node child in node.GetChildren())
                {
                    GD.Print($"    - {child.Name} ({child.GetType().Name})");
                    
                    // 递归检查一层更深的节点
                    if (child.GetChildCount() > 0)
                    {
                        GD.Print($"      包含 {child.GetChildCount()} 个更深层节点:");
                        foreach (Node grandchild in child.GetChildren())
                        {
                            GD.Print($"        * {grandchild.Name} ({grandchild.GetType().Name})");
                        }
                    }
                }
            }
        }
    }
    
    private void CheckCameraStatus()
    {
        GD.Print("--- 摄像机状态检查 ---");
        // 检查当前活动的摄像机
        var currentCamera = GetViewport().GetCamera2D();
        GD.Print($"当前活动摄像机: {currentCamera?.Name ?? "无活动摄像机"}");
        
        // 查找所有摄像机
        var cameras = GetTree().GetNodesInGroup("camera");
        GD.Print($"场景中有 {cameras.Count} 个摄像机在camera组中");
        
        foreach (Node cameraNode in cameras)
        {
            Camera2D camera = cameraNode as Camera2D;
            if (camera != null)
            {
                // 检查摄像机是否为当前活动摄像机
                bool isCurrent = GetViewport().GetCamera2D() == camera;
                GD.Print($"摄像机: {camera.Name} - 活动状态: {isCurrent} - 可见性: {camera.Visible}");
            }
        }
    }
    
    private void CheckViewportStatus()
    {
        GD.Print("--- 视口状态检查 ---");
        Viewport viewport = GetViewport();
        GD.Print("视口信息已获取");
        
        // 尝试手动创建一个临时摄像机以测试显示
        if (_statusCheckCount == 5) // 在第5次检查时尝试创建备用摄像机
        {
            TryCreateBackupCamera();
        }
    }
    
    private void TryCreateBackupCamera()
    {
        GD.Print("尝试创建备用摄像机进行显示测试");
        
        // 创建一个备用摄像机
        Camera2D backupCamera = new Camera2D();
        backupCamera.Name = "BackupCamera";
        backupCamera.Position = new Vector2(0, 0);
        // 使用MakeCurrent方法而不是Current属性
        backupCamera.MakeCurrent();
        backupCamera.Visible = true;
        AddChild(backupCamera);
        
        GD.Print("备用摄像机已创建并设为活动状态");
    }
    
    /// <summary>
    /// 进程帧更新
    /// </summary>
    public override void _Process(double delta)
    {
        // 添加帧计数，每秒输出一次状态
        _frameCount++;
        if (_frameCount % 60 == 0)
        {
            GD.Print($"帧更新中... 运行时间: {_frameCount/60:F1}秒");
            
            // 按Enter键手动检查场景状态
            if (Input.IsActionJustPressed("ui_accept"))
            {
                GD.Print("用户按Enter键手动触发场景状态检查");
                CheckSceneStatus();
            }
            
            // 按Esc键尝试重新加载场景
            if (Input.IsActionJustPressed("ui_cancel"))
            {
                GD.Print("用户按Esc键尝试重新加载场景");
                _statusCheckCount = 0;
                // 尝试重新加载当前场景
                SceneManager.SceneType currentScene = GameManager.Instance.SceneManager.CurrentSceneType;
                GameManager.Instance.ChangeScene(currentScene);
            }
        }
        
        // 这里可以添加全局的帧更新逻辑
        // 大部分逻辑会在GameManager和各个场景中处理
    }
}
