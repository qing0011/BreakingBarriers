using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignInPanel : BasePanel
{

    // 最大签到天数（一周7天）
    private const int MaxSignInCount = 7;

    // 上次签到日期
    private DateTime _lastDay;
    // 当前签到次数
    private int _signInCount;

    // 是否已达到最大签到次数
    private bool _isMaxSignInCount;
    // 是否显示下次签到倒计时
    private bool _showNextSignInTime;

    // UI元素
    public Toggle[] _SignInToggleTips;          // 签到切换按钮数组
    public Button _signInBtn;                    // 签到按钮
    public TextMeshProUGUI _SignInBtnContent;    // 签到按钮文本
    public Button _CloseBtn;                     // 关闭按钮

    /// <summary>
    /// 模拟按钮，正式发布时需要注销/移除
    /// </summary>

    // 测试时临时跳过"按周重置"逻辑的标志
    private bool _ignoreWeekResetForTest = false;

    [Header("测试用")]
    // 清除重置按钮（测试用）
    public Button _TestClearBtn;
    // 模拟下一天按钮（测试用）
    public Button _TestNextDayBtn;



    /// 初始化面板

    public override void Init()
    {
        // 设置签到按钮点击事件
        _signInBtn.onClick.RemoveAllListeners();
        _signInBtn.onClick.AddListener(OnSignInBtnClick);
        // 设置关闭按钮点击事件
        _CloseBtn.onClick.RemoveAllListeners();
        _CloseBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.HidePanel<SignInPanel>();
        });
        //模拟按钮

        // 设置测试按钮（仅在编辑器或测试时使用）
        if (_TestClearBtn != null)
        {
            _TestClearBtn.gameObject.SetActive(true);
            _TestClearBtn.onClick.AddListener(ClearSignInForTest);
        }

        if (_TestNextDayBtn != null)
        {
            _TestNextDayBtn.gameObject.SetActive(true);
            _TestNextDayBtn.onClick.RemoveAllListeners();
            _TestNextDayBtn.onClick.AddListener(SimulateNextDay);
        }


    }
    /// <summary>
    /// 模拟按钮，输出需要注销
    /// </summary>
    private void ClearSignInForTest()
    {
        Debug.Log("【测试】重置签到");
        // 重置签到数据
        GameDataMgr.Instance.signInSaveData.signInCount = 0;
        GameDataMgr.Instance.signInSaveData.lastSignInTime = "";
        // 保存数据
        GameDataMgr.Instance.SaveSignInData();
        // 重新加载数据并更新UI
        LoadData();
        _isMaxSignInCount = false;
        _showNextSignInTime = false;

        _signInBtn.interactable = true;
        _SignInBtnContent.text = "签到";

        UpdateUI();
    }
    /// <summary>
    /// 测试方法：模拟到下一天
    /// </summary>
    private void SimulateNextDay()
    {

        Debug.Log("【测试】模拟下一天");
        // 跳过周重置检查（测试专用）
        _ignoreWeekResetForTest = true; // 关键：跳过周重置

        SignInSaveData save = GameDataMgr.Instance.signInSaveData;
        // 修改最后签到时间，模拟时间流逝
        if (!string.IsNullOrEmpty(save.lastSignInTime))
        {
            DateTime last = DateTime.Parse(save.lastSignInTime);
            save.lastSignInTime = last.AddDays(-1).ToString();
        }
        else
        {
            save.lastSignInTime = DateTime.Now.AddDays(-1).ToString(); // 减少一天，使得今天可以再次签到
        }
        // 保存并更新UI
        GameDataMgr.Instance.SaveSignInData();

        LoadData();
        _isMaxSignInCount = _signInCount >= MaxSignInCount;
        _showNextSignInTime = false;

        _signInBtn.interactable = true;
        _SignInBtnContent.text = "签到";

        UpdateUI();

    }



    /// <summary>
    /// 当面板激活时调用
    /// </summary>
    private void OnEnable()
    {
        // 加载数据
        LoadData();
        // 检查是否已满签
        _isMaxSignInCount = _signInCount >= MaxSignInCount;
        // 更新UI
        UpdateUI();
        // 根据是否可以签到设置按钮状态
        if (CanSignToday())
        {
            _signInBtn.interactable = true;
            _SignInBtnContent.text = "签到";
            _showNextSignInTime = false;
        }
        else
        {
            _signInBtn.interactable = false;
            _showNextSignInTime = true;
        }
    }
    /// <summary>
    /// 每帧更新（用于倒计时显示）
    /// </summary>
    protected override void Update()
    {
        base.Update();
        // 如果已满签，不需要更新倒计时
        if (_isMaxSignInCount) return;
        // 显示下次签到倒计时
        if (_showNextSignInTime)
        {
            // 计算到明天0点的时间间隔
            TimeSpan interval = DateTime.Now.Date.AddDays(1) - DateTime.Now;
            // 格式化为HH:MM:SS
            if (interval > TimeSpan.Zero)
            {
                _SignInBtnContent.text =
                    $"{interval.Hours:D2}:{interval.Minutes:D2}:{interval.Seconds:D2}";
            }
            else
            {
                // 时间到，可以签到了
                _showNextSignInTime = false;
                _signInBtn.interactable = true;
                _SignInBtnContent.text = "签到";
            }
        }
    }

    // ================= 数据 =================
    /// <summary>
    /// 加载签到数据
    /// </summary>
    private void LoadData()
    {
        SignInSaveData save = GameDataMgr.Instance.signInSaveData;
        // 获取签到次数
        _signInCount = save.signInCount;
        // 获取最后签到时间
        if (string.IsNullOrEmpty(save.lastSignInTime))
        {
            _lastDay = DateTime.MinValue;// 从未签到
        }
        else
        {
            _lastDay = DateTime.Parse(save.lastSignInTime);
        }
        // 检查是否需要按周重置（除非测试中跳过）
        if (!_ignoreWeekResetForTest && !IsSameWeek(DateTime.Now, _lastDay))
           // if (!IsSameWeek(DateTime.Now, _lastDay))
        {
            ResetSignInData();
        }
    }
    /// <summary>
    /// 重置签到数据（每周重置）
    /// </summary>
    private void ResetSignInData()
    {
        // 重置本地变量
        _signInCount = 0;
        _lastDay = DateTime.MinValue;
        // 重置保存的数据
        SignInSaveData save = GameDataMgr.Instance.signInSaveData;
        save.signInCount = 0;
        save.lastSignInTime = "";
        // 保存到持久化存储
        GameDataMgr.Instance.SaveSignInData();
    }

    // ================= 签到 =================

    /// <summary>
    /// 签到按钮点击事件处理
    /// </summary>
    private void OnSignInBtnClick()
    {
        // 检查是否可以签到
        if (!CanSignToday() || _isMaxSignInCount)
            return;

        _signInCount++;
        _lastDay = DateTime.Now;
        // 更新签到数据
        SignInSaveData save = GameDataMgr.Instance.signInSaveData;
        save.signInCount = _signInCount;
        save.lastSignInTime = _lastDay.ToString();
        // 保存数据
        GameDataMgr.Instance.SaveSignInData();
        // 检查是否已满签
        _isMaxSignInCount = _signInCount >= MaxSignInCount;
        // 更新UI
        UpdateUI();
        // 如果不是满签，设置倒计时
        if (!_isMaxSignInCount)
        {
            _signInBtn.interactable = false;
            _showNextSignInTime = true;
        }
    }

    // ================= UI =================
    /// <summary>
    /// 更新UI显示
    /// </summary>
    private void UpdateUI()
    {
        bool canSignToday = CanSignToday();
        // 遍历所有签到切换按钮
        for (int i = 0; i < _SignInToggleTips.Length; i++)
        {
            // 是否已经签到
            bool signed = i < _signInCount;

            // 显示勾选状态
            _SignInToggleTips[i].isOn = signed;

            // 交互控制逻辑 // 设置交互状态
            if (signed)
            {
                // 已签到的不可点
                _SignInToggleTips[i].interactable = false;
            }
            else
            {
                // 只有“当前签到位” + 今天可签到 + 未满签 才能点
                // 只有满足以下条件才可交互：
                // 1. 今天可以签到
                // 2. 未达到最大签到次数
                // 3. 是当前应该签到的位置（第i个）
                _SignInToggleTips[i].interactable =
                    canSignToday &&
                    !_isMaxSignInCount &&
                    i == _signInCount;
            }
        }
    }
    /// <summary>
    /// 检查今天是否可以签到
    /// </summary>
    /// <returns>是否可以签到</returns>
    private bool CanSignToday()
    {
        // 从未签到过
        if (_lastDay == DateTime.MinValue)
            return true;
        // 已达到最大签到次数
        if (_isMaxSignInCount)
            return false;
        // 检查今天是否已经签到过（比较日期部分）
        return DateTime.Now.Date > _lastDay.Date;
    }
    /// <summary>
    /// 检查两个日期是否在同一周
    /// </summary>
    /// <param name="now">当前时间</param>
    /// <param name="last">上次签到时间</param>
    /// <returns>是否在同一周</returns>
    private bool IsSameWeek(DateTime now, DateTime last)
    {
        // 如果从未签到，视为同一周（方便第一次签到）
        if (last == DateTime.MinValue) return true;
        // 计算当前日期所在周的周一
        DateTime mondayNow =
            now.AddDays(1 - ((int)now.DayOfWeek == 0 ? 7 : (int)now.DayOfWeek)).Date;
        // 计算上次签到日期所在周的周一
        DateTime mondayLast =
            last.AddDays(1 - ((int)last.DayOfWeek == 0 ? 7 : (int)last.DayOfWeek)).Date;
        // 比较是否为同一周
        return mondayNow == mondayLast;
    }

}
