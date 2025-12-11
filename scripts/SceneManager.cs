using Godot;
using System.Collections.Generic;

/// <summary>
/// 场景管理器 - 负责处理游戏场景的加载和切换
/// </summary>
public class SceneManager : Node
{
    // 场景类型枚举
    public enum SceneType
    {
        Home,       // 洞府主场景
        Town,       // 城区场景
        Battle,     // 战斗场景
        Main        // 主场景（与Home相同，兼容不同调用）
    }

    // 场景路径映射字典
    private Dictionary<SceneType, string> _scenePaths = new Dictionary<SceneType, string>()
    {
        { SceneType.Home, "res://scenes/DojoScene.tscn" },
        { SceneType.Town, "res://scenes/CityScene.tscn" },
        { SceneType.Battle, "res://scenes/BattleScene.tscn" },
        { SceneType.Main, "res://scenes/DojoScene.tscn" }  // Main与Home使用相同场景文件
    };

    // 当前场景引用
    private Node _currentScene;
    
    // 当前场景类型
    private SceneType _currentSceneType;

    /// <summary>
    /// 获取当前场景类型
    /// </summary>
    public SceneType CurrentSceneType => _currentSceneType;

    /// <summary>
    /// 场景切换信号
    /// </summary>
    [Signal]
    public delegate void SceneChanged(SceneType newSceneType);

    /// <summary>
    /// 初始化方法
    /// </summary>
    public override void _Ready()
    {
        GD.Print("场景管理器初始化开始");
        
        // 预加载所有场景
        GD.Print("预加载场景资源");
        PreloadScenes();
        
        // 只在没有外部调用的情况下默认加载场景
        if (_currentScene == null)
        {
            GD.Print("SceneManager._Ready: 首次加载洞府场景");
            ChangeScene(SceneType.Home);
        }
        
        GD.Print("SceneManager初始化完成");
    }

    /// <summary>
    /// 预加载场景资源
    /// </summary>
    private void PreloadScenes()
    {
        GD.Print("场景预加载方法被调用");
        foreach (var path in _scenePaths.Values)
        {
            GD.Print($"尝试预加载场景: {path}");
            ResourceLoader.LoadThreadedRequest(path);
        }
        GD.Print("场景预加载完成");
    }

    /// <summary>
    /// 切换场景
    /// </summary>
    /// <param name="sceneType">目标场景类型</param>
    public void ChangeScene(SceneType sceneType)
    {
        GD.Print($"SceneManager: 请求切换到场景类型 {sceneType}");
        GD.Print($"目标场景路径: {_scenePaths[sceneType]}");
        
        // 防止重复加载相同场景
        if (_currentSceneType == sceneType && _currentScene != null)
        {
            GD.Print($"SceneManager.ChangeScene: 场景 {sceneType} 已经是当前场景，跳过加载");
            return;
        }

        // 异步加载场景，将枚举转换为int以兼容Godot的Variant类型系统
        GD.Print("SceneManager: 调用延迟方法 ChangeSceneDeferred");
        CallDeferred(nameof(ChangeSceneDeferred), (int)sceneType);
    }

    /// <summary>
    /// 延迟执行场景切换（在主线程中执行）
    /// </summary>
    private void ChangeSceneDeferred(int sceneTypeInt)
    {
        // 将int转换回枚举类型
        SceneType sceneType = (SceneType)sceneTypeInt;
        
        GD.Print($"=== 开始延迟切换到场景: {sceneType} ===");
        
        // 如果已有场景，则卸载
        if (_currentScene != null)
        {
            GD.Print("卸载当前场景");
            _currentScene.QueueFree();
            _currentScene = null;
        }

        // 加载新场景
        var scenePath = _scenePaths[sceneType];
        GD.Print($"加载场景路径: {scenePath}");
        
        var packedScene = ResourceLoader.Load<PackedScene>(scenePath);
        
        if (packedScene == null)
        {
            GD.PrintErr($"无法加载场景: {scenePath}");
            return;
        }

        // 实例化场景
        GD.Print("实例化新场景节点");
        _currentScene = packedScene.Instantiate();
        
        if (_currentScene == null)
        {
            GD.PrintErr($"无法实例化场景: {scenePath}");
            return;
        }
        
        // 添加到场景树
        AddChild(_currentScene);
        GD.Print($"场景已添加到场景树: {_currentScene.Name}");
        
        // 尝试找到并激活场景中的Camera2D节点
        GD.Print("开始查找并激活摄像机");
        // 首先尝试直接路径
        Camera2D camera = _currentScene.GetNodeOrNull<Camera2D>("Camera2D");
        
        // 如果没找到，尝试带场景名称的路径（从BattleScene.tscn的变更看，节点路径可能已改变）
        if (camera == null)
        {
            string sceneName = _currentScene.Name;
            camera = _currentScene.GetNodeOrNull<Camera2D>($"{sceneName}#Camera2D");
            if (camera != null)
            {
                GD.Print($"找到带场景名称的Camera2D节点: {sceneName}#Camera2D");
            }
            else
            {
                // 尝试在整个场景树中查找Camera2D节点
                foreach (Node child in _currentScene.GetChildren())
                {
                    Camera2D childCamera = child as Camera2D;
                    if (childCamera != null)
                    {
                        camera = childCamera;
                        GD.Print($"找到子节点中的Camera2D: {child.Name}");
                        break;
                    }
                }
            }
        }
        
        if (camera != null)
        {
            camera.MakeCurrent();
            GD.Print("已激活场景中的Camera2D节点");
            // 确保摄像机可见性
            camera.Visible = true;
        }
        else
        {
            GD.Print("场景中未找到任何Camera2D节点，这可能导致场景不可见");
            // 创建默认Camera2D节点作为备份
            Camera2D backupCamera = new Camera2D();
            backupCamera.Name = "BackupCamera2D";
            backupCamera.Position = new Vector2(360, 640);
            backupCamera.MakeCurrent();
            _currentScene.AddChild(backupCamera);
            GD.Print("已创建并激活备用Camera2D节点");
        }
        
        // 输出场景节点结构信息，用于调试
        PrintSceneNodeStructure(_currentScene, 0);
        
        // 更新当前场景类型
        _currentSceneType = sceneType;
        GD.Print($"更新当前场景类型为: {sceneType}");
        
        // 发送场景变更信号，将枚举转换为int以兼容Godot的Variant类型系统
        GD.Print("发送场景变更信号");
        EmitSignal(nameof(SceneChanged), (int)_currentSceneType);
        
        GD.Print($"=== 场景已成功切换至: {sceneType} ===");
    }
    
    /// <summary>
    /// 打印场景节点结构，用于调试
    /// </summary>
    private void PrintSceneNodeStructure(Node node, int depth)
    {
        string indent = new string(' ', depth * 2);
        GD.Print($"{indent}{node.GetPath()}: {node.GetType().Name}");
        
        // 递归打印子节点
        foreach (Node child in node.GetChildren())
        {
            PrintSceneNodeStructure(child, depth + 1);
        }
    }
}
