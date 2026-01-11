using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TipPanel : BasePanel
{
    public Button btnClose;
    

    public override void Init()
    {
        btnClose.onClick.RemoveAllListeners();
        btnClose.onClick.AddListener(() =>
        {
            //返回后必须重置
            GameDataMgr.Instance.ResetGameData();
            UIManager.Instance.HidePanel<TipPanel>();
            Time.timeScale = 0.1f;

            SceneManager.LoadScene("BeginScene");
        });
    }
}
