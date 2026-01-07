using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitPanel : BasePanel
{
    public CustomGUIButton btnQuit;
    public CustomGUIButton btnGoOn;
    public CustomGUIButton btnClose;

    //public override void HideMe()
    //{
    //    base.HideMe();
    //    Time.timeScale = 1;
    //}

    public override void Init()
    {
        btnQuit.clickEvent += () =>
        {
            SceneManager.LoadScene("BeginScene");
        };
        btnGoOn.clickEvent += () =>
        {
            UIManager.Instance.HidePanel<QuitPanel>();
            
        };
        btnClose.clickEvent += () =>
        {
            UIManager.Instance.HidePanel<QuitPanel>();
        };
        UIManager.Instance.HidePanel<QuitPanel>();
    }
}
