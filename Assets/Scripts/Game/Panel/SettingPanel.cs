using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingPanel : BasePanel
{
    public Button btnClose;
    public Toggle togMusic;
    public Toggle togSound;
    public Slider sliderMusic;
    public Slider sliderSound;
    public override void Init()
    {
        //初始化面板显示的内容 根绝本地存储的设置数据来初始化
        MusicData data = GameDataMgr.Instance.musicData;

        //初始化开关控制的状态
        togMusic.isOn = data.musicOpen;
        togSound.isOn = data.musicOpen;
        //初始化拖动条控制的大小
        sliderMusic.value = data.musicValue;
        sliderSound.value = data.soundValue;
        //关闭按钮
        btnClose.onClick.AddListener(() =>
        {
            //只有关闭面板才会记录数据（节省性能）
            GameDataMgr.Instance.SaveMusicData();
            //隐藏自己印象设置面板
            UIManager.Instance.HidePanel<SettingPanel>();
        });
        togMusic.onValueChanged.AddListener((v) =>
        {
            //让背景音乐进行开关
            BKMusic.Instance.SetIsOpen(v);
            //记录开关数据
            GameDataMgr.Instance.musicData.musicOpen = v;
        });
        togSound.onValueChanged.AddListener((v) =>
        {
            //记录开关数据
            GameDataMgr.Instance.musicData.soundOpen = v;
        });
        sliderMusic.onValueChanged.AddListener((v) =>
        {
            //让背景音乐进行开关
            BKMusic.Instance.ChangeValue(v);
            //记录开关数据
            GameDataMgr.Instance.musicData.musicValue = v;
        });
        sliderSound.onValueChanged.AddListener((v) =>
        {
            //记录开关数据
            GameDataMgr.Instance.musicData.soundValue = v;
        });


    }

}
