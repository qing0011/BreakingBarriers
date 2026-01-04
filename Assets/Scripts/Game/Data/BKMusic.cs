using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BKMusic : MonoBehaviour
{
   public static BKMusic instance;
    public static BKMusic Instance => instance;
    private AudioSource audioSource;
     void Awake()
    {
        instance = this;
        audioSource =this.GetComponent<AudioSource>();

        ChangeValue(GameDataMgr.Instance.musicData.bkValue);
        ChangeOpen(GameDataMgr.Instance.musicData.isOpenOK);
    }
    //改变音乐大小
    public void ChangeValue(float value)
    {
        audioSource.volume = value;
    }
    //开关音乐
    public void ChangeOpen(bool isOpen)
    {
        //开启不静音，关闭静音
        audioSource.mute = !isOpen;
    }
}
