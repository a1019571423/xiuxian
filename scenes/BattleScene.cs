using Godot;
using System.Collections.Generic;

/// <summary>
/// 战斗场景
/// </summary>
public partial class BattleScene : Node2D
{
    // 战斗参与者基类
    private class BattleEntity
    {
        public int MaxHealth { get; private set; }
        public int CurrentHealth { get; set; }
        public int Attack { get; private set; }
        public int Defense { get; set; }
        public string Name { get; private set; }
        
        public BattleEntity(string name, int maxHealth, int attack, int defense)
        {
            Name = name;
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
            Attack = attack;
            Defense = defense;
        }
        
        public bool IsAlive() => CurrentHealth > 0;
        
        public void TakeDamage(int damage)
        {
            // 计算实际伤害（考虑防御）
            int actualDamage = Mathf.Max(1, damage - Defense / 10);
            CurrentHealth = Mathf.Max(0, CurrentHealth - actualDamage);
        }
        
        public void Heal(int amount)
        {
            CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + amount);
        }
    }
    
    // 玩家和敌人实例
    private BattleEntity _player;
    private BattleEntity _enemy;
    
    // UI元素引用
    private RichTextLabel _messageLog;
    private ColorRect _playerHealthBar;
    private ColorRect _enemyHealthBar;
    private Label _playerHealthText;
    private Label _enemyHealthText;
    
    // 战斗状态
    private bool _isPlayerTurn = true;
    private bool _isBattleActive = true;
    
    /// <summary>
    /// 场景准备就绪时调用
    /// </summary>
    public override void _Ready()
    {
        // 初始化玩家和敌人
        _player = new BattleEntity("玩家", 100, 20, 50);
        _enemy = new BattleEntity("妖怪", 80, 15, 30);
        
        // 获取UI引用
        _messageLog = GetNode<RichTextLabel>("MessageLog");
        _playerHealthBar = GetNode<ColorRect>("PlayerArea/PlayerHealthBar");
        _enemyHealthBar = GetNode<ColorRect>("EnemyArea/EnemyHealthBar");
        _playerHealthText = GetNode<Label>("PlayerArea/PlayerHealthText");
        _enemyHealthText = GetNode<Label>("EnemyArea/EnemyHealthText");
        
        // 连接按钮信号
        GetNode<Button>("ActionPanel/AttackButton").Pressed += OnAttackButtonPressed;
        GetNode<Button>("ActionPanel/SkillButton").Pressed += OnSkillButtonPressed;
        GetNode<Button>("ActionPanel/DefendButton").Pressed += OnDefendButtonPressed;
        GetNode<Button>("ActionPanel/EscapeButton").Pressed += OnEscapeButtonPressed;
        
        // 更新UI
        UpdateHealthBars();
        
        // 添加初始战斗消息
        AddBattleMessage("战斗开始！一个妖怪出现了！");
        AddBattleMessage("轮到你行动！");
        
        GD.Print("战斗场景加载完成");
    }
    
    /// <summary>
    /// 处理输入事件
    /// </summary>
    public override void _Input(InputEvent @event)
    {
        // 监听菜单按键
        if (@event.IsActionPressed("menu"))
        {
            // 尝试逃跑
            TryEscape();
        }
    }
    
    /// <summary>
    /// 攻击按钮点击事件
    /// </summary>
    private void OnAttackButtonPressed()
    {
        if (_isBattleActive && _isPlayerTurn)
        {
            PlayerAttack();
        }
    }
    
    /// <summary>
    /// 技能按钮点击事件
    /// </summary>
    private void OnSkillButtonPressed()
    {
        if (_isBattleActive && _isPlayerTurn)
        {
            PlayerSkill();
        }
    }
    
    /// <summary>
    /// 防御按钮点击事件
    /// </summary>
    private void OnDefendButtonPressed()
    {
        if (_isBattleActive && _isPlayerTurn)
        {
            PlayerDefend();
        }
    }
    
    /// <summary>
    /// 逃跑按钮点击事件
    /// </summary>
    private void OnEscapeButtonPressed()
    {
        if (_isBattleActive)
        {
            TryEscape();
        }
    }
    
    /// <summary>
    /// 玩家普通攻击
    /// </summary>
    private void PlayerAttack()
    {
        // 计算伤害
        int damage = _player.Attack;
        _enemy.TakeDamage(damage);
        
        AddBattleMessage($"你对{_enemy.Name}造成了{damage}点伤害！");
        
        // 更新UI
        UpdateHealthBars();
        
        // 检查战斗是否结束
        if (!_enemy.IsAlive())
        {
            BattleEnd(true);
            return;
        }
        
        // 切换到敌人回合
        _isPlayerTurn = false;
        GetTree().CreateTimer(1.0f).Timeout += EnemyTurn;
    }
    
    /// <summary>
    /// 玩家使用技能
    /// </summary>
    private void PlayerSkill()
    {
        // 计算技能伤害（比普通攻击高50%）
        int damage = (int)(_player.Attack * 1.5f);
        _enemy.TakeDamage(damage);
        
        AddBattleMessage($"你使用了技能！对{_enemy.Name}造成了{damage}点伤害！");
        
        // 更新UI
        UpdateHealthBars();
        
        // 检查战斗是否结束
        if (!_enemy.IsAlive())
        {
            BattleEnd(true);
            return;
        }
        
        // 切换到敌人回合
        _isPlayerTurn = false;
        GetTree().CreateTimer(1.0f).Timeout += EnemyTurn;
    }
    
    /// <summary>
    /// 玩家防御
    /// </summary>
    private void PlayerDefend()
    {
        // 增加临时防御
        _player.Defense += 20;
        
        AddBattleMessage("你进入了防御姿态，临时增加了防御力！");
        
        // 切换到敌人回合
        _isPlayerTurn = false;
        GetTree().CreateTimer(1.0f).Timeout += EnemyTurn;
    }
    
    /// <summary>
    /// 尝试逃跑
    /// </summary>
    private void TryEscape()
    {
        // 50%概率成功逃跑
        bool success = GD.Randf() > 0.5f;
        
        if (success)
        {
            AddBattleMessage("你成功逃脱了战斗！");
            GetTree().CreateTimer(1.5f).Timeout += ExitBattleScene;
        }
        else
        {
            AddBattleMessage("逃跑失败！敌人抓住了机会攻击你！");
            
            // 逃跑失败后，敌人会反击
            EnemyAttack();
        }
    }
    
    /// <summary>
    /// 敌人回合
    /// </summary>
    private void EnemyTurn()
    {
        if (!_isBattleActive)
            return;
        
        AddBattleMessage($"{_enemy.Name}的回合！");
        
        // 敌人行动选择（简化为总是攻击）
        EnemyAttack();
    }
    
    /// <summary>
    /// 敌人攻击
    /// </summary>
    private void EnemyAttack()
    {
        // 计算伤害
        int damage = _enemy.Attack;
        _player.TakeDamage(damage);
        
        AddBattleMessage($"{_enemy.Name}对你造成了{damage}点伤害！");
        
        // 检查是否有临时防御，如果有则清除
        if (_player.Defense > 50) // 假设基础防御是50
        {
            _player.Defense -= 20;
            AddBattleMessage("你的防御姿态结束了！");
        }
        
        // 更新UI
        UpdateHealthBars();
        
        // 检查战斗是否结束
        if (!_player.IsAlive())
        {
            BattleEnd(false);
            return;
        }
        
        // 切换回玩家回合
        _isPlayerTurn = true;
        AddBattleMessage("轮到你行动！");
    }
    
    /// <summary>
    /// 战斗结束
    /// </summary>
    private void BattleEnd(bool playerWon)
    {
        _isBattleActive = false;
        
        if (playerWon)
        {
            AddBattleMessage($"你击败了{_enemy.Name}！获得了战斗胜利！");
        }
        else
        {
            AddBattleMessage("你被击败了！");
        }
        
        // 延迟返回主场景
        GetTree().CreateTimer(2.0f).Timeout += ExitBattleScene;
    }
    
    /// <summary>
    /// 退出战斗场景
    /// </summary>
    private void ExitBattleScene()
    {
        if (GameManager.Instance != null && GameManager.Instance.SceneManager != null)
        {
            GameManager.Instance.SceneManager.ChangeScene(SceneManager.SceneType.Main);
        }
    }
    
    /// <summary>
    /// 更新血条UI
    /// </summary>
    private void UpdateHealthBars()
    {
        // 更新玩家血条
        float playerHealthPercent = (float)_player.CurrentHealth / _player.MaxHealth;
        _playerHealthBar.Size = new Vector2(100 * playerHealthPercent, 10);
        _playerHealthText.Text = $"{_player.Name}: {_player.CurrentHealth}/{_player.MaxHealth}";
        
        // 更新敌人血条
        float enemyHealthPercent = (float)_enemy.CurrentHealth / _enemy.MaxHealth;
        _enemyHealthBar.Size = new Vector2(100 * enemyHealthPercent, 10);
        _enemyHealthText.Text = $"{_enemy.Name}: {_enemy.CurrentHealth}/{_enemy.MaxHealth}";
    }
    
    /// <summary>
    /// 添加战斗消息
    /// </summary>
    private void AddBattleMessage(string message)
    {
        _messageLog.Text += message + "\n";
        // 使用Godot 4.x兼容的滚动方法
        _messageLog.ScrollToLine(_messageLog.GetLineCount());
    }
}
