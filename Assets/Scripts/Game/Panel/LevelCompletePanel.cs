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
        btnGoOn.onClick.AddListener(() =>
        {
      
            GameLevelMgr.Instance.ContinueNextLevel();
            UIManager.Instance.HidePanel<LevelCompletePanel>();
            Destroy(gameObject);
        });
    }

    public void SetText(string msg)
    {
        txtTip.text = msg;
    }


}


