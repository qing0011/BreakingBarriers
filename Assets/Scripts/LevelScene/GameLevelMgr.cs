using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLevelMgr : MonoBehaviour
{
    // 单例实例
    public static GameLevelMgr Instance;

    // 当前关卡ID
    public  int currentLevelId;

    // 剩余时间
    private int remainTime;

    // 游戏是否正在运行
    public bool isRunning;

    // 计时协程引用
    private Coroutine timeCoroutine;

    // 当前得分（属性，外部可读不可写）
    public int CurrentScore;



    ////加载怪物和玩家

    //public PlayerObj player;

    ////所欲的出怪点
    //private List<MonsterPoint> points = new List<MonsterPoint>();




    private void Awake()
    {


        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

    }

  
    ////获取场景中玩家的出生位置
    //Transform heroPos = GameObject.Find("HeroBornPos").transform;
    ////实例化玩家预制体， 把他的位置角度设置为 场景当中出生点一致
    //GameObject mosterObj1 = GameObject.Instantiate(Resources.Load<GameObject>("Monster/ "));

    #region 场景相关关卡
    private void OnEnable()
    {
        // 注册场景加载完成事件
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // 取消注册场景加载事件
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 开始指定关卡
    public void StartLevel(int levelId)
    {
        // 从游戏数据管理器中查找对应ID的关卡数据
        SceneData data = GameDataMgr.Instance.sceneDataList
            .Find(s => s.id == levelId);

        if (data == null)
        {
            // 未找到关卡数据，直接返回
            return;
        }

        // 设置当前关卡数据到管理器
        GameDataMgr.Instance.currentSceneData = data;
        GameDataMgr.Instance.currentSceneId = data.id;
        // 设置当前关卡ID
        currentLevelId = data.id;
        // 第一关时清空积分
        //if (levelId == 1)
        
            CurrentScore = 0;
        
        ////每关开始都重置玩家血量
        //if (GameDataMgr.Instance.playerData != null)
        //{
        //    GameDataMgr.Instance.playerData.hp = GameDataMgr.Instance.playerData.maxHp;
        //    Debug.Log($"第{levelId}关开始，玩家血量重置为：{GameDataMgr.Instance.playerData.hp}");
        //}
        // 初始化游戏状态
        remainTime = data.timeLimit;  // 从数据中获取时间限制
        isRunning = true;

        // 进入新关卡，更新玩家属性
        GameDataMgr.Instance.OnEnterLevel(data.id);

        // 显示游戏UI面板
        UIManager.Instance.ShowPanel<GamePanel>();

        // 获取游戏面板引用
        GamePanel panel = UIManager.Instance.GetPanel<GamePanel>();

        if (panel == null)
        {
            // 面板获取失败，返回
            return;
        }
       
        // 初始化UI显示
        panel.SetTime(remainTime);
        panel.SetScore(CurrentScore);
        // 延迟一帧更新血量UI
        StartCoroutine(DelayedUpdateHP(panel));
        // 启动计时协程
        if (timeCoroutine != null)
            StopCoroutine(timeCoroutine);

        timeCoroutine = StartCoroutine(TimeCounter());
        LoadPlayer();

    }

    // 延迟更新血量UI
    private IEnumerator DelayedUpdateHP(GamePanel panel)
    {
        yield return null; // 等待一帧，确保玩家对象已创建

        // 从数据层获取最新血量
        if (GameDataMgr.Instance.playerData != null)
        {
            int maxHp = GameDataMgr.Instance.playerData.maxHp;
            int hp = GameDataMgr.Instance.playerData.hp;
            panel.UpdateHP(maxHp, hp);
           // Debug.Log($"延迟更新UI血量：{hp}/{maxHp}");
        }
    }
    // 计时协程
    private IEnumerator TimeCounter()
    {
        // 当游戏运行且时间未耗尽时持续计时
        while (isRunning && remainTime > 0)
        {
            yield return new WaitForSeconds(1f);

            remainTime--;

            // 更新UI显示(第一种）
            // UIManager.Instance.GetPanel<GamePanel>().SetTime(remainTime);
            //UI消失时不更新
            var panel = UIManager.Instance.GetPanel<GamePanel>();
            if (panel != null)
            {
                panel.SetTime(remainTime);
            }
        }

        // 时间耗尽处理
        if (remainTime <= 0)
        {
            OnTimeOut();
        }
    }

    // 增加分数
    public void AddScore(int value)
    {
        CurrentScore += value;

        // 更新UI显示
        UIManager.Instance.GetPanel<GamePanel>().SetScore(CurrentScore);
       
    }

    // 超时处理
    private void OnTimeOut()
    {
        isRunning = false;

        // 停止计时协程
        if (timeCoroutine != null)
        {
            StopCoroutine(timeCoroutine);
            timeCoroutine = null;
        }
      
        // 显示失败面板
       // UIManager.Instance.ShowPanel<FailPanel>();
        if (UIManager.Instance.ShowPanel<FailPanel>())
        {
            // 暂停游戏时间
            Time.timeScale = 0f;
            return;
        }
        else
        {
            UIManager.Instance.ShowPanel<FailPanel>();
            // 暂停游戏时间
            Time.timeScale = 0f;
        }
    }

    // 关卡完成处理
    public void OnLevelFinish()
    {
        // 如果游戏不在运行状态则返回
        if (!isRunning) return;

        // 停止游戏运行
        isRunning = false;
        StopTimeCoroutine();
       
            // 显示通关面板
            var panel = UIManager.Instance.ShowPanel<LevelCompletePanel>();
            if (panel != null)
            {
                // 设置面板文本（当前为空字符串）
                panel.SetText(" ");
            }

        if (UIManager.Instance.ShowPanel<LevelCompletePanel>())
        {
            // 暂停游戏时间
            Time.timeScale = 0f;
            return;
        }
        else
        {
            UIManager.Instance.ShowPanel<FailPanel>();
            // 暂停游戏时间
            Time.timeScale = 0f;
        }

    }

    // 继续下一关
    public void ContinueNextLevel()
    {
        // 恢复游戏时间
        Time.timeScale = 1f;
        
        // 计算下一关ID
        int nextId = GameDataMgr.Instance.currentSceneId + 1;

        // 查找下一关数据
        SceneData nextLevel = GameDataMgr.Instance.sceneDataList
            .Find(s => s.id == nextId);

        if (nextLevel == null)
        {
            // 没有下一关（可能是最后一关）
            return;
        }
      
        // 加载下一关场景
        SceneManager.LoadScene(nextLevel.sceneName);
    }
   

    // 下一场景加载完成回调
    private void OnNextSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnNextSceneLoaded;
        StartLevel(currentLevelId + 1);
    }

    // 场景加载完成回调
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 根据场景名称查找对应的关卡数据
        SceneData data = GameDataMgr.Instance.sceneDataList
            .Find(s => s.sceneName == scene.name);

        if (data == null)
        {
            // 不是游戏关卡场景，停止计时协程并重置游戏状态
            isRunning = false;
            StopTimeCoroutine();
            return;
        }

        // 开始对应关卡
        StartLevel(data.id);
    }

    // 停止计时协程
    private void StopTimeCoroutine()
    {
        if (timeCoroutine != null)
        {
            StopCoroutine(timeCoroutine);
            timeCoroutine = null;
        }
    }

    #endregion

    #region 怪物相关关卡

    // 加载玩家
    private void LoadPlayer()
    {
        Transform heroPos = GameObject.Find("HeroBornPos")?.transform;
        if (heroPos == null)
        {
            Debug.LogError("找不到 HeroBornPos 对象！");
            return;
        }

        GameObject playerObj = Instantiate(
            Resources.Load<GameObject>("Player/PlayerPrefab"),
            heroPos.position,
            heroPos.rotation
        );

        PlayerObj player = playerObj.GetComponent<PlayerObj>();

        // 确保玩家以满血状态出现
       // GameDataMgr.Instance.playerData.hp = GameDataMgr.Instance.playerData.maxHp;
        player.ApplyRuntimeData();


        // 告诉摄像机：目标是谁
        CameraMove cam = Camera.main.GetComponent<CameraMove>();
        if (cam != null)
        {
            cam.SetTarget(player.transform);
        }
        else
        {
            Debug.LogError("Main Camera 上没有 CameraMove 组件！");
        }


        //  延迟恢复
        StartCoroutine(RestoreWeaponNextFrame(player));
    }

    private IEnumerator RestoreWeaponNextFrame(PlayerObj player)
    {
        yield return null; // 等一帧，确保 Start() 执行完

        int weaponId = GameDataMgr.Instance.playerData.weaponId;

        if (weaponId >= 0)
        {
            player.ChangeWeaponById(weaponId);
           // Debug.Log("【武器继承】已恢复武器 ID：" + weaponId);
        }
        else
        {
           // Debug.LogWarning("【武器继承】玩家当前没有任何武器");
        }
       // Debug.Log("【weaponPos】" + player.weaponPos);


        //Debug.Log("【武器检查】nowWeapon = " + (player.nowWeapon == null ? "NULL" : "OK"));
    }


    #endregion
    public void GetPlayerLevel(PlayerObj player)
    {

    }
}