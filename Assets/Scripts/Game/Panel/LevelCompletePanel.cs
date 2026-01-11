using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class LevelCompletePanel : BasePanel
{
   
    public TMP_Text txtTip;
    public Button btnGoOn;

    public override void Init()
    {
        btnGoOn.onClick.RemoveAllListeners();

        btnGoOn.onClick.AddListener(() =>
        {
            // 刷新最高分
            GameDataMgr.Instance.TryRefreshMaxScore(GameDataMgr.Instance.labScore);
            GameLevelMgr.Instance.ContinueNextLevel();
            UIManager.Instance.HidePanel<LevelCompletePanel>();
           
        });

    }

    public void SetText(string msg)
    {
        txtTip.text = "恭喜通关";
    }


}


