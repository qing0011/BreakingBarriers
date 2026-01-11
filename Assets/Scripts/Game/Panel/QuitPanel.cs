using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuitPanel : BasePanel
{
    public Button btnQuit;
    public Button btnGoOn;
    public Button btnClose;

    //public override void HideMe()
    //{
    //    base.HideMe();
    //    Time.timeScale = 1;
    //}

    public override void Init()
    {
        btnQuit.onClick.RemoveAllListeners();
        btnQuit.onClick.AddListener(() =>
        {

        });
        btnGoOn.onClick.RemoveAllListeners();
        btnGoOn.onClick.AddListener(() =>
        {

        });
        btnClose.onClick.RemoveAllListeners();
        btnClose.onClick.AddListener(() =>
        {

        });
      
        UIManager.Instance.HidePanel<QuitPanel>();
    }
}
