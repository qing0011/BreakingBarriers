using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLevelMgr : MonoBehaviour
{
    // 单例实例
    public static GameLevelMgr Instance;

    // 当前关卡ID
    private int currentLevelId;

    // 剩余时间
    private int remainTime;

    // 游戏是否正在运行
    public bool isRunning;

    // 计时协程引用
    private Coroutine timeCoroutine;

    // 当前得分（属性，外部可读不可写）
    public int CurrentScore;

    private void Awake()
    {
        // 单例模式实现
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // 使对象在场景切换时不被销毁
        DontDestroyOnLoad(gameObject);
    }

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
        if (levelId == 1)
        {
            CurrentScore = 0;
        }
        // 初始化游戏状态
        remainTime = data.timeLimit;  // 从数据中获取时间限制
        isRunning = true;

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

        // 启动计时协程
        if (timeCoroutine != null)
            StopCoroutine(timeCoroutine);

        timeCoroutine = StartCoroutine(TimeCounter());
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
        UIManager.Instance.ShowPanel<FailPanel>();
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
    }

    // 继续下一关
    public void ContinueNextLevel()
    {
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

    // 下一场景加载完成回调（当前未使用）
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
            // 不是游戏关卡场景，直接返回
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
}