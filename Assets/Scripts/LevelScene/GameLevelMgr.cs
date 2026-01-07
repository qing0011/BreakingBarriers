using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;



/// <summary>
/// �ؿ����̹����������̵��ݣ�
/// ����
/// 1. ���������������
/// 2. ���� SceneData �����ؿ�
/// 3. ��������ʱ���÷֡�ͨ�ء�ʧ��

public class GameLevelMgr : MonoBehaviour
{
    // ȫ��Ψһʵ��
    public static GameLevelMgr Instance;
    // ��ǰ�ؿ� ID���߼��㣩
    private int currentLevelId;
    // ��ǰ�ؿ�ʣ��ʱ�䣨�룩
    private int remainTime;
    // ��ǰ�ؿ��Ƿ���������
    public bool isRunning;
    // ����ʱЭ�����ã�����ֹͣ
    private Coroutine timeCoroutine;
    // ��ǰ�÷֣�ֻ����
    public int CurrentScore { get; private set; }
    #region ��������
    // ���� + �糡������
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #endregion

    // ע�᳡����������¼�
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    // ��ע�ᣬ��ֹ�ظ��ص�
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    #region �ؿ���ʼ

    /// <summary>
    /// �����ؿ����ڳ���������ɺ���ã� ֻ���� OnSceneLoaded ����
    /// </summary>
    public void StartLevel(int levelId)
    {
        Debug.Log("[GameLevelMgr] StartLevel 被调用，传入的levelId: " + levelId);
        Debug.Log("[GameLevelMgr] 当前GameDataMgr.currentSceneId: " + GameDataMgr.Instance.currentSceneId);

        // �� GameDataMgr ���ùؿ�����
        SceneData data = GameDataMgr.Instance.sceneDataList
            .Find(s => s.id == levelId);

        if (data == null)
        {
            Debug.LogError($"[GameLevelMgr] δ�ҵ��ؿ����� id={levelId}");
            return;
        }

        Debug.Log("[GameLevelMgr] 找到关卡数据: id=" + data.id + ", sceneName=" + data.sceneName);
        // ���õ�ǰ�ؿ����ݣ�ȫ��Ψһ��
        GameDataMgr.Instance.currentSceneData = data;
        GameDataMgr.Instance.currentSceneId = data.id;///////////////////////////////////
        // 设置当前关卡ID
        currentLevelId = data.id;
        Debug.Log("[GameLevelMgr] 更新后的currentLevelId: " + currentLevelId);
        Debug.Log("[GameLevelMgr] 更新后的GameDataMgr.currentSceneId: " + GameDataMgr.Instance.currentSceneId);
        // ��ʼ��ʱ�䣨ÿ��ˢ�£�
        remainTime = data.timeLimit;
        isRunning = true;
        Debug.Log("[GameLevelMgr] 设置isRunning为true，当前isRunning状态: " + isRunning);

        // ȷ�� GamePanel �Ѿ�����
        UIManager.Instance.ShowPanel<GamePanel>();
        // ��ʼ�� UI
        GamePanel panel = UIManager.Instance.GetPanel<GamePanel>();


        if (panel == null)
        {
            Debug.LogError(" GamePanel ��ȻΪ null");
            return;
        }
        // ��ʼ�� UI
        panel.SetTime(remainTime);
        panel.SetScore(CurrentScore);

        // ȷ�������ظ�����Э��
        if (timeCoroutine != null)
            StopCoroutine(timeCoroutine);
        // ��������ʱ
        timeCoroutine = StartCoroutine(TimeCounter());
    }

    #endregion

    #region ����ʱ�߼�

    private IEnumerator TimeCounter()
    {
        while (isRunning && remainTime > 0)
        {
            yield return new WaitForSeconds(1f);

            remainTime--;

            UIManager.Instance.GetPanel<GamePanel>().SetTime(remainTime);
        }

        if (remainTime <= 0)
        {
            OnTimeOut();
        }
    }
    // ���ӷ���
    public void AddScore(int value)
    {
        CurrentScore += value;

        UIManager.Instance.GetPanel<GamePanel>().SetScore(CurrentScore);
    }
    // ʱ��ľ�
    private void OnTimeOut()
    {
        isRunning = false;

        // ֹͣЭ��
        if (timeCoroutine != null)
        {
            StopCoroutine(timeCoroutine);
            timeCoroutine = null;
        }

        UIManager.Instance.ShowPanel<FailPanel>();
    }
    #endregion

    #region �ؿ����

    /// <summary>
    /// ���յ� Trigger ����������
    /// </summary>
    /// // �ؿ���ɣ����յ� / ����������
    public void OnLevelFinish()
    {
        Debug.Log("[GameLevelMgr] OnLevelFinish 被调用，当前关卡ID: " + currentLevelId);
        Debug.Log("[GameLevelMgr] OnLevelFinish 中的isRunning状态: " + isRunning);
        if (!isRunning) return;

        Debug.Log("[GameLevelMgr] 执行关卡完成逻辑...");
        isRunning = false;
        StopTimeCoroutine();
        // 显示通关面板
        Debug.Log("[GameLevelMgr] 显示LevelCompletePanel...");
        var panel = UIManager.Instance.ShowPanel<LevelCompletePanel>();
        if (panel != null)
        {
            Debug.Log("[GameLevelMgr] 设置关卡完成文本...");
            panel.SetText("��ϲ����Լ����ؿ�");
        }
        else
        {
            Debug.LogError("[GameLevelMgr] 显示LevelCompletePanel失败！");
        }
    }

    /// <summary>
    /// ͨ���������������
    /// </summary>
    public void ContinueNextLevel()
    {


        int nextId = GameDataMgr.Instance.currentSceneId + 1;

        SceneData nextLevel = GameDataMgr.Instance.sceneDataList
            .Find(s => s.id == nextId);

        if (nextLevel == null)
        {
            Debug.Log("�Ѿ������һ��");
            return;
        }
        Debug.Log("Load Scene = " + nextLevel.sceneName);

        SceneManager.LoadScene(nextLevel.sceneName);
       
    }

    private void OnNextSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnNextSceneLoaded;
        StartLevel(currentLevelId + 1);

    }

    /// ���г���������ɶ��ᴥ��
    /// ���ǡ��ؿ�������ʼ����Ψһ���

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("[GameLevelMgr] OnSceneLoaded 被调用，场景名称: " + scene.name);

        // 获取当前场景的 SceneData   这个是关卡管理的直接入口
        SceneData data = GameDataMgr.Instance.sceneDataList
            .Find(s => s.sceneName == scene.name);

        if (data == null)
        {
            Debug.Log($"[GameLevelMgr] 不是关卡场景，不进行初始化: {scene.name}");
            return;
        }


        Debug.Log($"[GameLevelMgr] 加载关卡 {data.id} : {data.sceneName}");

        StartLevel(data.id);
    }

    #endregion
    private void StopTimeCoroutine()
    {
        if (timeCoroutine != null)
        {
            StopCoroutine(timeCoroutine);
            timeCoroutine = null;
        }
    }
   

  

  
}
