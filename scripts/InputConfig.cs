using Godot;
using System.Collections.Generic;

/// <summary>
/// 输入配置 - 管理游戏的输入映射配置
/// </summary>
public static class InputConfig
{
    /// <summary>
    /// 初始化游戏输入映射
    /// 注意：在实际项目中，输入映射最好通过Godot编辑器的项目设置进行配置
    /// 此方法仅作为备用方案或程序化设置输入映射
    /// </summary>
    public static void InitializeInputMap()
    {
        // 定义输入动作及其对应的按键映射
        var inputActions = new Dictionary<string, List<Key>>()
        {
            { InputManager.MOVE_UP, new List<Key>() { Key.W, Key.Up } },
            { InputManager.MOVE_DOWN, new List<Key>() { Key.S, Key.Down } },
            { InputManager.MOVE_LEFT, new List<Key>() { Key.A, Key.Left } },
            { InputManager.MOVE_RIGHT, new List<Key>() { Key.D, Key.Right } },
            { InputManager.INTERACT, new List<Key>() { Key.E, Key.Enter } },
            { InputManager.MENU, new List<Key>() { Key.Escape, Key.Space } }
        };

        // 配置输入动作映射
        foreach (var action in inputActions)
        {
            // 检查动作是否已存在，如果不存在则创建
            if (!InputMap.HasAction(action.Key))
            {
                InputMap.AddAction(action.Key);
            }

            // 为每个动作添加对应的按键映射
            foreach (var key in action.Value)
            {
                // 创建键盘输入事件
                var keyEvent = new InputEventKey();
                keyEvent.KeyLabel = key;
                keyEvent.Pressed = true;
                
                // 添加按键映射到动作
                InputMap.ActionAddEvent(action.Key, keyEvent);
            }
        }

        // 添加鼠标/触摸点击作为交互动作
        var mouseEvent = new InputEventMouseButton();
        mouseEvent.ButtonIndex = MouseButton.Left;
        mouseEvent.Pressed = true;
        InputMap.ActionAddEvent(InputManager.INTERACT, mouseEvent);

        // 添加触摸屏点击作为交互动作
        var touchEvent = new InputEventScreenTouch();
        touchEvent.Index = 0;
        touchEvent.Pressed = true;
        InputMap.ActionAddEvent(InputManager.INTERACT, touchEvent);

        GD.Print("输入映射配置完成");
    }

    /// <summary>
    /// 在GameManager中注册输入管理器
    /// </summary>
    public static void RegisterInputManager()
    {
        // 创建并添加输入管理器到游戏管理器
        var inputManager = InputManager.CreateAndAddToGameManager();
        
        if (inputManager != null)
        {
            GD.Print("输入管理器已注册到游戏管理器");
        }
        else
        {
            GD.PrintErr("无法注册输入管理器，游戏管理器未初始化");
        }
    }
}
