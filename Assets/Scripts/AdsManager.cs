using UnityEngine;
using System.Runtime.InteropServices;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance;

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void WX_ShowRewardAd(string type);
#endif

    private System.Action reviveCallback;
    private System.Action doubleCoinCallback;

    void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// 复活广告
    /// </summary>
    public void ShowReviveAd(System.Action onReward)
    {
        reviveCallback = onReward;

#if UNITY_WEBGL && !UNITY_EDITOR
        WX_ShowRewardAd("revive");
#else
        Debug.Log("模拟复活广告");
        onReward?.Invoke();
#endif
    }

    /// <summary>
    /// 双倍金币广告
    /// </summary>
    public void ShowDoubleCoinAd(System.Action onReward)
    {
        doubleCoinCallback = onReward;

#if UNITY_WEBGL && !UNITY_EDITOR
        WX_ShowRewardAd("double");
#else
        Debug.Log("模拟双倍金币广告");
        onReward?.Invoke();
#endif
    }

    /// <summary>
    /// JS回调复活奖励
    /// </summary>
    public void OnReviveReward()
    {
        Debug.Log("复活广告奖励发放");
        reviveCallback?.Invoke();
    }

    /// <summary>
    /// JS回调双倍金币
    /// </summary>
    public void OnDoubleCoinReward()
    {
        Debug.Log("双倍金币奖励发放");
        doubleCoinCallback?.Invoke();
    }
}