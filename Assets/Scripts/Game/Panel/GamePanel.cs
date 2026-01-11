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

   


    public float hpw = 350;
    private int time;


    //监听事件按钮
    public override void Init()
    {
      btnSetting.onClick.RemoveAllListeners();
        btnSetting.onClick.AddListener(() =>
        {

            UIManager.Instance.ShowPanel<SettingPanel>();
            //Time.timeScale = 0.1f;
        });

        btnReturn.onClick.RemoveAllListeners();
        btnReturn.onClick.AddListener(() =>
        {

            //返回后必须重置
            GameDataMgr.Instance.ResetGameData();
            UIManager.Instance.HidePanel<GamePanel>();
            Time.timeScale = 0.1f;

            SceneManager.LoadScene("BeginScene");
            
        });

       
    }




    // ====== 对外接口 ======

    public void SetScore(int score)
    {
        
        labScore.text = score.ToString();
        GameDataMgr.Instance.labScore = score;
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
            labTime.text += time / 3600 + "H";
        }
        if (time % 3600 / 60 > 0 || labTime.text != " ")
        {
            labTime.text += time % 3600 / 60 + "M";
        }
        labTime.text += time % 60 + "S";
    }



    // 更新血条
    public void UpdateHP(int maxHP, int HP)
    {
        // 更新文字
        hpText.text = $"{HP} / {maxHP}";
        float ratio = Mathf.Clamp01((float)HP / maxHP);

        // 更新血条
        hpFill.fillAmount = ratio;


    }

}