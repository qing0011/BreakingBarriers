using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BeginPanel : BasePanel<BeginPanel>
{
    //声明变量
    public CustomGUIButton btnBegin;
    public CustomGUIButton btnSetting;
    public CustomGUIButton btnQuit;
    public CustomGUIButton btnRank;
    // Start is called before the first frame update
    void Start()
    {
        //在Game试图锁定鼠标
        Cursor.lockState = CursorLockMode.Confined;


        //启动开始面板事件
        btnBegin.clickEvent += () =>
        {
            SceneManager.LoadScene("GameScene");
        };
        //开启设置面板事件
        btnSetting.clickEvent += () =>
        {
            SettingPanel.Instance.ShowMe();
            //避免穿透 隐藏自己
            HideMe();
        };
        //退出按钮事件
        btnQuit.clickEvent += () =>
        {
            Application.Quit();
        };
        //排行榜按钮事件
        btnRank.clickEvent += () =>
        {
            RankPanel.Instance.ShowMe();
            //避免穿透 隐藏自己
            HideMe();
        };
    }

}
