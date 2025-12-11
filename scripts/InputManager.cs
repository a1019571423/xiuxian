using Godot;

/// <summary>
/// 输入管理器 - 统一处理游戏中的输入系统
/// </summary>
public class InputManager : Node
{
    // 输入动作名称常量
    public const string MOVE_UP = "move_up";
    public const string MOVE_DOWN = "move_down";
    public const string MOVE_LEFT = "move_left";
    public const string MOVE_RIGHT = "move_right";
    public const string INTERACT = "interact";
    public const string MENU = "menu";
    
    // 输入方向向量
    private Vector2 _inputDirection = Vector2.Zero;
    
    // 是否正在拖拽
    private bool _isDragging = false;
    
    // 上次触摸位置
    private Vector2 _lastTouchPosition = Vector2.Zero;
    
    // 拖拽灵敏度阈值
    private const float DRAG_THRESHOLD = 20.0f;

    /// <summary>
    /// 获取当前输入方向向量
    /// </summary>
    public Vector2 InputDirection => _inputDirection;

    /// <summary>
    /// 输入方向更新信号
    /// </summary>
    [Signal]
    public delegate void DirectionChanged(Vector2 newDirection);

    /// <summary>
    /// 交互按钮按下信号
    /// </summary>
    [Signal]
    public delegate void InteractPressed();

    /// <summary>
    /// 菜单按钮按下信号
    /// </summary>
    [Signal]
    public delegate void MenuPressed();

    /// <summary>
    /// 触摸开始信号
    /// </summary>
    [Signal]
    public delegate void TouchStarted(Vector2 position);

    /// <summary>
    /// 触摸结束信号
    /// </summary>
    [Signal]
    public delegate void TouchEnded(Vector2 position);

    /// <summary>
    /// 触摸移动信号
    /// </summary>
    [Signal]
    public delegate void TouchMoved(Vector2 position);

    /// <summary>
    /// 处理输入事件
    /// </summary>
    public override void _Input(InputEvent @event)
    {
        // 处理触屏输入
        HandleTouchInput(@event);
        
        // 处理键盘输入
        HandleKeyboardInput(@event);
        
        // 处理交互和菜单输入
        HandleActionInput(@event);
    }

    /// <summary>
    /// 处理触屏输入
    /// </summary>
    private void HandleTouchInput(InputEvent @event)
    {
        // 触摸开始
        if (@event is InputEventScreenTouch touchEvent && touchEvent.Pressed)
        {
            _isDragging = true;
            _lastTouchPosition = touchEvent.Position;
            EmitSignal(nameof(TouchStarted), touchEvent.Position);
        }
        // 触摸结束
        else if (@event is InputEventScreenTouch releaseEvent && !releaseEvent.Pressed)
        {
            _isDragging = false;
            ResetInputDirection();
            EmitSignal(nameof(TouchEnded), releaseEvent.Position);
        }
        // 触摸移动
        else if (@event is InputEventScreenDrag dragEvent && _isDragging)
        {
            UpdateTouchDirection(dragEvent.Position);
            EmitSignal(nameof(TouchMoved), dragEvent.Position);
        }
    }

    /// <summary>
    /// 更新触摸输入方向
    /// </summary>
    private void UpdateTouchDirection(Vector2 touchPosition)
    {
        Vector2 delta = touchPosition - _lastTouchPosition;
        
        // 只有当移动距离超过阈值时才更新方向
        if (delta.Length() > DRAG_THRESHOLD)
        {
            // 重置输入方向
            _inputDirection = Vector2.Zero;
            
            // 根据拖拽方向设置输入方向
            float angle = delta.Angle();
            float absAngle = Mathf.Abs(angle);
            
            // 计算哪个方向的分量更大
            if (absAngle < Mathf.Pi / 4 || absAngle > Mathf.Pi * 3 / 4)
            {
                // 水平方向
                _inputDirection.X = Mathf.Sign(delta.X);
            }
            else
            {
                // 垂直方向
                _inputDirection.Y = -Mathf.Sign(delta.Y); // Godot的Y轴向下，这里反转以符合直觉
            }
            
            // 发出方向改变信号
            EmitSignal(nameof(DirectionChanged), _inputDirection);
            
            // 更新上一次触摸位置
            _lastTouchPosition = touchPosition;
        }
    }

    /// <summary>
    /// 处理键盘输入
    /// </summary>
    private void HandleKeyboardInput(InputEvent @event)
    {
        // 检查是否是键盘事件
        if (@event is InputEventKey keyEvent)
        {
            // 只有在按键状态变化时才检查
            if (keyEvent.Pressed || !keyEvent.Echo)
            {
                // 检查WASD和方向键输入
                UpdateKeyboardDirection();
            }
        }
    }

    /// <summary>
    /// 更新键盘输入方向
    /// </summary>
    private void UpdateKeyboardDirection()
    {
        // 重置输入方向
        _inputDirection = Vector2.Zero;
        
        // 检查上下左右输入
        if (Input.IsActionPressed(MOVE_UP))
        {
            _inputDirection.Y -= 1;
        }
        
        if (Input.IsActionPressed(MOVE_DOWN))
        {
            _inputDirection.Y += 1;
        }
        
        if (Input.IsActionPressed(MOVE_LEFT))
        {
            _inputDirection.X -= 1;
        }
        
        if (Input.IsActionPressed(MOVE_RIGHT))
        {
            _inputDirection.X += 1;
        }
        
        // 归一化向量，确保对角线移动速度不会变快
        if (_inputDirection.LengthSquared() > 0)
        {
            _inputDirection = _inputDirection.Normalized();
        }
        
        // 发出方向改变信号
        EmitSignal(nameof(DirectionChanged), _inputDirection);
    }

    /// <summary>
    /// 处理动作输入（交互、菜单等）
    /// </summary>
    private void HandleActionInput(InputEvent @event)
    {
        // 检查交互按键
        if (@event.IsActionPressed(INTERACT))
        {
            EmitSignal(nameof(InteractPressed));
        }
        
        // 检查菜单按键
        if (@event.IsActionPressed(MENU))
        {
            EmitSignal(nameof(MenuPressed));
        }
    }

    /// <summary>
    /// 重置输入方向
    /// </summary>
    private void ResetInputDirection()
    {
        _inputDirection = Vector2.Zero;
        EmitSignal(nameof(DirectionChanged), _inputDirection);
    }

    /// <summary>
    /// 将输入管理器添加到游戏管理器中
    /// </summary>
    public static InputManager CreateAndAddToGameManager()
    {
        if (GameManager.Instance != null)
        {
            InputManager inputManager = new InputManager();
            GameManager.Instance.AddChild(inputManager);
            return inputManager;
        }
        return null;
    }
}
