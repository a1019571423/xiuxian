using Godot;

/// <summary>
/// 项目配置 - 管理游戏的基础配置和渲染参数
/// </summary>
public static class ProjectConfig
{
    /// <summary>
    /// 初始化项目设置
    /// 注意：在实际项目中，这些设置最好通过Godot编辑器的项目设置进行配置
    /// 此方法提供程序化配置作为备用
    /// </summary>
    public static void InitializeProjectSettings()
    {
        // 设置渲染参数
        ConfigureRenderer();
        
        // 设置窗口参数
        ConfigureWindow();
        
        // 设置显示参数
        ConfigureDisplay();
        
        GD.Print("项目基础配置初始化完成");
    }
    
    /// <summary>
    /// 配置渲染器参数
    /// </summary>
    private static void ConfigureRenderer()
    {
        // 设置抗锯齿（如果支持）
        // 注意：这些设置在运行时可能无法修改，取决于平台
        ProjectSettings.SetSetting("rendering/anti_aliasing/quality", 2); // 2x MSAA
        
        // 设置纹理过滤
        ProjectSettings.SetSetting("rendering/textures/default_filter", 2); // 2 = Bilinear
        
        // 设置垂直同步
        ProjectSettings.SetSetting("display/window/vsync/vsync_mode", 1); // 启用垂直同步
    }
    
    /// <summary>
    /// 配置窗口参数
    /// </summary>
    private static void ConfigureWindow()
    {
        // 设置窗口标题
        ProjectSettings.SetSetting("display/window/title", "修仙游戏");
        
        // 设置初始窗口大小
        ProjectSettings.SetSetting("display/window/size/width", 1024);
        ProjectSettings.SetSetting("display/window/size/height", 600);
        
        // 设置窗口最小尺寸
        ProjectSettings.SetSetting("display/window/size/min_width", 800);
        ProjectSettings.SetSetting("display/window/size/min_height", 600);
        
        // 允许窗口调整大小
        ProjectSettings.SetSetting("display/window/size/resizable", true);
    }
    
    /// <summary>
    /// 配置显示参数
    /// </summary>
    private static void ConfigureDisplay()
    {
        // 设置像素比例模式
        ProjectSettings.SetSetting("display/window/stretch/mode", 4); // viewport
        ProjectSettings.SetSetting("display/window/stretch/aspect", 2); // keep_width
        
        // 设置帧率上限
        ProjectSettings.SetSetting("application/run/max_fps", 60);
        
        // 保存设置
        // 注意：在导出的游戏中可能需要添加适当的权限
        ProjectSettings.Save();
    }
    
    /// <summary>
    /// 从GameManager初始化时调用此方法
    /// 将此方法添加到GameManager._Ready()中
    /// </summary>
    public static void InitializeFromGameManager()
    {
        InitializeProjectSettings();
    }
}
