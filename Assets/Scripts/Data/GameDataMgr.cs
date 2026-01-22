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

    //怪物

    public List<MonsterData> monsterDataList;

    //需要重置的数据
    public int maxHP = 100;
    public int playerHP;
    public int labScore;
    public int labTime;


    //玩家最高分记录
    public ScoreData scoreData;

    private const string SCORE_SAVE_FILE = "scoreData";
    //继续游戏基础价格
    private const int CONTINUE_BASE_COST = 10;

    private GameDataMgr()
    {
        //可以去初始化 游戏数据
        musicData = JsonMgr.Instance.LoadData<MusicData>("MusicData");

        //初始化 读取 排行榜数据
        rankData = JsonMgr.Instance.LoadData<RankList>("Rank");

        sceneDataList = JsonMgr.Instance.LoadData<List<SceneData>>("SceneData");

        //怪物
        monsterDataList = JsonMgr.Instance.LoadData<List<MonsterData>>("MonsterData");

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
        //积分存储
        scoreData = JsonMgr.Instance.LoadData<ScoreData>(SCORE_SAVE_FILE);
        if (scoreData == null)
        {
            scoreData = new ScoreData();
            SaveScoreData(); // 立刻生成存档
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
        //重置本局继续次数
        scoreData.continueCount = 0;
        SaveScoreData();
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

    /// <summary>
    /// 获取本次继续所需的积分
    /// 100 → 200 → 400 → ...
    /// </summary>
    public int GetContinueCost()
    {
        // 100 * 2^continueCount
        return CONTINUE_BASE_COST * (1 << scoreData.continueCount);
    }

    //最高积分方法

    public void SaveScoreData()
    {
        JsonMgr.Instance.SaveData(scoreData, SCORE_SAVE_FILE);
    }
    //l累计总计分（每次完成游戏后获得的积分与目前拥有的积分累加）
    public  void TryRefreshTotalScoreData()
    {
        //在ScoreData里面声明了两个变量数据：haveScore：是她原本拥有的，初始化为0；
        //buyContinue：这个是购买的需要花费的钱。购买这个数据写死的话是不需要的。

        //没有购买的话则就是完成关卡的积分+原视界面有的金粉进行刷新
        //如果购买了按钮的话就会从总分里面减去购买的金币或主界面的金币进行刷新

        if (labScore <= 0)
            return;

        scoreData.haveScore += labScore;
        SaveScoreData();

        Debug.Log($"【积分结算】本局:{labScore}  当前总积分:{scoreData.haveScore}");
    }
    //购买了继续积分（购买了按钮之后需要扣除相应的积分
    public bool BuyContinueScoreData()
    {
        //就会减去购买的钱数这里时写死的比如说购买一次100金币。
        int cost = GetContinueCost();

        if (scoreData.haveScore < cost)
        {
            Debug.Log("【购买失败】积分不足");
            return false;
        }

        scoreData.haveScore -= cost;
        scoreData.continueCount++; // 关键点：成功才递增
        SaveScoreData();

        Debug.Log($"【购买继续】消耗:{cost} 剩余:{scoreData.haveScore}");
        return true;
    }
    //用“本局积分”尝试刷新最高分
    public void TryRefreshMaxScore(int fistScore)
    {
        if (fistScore > scoreData.maxScore)
        {
            scoreData.maxScore = fistScore;
            SaveScoreData();
        }
    }
    /// 重置最高分
    public void ResetMaxScore()
    {
        scoreData.maxScore = 0;
        SaveScoreData();
    }
    /// 重置最高分
    public void ResetTotalScore()
    {
        scoreData.haveScore = 0;
        SaveScoreData();
    }


    //怪物数据
    public MonsterData GetMonsterData(int monsterId)
    {
        foreach (MonsterData data in monsterDataList)
        {
            if (data.id == monsterId)
                return data;
        }
        return null;
    }
}
