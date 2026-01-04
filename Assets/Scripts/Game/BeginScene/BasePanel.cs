using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanel <T>: MonoBehaviour where T:class

{
    //私有成员变量
    private static T instance;

    //获取静态方法
    public static T Instance => instance;
    //游戏开始前执行单例 
    private void Awake()
    {
        // 如果已经有实例，说明这是重复的（可能是场景里又放了一个同名UI）
        //if (instance != null && instance != this as T)
        //{
        //    Destroy(this.gameObject); // 避免重复的UI
        //    return;
        //}
        instance = this as T;
        // 跨场景保留
       // DontDestroyOnLoad(this.gameObject);
    }
    //显示隐藏两个方法
    public virtual void ShowMe()
    {
        this.gameObject.SetActive(true);
    }

    public virtual void HideMe()
    {
        this.gameObject.SetActive(false);
    }


}
