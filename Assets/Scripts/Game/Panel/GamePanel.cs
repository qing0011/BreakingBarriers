using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePanel : BasePanel
{
    public TMP_Text labScore;
    public TMP_Text labTime;
    public Button btnSetting;
    public Button btnReturn;

    public TMP_Text hpText;   // HP 数值文字
    public Image hpFill;      // 血条填充

    ////记录当前分数
    //[HideInInspector]
    //public int nowScore = 0;
    //[HideInInspector]
    //public float nowTime = 0;

   
    public float hpw = 350;
    private int time;

    //protected override void Update()
    //{
    //    base.Update();
    //    hpText.text = GameDataMgr.Instance.playerHP.ToString();
    //    labScore.text = GameDataMgr.Instance.labScore.ToString();
    //    labTime.text = GameDataMgr.Instance.labTime.ToString();
    //}
    //监听事件按钮
    public override void Init()
    {

        btnSetting.onClick.AddListener(() =>
        {
            UIManager.Instance.ShowPanel<SettingPanel>();
            Time.timeScale = 0;
        });
        btnReturn.onClick.AddListener(() =>
        {
            UIManager.Instance.HidePanel<GamePanel>();
            SceneManager.LoadScene("BeginScene");
        });

        //// 初始化UI显示
        //UpdateScoreDisplay();
        //UpdateTimeDisplay();
    }

    //protected override void Update()
    // {
    //     base.Update();
    //     // 通过帧累计间隔时间
    //     nowTime += Time.deltaTime;

    //     // 把秒转换成时分秒
    //     time = (int)nowTime;
    //     UpdateTimeDisplay();
    // }

    // ====== 对外接口 ======

    public void SetScore(int score)
    {
        
        labScore.text = score.ToString();
    }

    public void SetTime(int seconds)
    {
        time = seconds;
        UpdateTimeDisplay();
    }

    // ====== UI 内部 ======
    // 更新时间显示
    private void UpdateTimeDisplay()
    {
        labTime.text = " ";
        if (time / 3600 > 0)
        {
            labTime.text += time / 3600 + "时";
        }
        if (time % 3600 / 60 > 0 || labTime.text != " ")
        {
            labTime.text += time % 3600 / 60 + "分";
        }
        labTime.text += time % 60 + "秒";
    }

    //// 更新分数显示
    //private void UpdateScoreDisplay()
    //{
    //    labScore.text = nowScore.ToString();
    //}

    //// 提供给外部加分方法
    //public void AddScore(int score)
    //{
    //    nowScore += score;
    //    // 更新界面显示
    //    UpdateScoreDisplay();
    //}

    // 更新血条
    public void UpdateHP(int maxHP, int HP)
    {
        // 更新文字
        hpText.text = $"{HP} / {maxHP}";
        float ratio = Mathf.Clamp01((float)HP / maxHP);

        // 更新血条
        hpFill.fillAmount = ratio;
        
       
    }

    //// 重置游戏数据（可选，在需要重新开始游戏时调用）
    //public void ResetGameData()
    //{
    //    nowScore = 0;
    //    nowTime = 0;
    //    UpdateScoreDisplay();
    //    UpdateTimeDisplay();
    //}


   
}