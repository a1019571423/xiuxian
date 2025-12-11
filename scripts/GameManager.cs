using Godot;
using System;
using System.IO;

/// <summary>
/// 游戏管理器 - 全局单例，管理游戏的核心组件和状态
/// </summary>
public class GameManager : Node
{
    // 单例实例
    private static GameManager _instance;
    
    // 场景管理器引用
    private SceneManager _sceneManager;
    
    // 输入管理器引用
    private InputManager _inputManager;
    
    // 日志文件路径
    private string _logFilePath = "user://xiuxian_game_log.txt";
    
    // 公共访问属性
    public static GameManager Instance => _instance;
    public SceneManager SceneManager => _sceneManager;

    /// <summary>
    /// 获取输入管理器
    /// </summary>
    public InputManager InputManager => _inputManager;

    private void WriteToLog(string message)
    {
        string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
        string logMessage = $"[{timestamp}] {message}\n";
        
        try
        {
            // 写入日志文件
            File.AppendAllText(_logFilePath, logMessage);
            // 同时输出到控制台
            GD.Print(message);
        }
        catch (Exception ex)
        {
            GD.PrintErr($"写入日志失败: {ex.Message}");
        }
    }
    
    /// <summary>
    /// 初始化方法
    /// </summary>
    public override void _Ready()
    {
        // 设置单例
        if (_instance != null)
        {
            QueueFree();
            return;
        }
        
        _instance = this;
        
        // 清除旧日志
        if (File.Exists(_logFilePath))
        {
            File.Delete(_logFilePath);
        }
        
        WriteToLog("游戏管理器初始化开始");
        
        // 防止在场景切换时被销毁 - 使用Node的TreeExiting信号来处理
        
        // 初始化项目配置
        ProjectConfig.InitializeFromGameManager();
        WriteToLog("项目配置初始化完成");
        
        // 初始化输入映射配置
        InputConfig.InitializeInputMap();
        
        // 创建并添加输入管理器
        _inputManager = new InputManager();
        AddChild(_inputManager);
        WriteToLog("输入管理器已创建并添加到场景树");
        
        // 创建并添加场景管理器
        _sceneManager = new SceneManager();
        AddChild(_sceneManager);
        WriteToLog("场景管理器已创建并添加到场景树");
        
        WriteToLog("游戏管理器初始化完成");
    }

    /// <summary>
    /// 切换场景的便捷方法
    /// </summary>
    /// <param name="sceneType">目标场景类型</param>
    public void ChangeScene(SceneManager.SceneType sceneType)
    {
        WriteToLog($"GameManager: 请求切换场景到 {sceneType}");
        _sceneManager.ChangeScene(sceneType);
    }

    /// <summary>
    /// 节点退出树时清理
    /// </summary>
    public override void _ExitTree()
    {
        WriteToLog("游戏管理器销毁中");
        if (_instance == this)
        {
            _instance = null;
            WriteToLog("游戏管理器单例已清理");
        }
    }
}
