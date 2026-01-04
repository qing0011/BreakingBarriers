
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePanel1 : BasePanel<GamePanel>
{
    // 关联场景上的控件
    public CustomGUILabel labScore;
    public CustomGUILabel labTime;
    public CustomGUIButton btnSetting;
    public CustomGUIButton btnQuit;
    public CustomGUITexture texHP;

    // 记录当前分数 - 使用静态变量确保数据持久化
    [HideInInspector]
    public static int nowScore = 0;
    [HideInInspector]
    public static float nowTime = 0;

    public float hpw = 350;
    private int time;

    public object WinPanel { get; internal set; }

    // 添加Awake方法确保对象不随场景切换被销毁
    void Awake()
    {
        // 确保只有一个GamePanel实例存在
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 设置为跨场景不销毁
        DontDestroyOnLoad(gameObject);


    }

    void Start()
    {
        // 监听事件按钮
        btnSetting.clickEvent += () =>
        {
            SettingPanel.Instance.ShowMe();
            Time.timeScale = 0;
        };

        btnQuit.clickEvent += () =>
        {
            QuitPanel.Instance.ShowMe();
            Time.timeScale = 0;
        };
        // 初始化UI显示
        UpdateScoreDisplay();
        UpdateTimeDisplay();
    }

    void Update()
    {
        // 通过帧累计间隔时间
        nowTime += Time.deltaTime;

        // 把秒转换成时分秒
        time = (int)nowTime;
        UpdateTimeDisplay();
    }

    // 更新时间显示
    private void UpdateTimeDisplay()
    {
        labTime.content.text = " ";
        if (time / 3600 > 0)
        {
            labTime.content.text += time / 3600 + "时";
        }
        if (time % 3600 / 60 > 0 || labTime.content.text != " ")
        {
            labTime.content.text += time % 3600 / 60 + "分";
        }
        labTime.content.text += time % 60 + "秒";
    }

    // 更新分数显示
    private void UpdateScoreDisplay()
    {
        labScore.content.text = nowScore.ToString();
    }

    // 提供给外部加分方法
    public void AddScore(int score)
    {
        nowScore += score;
        // 更新界面显示
        UpdateScoreDisplay();
    }

    // 更新血条
    public void UpdateHP(int maxHP, int HP)
    {
        texHP.guiPos.width = (float)HP / maxHP * hpw;
    }

    // 重置游戏数据（可选，在需要重新开始游戏时调用）
    public void ResetGameData()
    {
        nowScore = 0;
        nowTime = 0;
        UpdateScoreDisplay();
        UpdateTimeDisplay();
    }
}