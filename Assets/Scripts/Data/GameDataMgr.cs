using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

/// <summary>
/// 这个是游戏数据管理类 是一个单例模式对象
/// </summary>
public class GameDataMgr
{
    private static GameDataMgr instance = new GameDataMgr();
    public static GameDataMgr Instance => instance;


    // ========= 签到存档 =========
    public SignInSaveData signInSaveData;
    //签到数据对象列表
    public List<SignInInfo> signInInfoList;


    //音效数据对象
    public MusicData musicData;
    //排行榜数据对象
    public RankList rankData;

    //关卡数据对象
    public int currentSceneId = 1;
    public SceneData currentSceneData;

    public List<SceneData> sceneDataList;

    //需要重置的数据
    public int maxHP = 100;
    public int playerHP;
    public int labScore;
    public int labTime;

    private GameDataMgr()
    {
        //可以去初始化 游戏数据
        musicData = JsonMgr.Instance.LoadData<MusicData>("MusicData");

        //初始化 读取 排行榜数据
        rankData = JsonMgr.Instance.LoadData<RankList>("Rank");

        sceneDataList = JsonMgr.Instance.LoadData<List<SceneData>>("SceneData");

        //签到
        signInInfoList = JsonMgr.Instance.LoadData<List<SignInInfo>>("SignInInfo");
        //初始化签到存档（重点）
        signInSaveData =JsonMgr.Instance.LoadData<SignInSaveData>("SignInSave");
        if (signInSaveData == null)
        {
            signInSaveData = new SignInSaveData
            {
                signInCount = 0,
                lastSignInTime = ""
            };
            SaveSignInData();
        }

    }
    /// <summary>
    /// 新游戏 / 返回开始界面时调用 重置
    /// </summary>
    public void ResetGameData()
    {
        playerHP = maxHP;
        labScore = 0;
        labTime = 0;

        Debug.Log("【GameDataMgr】游戏数据已重置");
    }
    public SceneData GetCurrentSceneData()
    {
        return sceneDataList.Find(s => s.id == currentSceneId);
    }

    public void SaveSignInData()
    {
        JsonMgr.Instance.SaveData(signInSaveData, "SignInSave");
    }
    //提供一些API给外部 方便数据的改变存储

    //提供一个 在排行榜中添加数据的方法
    public void SaveMusicData()
    {
        JsonMgr.Instance.SaveData(musicData, "MusicData");
    }
    public void AddRankInfo(string name, int score, float time)
    {
        rankData.list.Add(new RankInfo(name, score, time));
        //排序
        rankData.list.Sort((a, b) => a.time < b.time ? -1 : 1);
        //排序过后 移除10条以外的数据
        //从尾部往前遍历 移除每一条
        for (int i = rankData.list.Count - 1; i >= 10; i--)
        {
            rankData.list.RemoveAt(i);
        }
        //存储
        JsonMgr.Instance.SaveData(rankData, "Rank");
    }
    public void SaveSiginInfo()
    {
        //signInInfo.Add(new SignInInfo(id, name));
    }
    public void PlaySound(string resName)
    {
        GameObject musicObj = new GameObject();
        AudioSource a = musicObj.AddComponent<AudioSource>();
        a.clip = Resources.Load<AudioClip>(resName);
        a.volume = musicData.soundValue;
        a.mute = !musicData.soundOpen;
        a.Play();

        GameObject.Destroy(musicObj, 1);
    }

}
