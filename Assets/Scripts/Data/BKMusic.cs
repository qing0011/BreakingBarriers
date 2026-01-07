using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class BKMusic : MonoBehaviour
{
   public static BKMusic instance;
    public static BKMusic Instance => instance;
    private AudioSource audioSource;
     void Awake()
    {
        //初始化
        instance = this;
        audioSource = this.GetComponent<AudioSource>();
        //通过数据来设置音效大小和开关
        MusicData data = GameDataMgr.Instance.musicData;
        SetIsOpen(data.musicOpen);
        ChangeValue(data.musicValue);
    }
    //设置开关
    public void SetIsOpen(bool isOpen)
    {
        audioSource.mute = !isOpen;
    }
    //大小改变
    public void ChangeValue(float v)
    {
        audioSource.volume = v;
    }
}
