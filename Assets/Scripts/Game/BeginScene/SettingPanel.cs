using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingPanel : BasePanel<SettingPanel>
{
    //声明变量
    public CustomGUIButton btnClose;

    public CustomGUIToggle togMusic;
    public CustomGUIToggle togSound;

    public CustomGUISlider sliderMusic;
    public CustomGUISlider sliderSound;

    
  void Start()
    {
        //音乐滑杆
        sliderMusic.changeValue += (value) =>
        {
            GameDataMgr.Instance.ChangeBKValue(value);
        };
        //声音滑杆
        sliderSound.changeValue += (value) =>
        {
            GameDataMgr.Instance.ChangeSoundValue(value);
        };
        //音乐启动关闭
        togMusic.changeValue += (value) =>
        {
            GameDataMgr.Instance.OpenOrCloseBKMusic(value);
        };
        //音乐启动关闭
        togSound.changeValue += (value) =>
        {
            GameDataMgr.Instance.OpenOrCloseSound(value);
        };
        //关闭设置对话框
        btnClose.clickEvent += () =>
        {
            //隐藏面板
            HideMe();
            
          
            //判断当前所在场景 应该如何判断
            if (SceneManager.GetActiveScene().name == "BeginScene")
            {
                //让开始面板重新显示出来
                BeginPanel.Instance.ShowMe();
            }

        };
        //隐藏启动面板
        HideMe();
    }
    //当现实自己时，更新设置的信息

    public void UpdatePanelInfo()
    {
        //面板数据，重新赋值
        MusicData data = GameDataMgr.Instance.musicData;

        //设置面板内容（面板上的信息就是数据内容）

        sliderMusic.nowValue = data.bkValue;
        sliderSound.nowValue = data.soundValue;
        togMusic.isSel = data.isOpenOK;
        togSound.isSel = data.isOpenSound;
    }
    public override void ShowMe()
    {
        //面板开启
        base.ShowMe();

        //数据更新
        UpdatePanelInfo();
       
    }
    public override void HideMe()
    {
        base.HideMe();
        Time.timeScale = 1;
    }
}
