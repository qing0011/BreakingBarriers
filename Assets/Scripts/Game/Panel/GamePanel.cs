using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GamePanel : BasePanel
{
    public TMP_Text labScore;
    public TMP_Text labTime;
    public Button btnSetting;
    public Button btnReturn;

    public TMP_Text hpText;   // HP 数值文字
    public Image hpFill;      // 血条填充

    public float hpw = 350;
    private int time;

    // ====== 新增：积分特效相关 ======
    [Header("积分特效设置")]
    public Transform scoreEffectTarget;  // 特效飞向的目标位置（在UI中创建一个空对象）
    public TMP_FontAsset scoreEffectFont; // 特效文字字体
    public Color scoreEffectColor = Color.yellow; // 特效文字颜色

    // 对象池相关
    private Queue<GameObject> scoreEffectPool = new Queue<GameObject>();
    private const int EFFECT_POOL_SIZE = 5;
    private List<Coroutine> activeCoroutines = new List<Coroutine>();


    //监听事件按钮
    public override void Init()
    {
      btnSetting.onClick.RemoveAllListeners();
        btnSetting.onClick.AddListener(() =>
        {

            UIManager.Instance.ShowPanel<SettingPanel>();
            //Time.timeScale = 0.1f;
        });

        btnReturn.onClick.RemoveAllListeners();
        btnReturn.onClick.AddListener(() =>
        {
            GameDataMgr.Instance.TryRefreshMaxScore(GameDataMgr.Instance.labScore);
            GameDataMgr.Instance.TryRefreshTotalScoreData();

            //返回后必须重置
            GameDataMgr.Instance.ResetGameData();
            UIManager.Instance.HidePanel<GamePanel>();
            Time.timeScale = 0.1f;

            SceneManager.LoadScene("BeginScene");
            
        });

        // 初始化特效对象池
        InitializeEffectPool();
        // 如果没有指定目标位置，使用labScore的位置
        if (scoreEffectTarget == null && labScore != null)
        {
            GameObject targetObj = new GameObject("ScoreEffectTarget");
            targetObj.transform.SetParent(labScore.transform.parent);
            targetObj.transform.position = labScore.transform.position;
            scoreEffectTarget = targetObj.transform;
        }
    }
    // ====== 新增：初始化特效对象池 ======
    private void InitializeEffectPool()
    {
        for (int i = 0; i < EFFECT_POOL_SIZE; i++)
        {
            CreateEffectInPool();
        }
    }

    private void CreateEffectInPool()
    {
        // 创建特效GameObject
        GameObject effectObj = new GameObject("ScoreEffect");
        effectObj.transform.SetParent(transform); // 作为GamePanel的子对象
        effectObj.SetActive(false);

        // 添加TextMeshPro组件
        TextMeshProUGUI text = effectObj.AddComponent<TextMeshProUGUI>();
        text.font = scoreEffectFont != null ? scoreEffectFont : labScore.font;
        text.color = scoreEffectColor;
        text.fontSize = 60;
        text.alignment = TextAlignmentOptions.Center;
        text.raycastTarget = false; // 不接收射线检测

        // 添加CanvasGroup用于淡出效果
        CanvasGroup canvasGroup = effectObj.AddComponent<CanvasGroup>();

        scoreEffectPool.Enqueue(effectObj);
    }

    // ====== 新增：创建积分特效的公共方法 ======
    public void CreateScoreEffect(Vector3 worldPosition, int scoreValue)
    {
        // 确保对象池中有可用的特效
        if (scoreEffectPool.Count == 0)
        {
            CreateEffectInPool();
        }

        // 从对象池获取特效
        GameObject effect = scoreEffectPool.Dequeue();
        effect.SetActive(true);

        // 设置初始位置：世界坐标转换为屏幕坐标
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
        effect.transform.position = screenPos;

        // 设置显示的分数文本
        TextMeshProUGUI text = effect.GetComponent<TextMeshProUGUI>();
        if (text != null)
        {
            text.text = "+" + scoreValue.ToString();
            text.alpha = 1f;
        }

        // 启动飞行动画协程
        Coroutine flyCoroutine = StartCoroutine(FlyEffectToTarget(effect, scoreValue));
        activeCoroutines.Add(flyCoroutine);
    }
    //特效飞向目标
    private IEnumerator FlyEffectToTarget(GameObject effect, int scoreValue)
    {
        Vector3 startPos = effect.transform.position;
        Vector3 targetPos = scoreEffectTarget.position;
        float duration = 0.8f; // 飞行总时间
        float elapsed = 0f;

        // 获取组件
        TextMeshProUGUI text = effect.GetComponent<TextMeshProUGUI>();
        CanvasGroup canvasGroup = effect.GetComponent<CanvasGroup>();

        // ====== 修改1：设置初始大小更大 ======
        Vector3 initialScale = new Vector3(1.5f, 1.5f, 1f); // 初始放大1.5倍
        effect.transform.localScale = initialScale;

        // ====== 新增：先放大再缩小的动画 ======
        float popDuration = 0.2f; // 弹出动画时间
        float popElapsed = 0f;

        // 先快速放大（弹出效果）
        while (popElapsed < popDuration)
        {
            popElapsed += Time.deltaTime;
            float t = popElapsed / popDuration;
            effect.transform.localScale = Vector3.Lerp(initialScale * 0.5f, initialScale * 2f, t);
            yield return null;
        }

        // ====== 修改2：调整飞行过程中的缩放 ======
        float flyScale = 1.8f; // 飞行时的基础大小

        // 飞行过程
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float easeT = EaseOutCubic(t);

            // 移动到目标位置
            effect.transform.position = Vector3.Lerp(startPos, targetPos, easeT);

            // ====== 修改3：调整缩放逻辑 ======
            // 飞行过程中逐渐缩小，但保持更大
            float scaleFactor = 1f - t * 0.5f; // 从1缩小到0.5
            effect.transform.localScale = new Vector3(
                flyScale * scaleFactor,
                flyScale * scaleFactor,
                1f
            );

            // ====== 修改4：更晚开始淡出 ======
            if (t > 0.8f) // 从80%开始淡出（原来是70%）
            {
                if (canvasGroup != null)
                    canvasGroup.alpha = 1f - (t - 0.8f) / 0.2f;
                else if (text != null)
                    text.alpha = 1f - (t - 0.8f) / 0.2f;
            }

            yield return null;
        }

        // 到达目标后
        OnEffectReachedTarget(scoreValue, effect);
    }

    // ======缓动函数（不使用DOTween） ======
    private float EaseOutCubic(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3);
    }

    // ====== 特效到达目标后的处理 ======
    private void OnEffectReachedTarget(int scoreValue, GameObject effect)
    {
        // 1. 更新分数（使用原有的SetScore方法）
        int currentScore = GameDataMgr.Instance.labScore + scoreValue;
        SetScore(currentScore);

        // 2. 播放UI反馈效果
        StartCoroutine(PlayScoreUIFeedback());

        // 3. 重置特效并放回对象池
        ResetEffect(effect);
        scoreEffectPool.Enqueue(effect);
    }

    // ====== 重置特效 ======
    private void ResetEffect(GameObject effect)
    {
        effect.SetActive(false);
        effect.transform.localScale = Vector3.one;

        // 重置透明度
        CanvasGroup canvasGroup = effect.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
            canvasGroup.alpha = 1f;

        TextMeshProUGUI text = effect.GetComponent<TextMeshProUGUI>();
        if (text != null)
            text.alpha = 1f;
    }

    // ======UI反馈动画 ======
    private IEnumerator PlayScoreUIFeedback()
    {
        if (labScore == null) yield break;

        Vector3 originalScale = labScore.transform.localScale;
        float duration = 0.3f;
        float elapsed = 0f;

        // 放大
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            labScore.transform.localScale = originalScale * (1f + Mathf.Sin(t * Mathf.PI) * 0.3f);
            yield return null;
        }

        // 恢复
        labScore.transform.localScale = originalScale;
    }

    // ====== 新增：清理协程 ======
    public override void HideMe(UnityAction callBack)
    {
        // 停止所有活跃的协程
        foreach (Coroutine coroutine in activeCoroutines)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
        }
        activeCoroutines.Clear();

        base.HideMe(callBack);
    }

    // ====== 对外接口 ======
    public void SetScore(int score)
    {
        
        labScore.text = score.ToString();
        GameDataMgr.Instance.labScore = score;
    }
    public void SetTime(int seconds)
    {
        time = seconds;
        UpdateTimeDisplay();
    }
    // ====== UI 内部 ======
    // 更新时间显示
    private void UpdateTimeDisplay()
    {
        labTime.text = " ";
        if (time / 3600 > 0)
        {
            labTime.text += time / 3600 + "H";
        }
        if (time % 3600 / 60 > 0 || labTime.text != " ")
        {
            labTime.text += time % 3600 / 60 + "M";
        }
        labTime.text += time % 60 + "S";
    }
    // 更新血条
    public void UpdateHP(int maxHP, int HP)
    {
        // 更新文字
        hpText.text = $"{HP} / {maxHP}";
        float ratio = Mathf.Clamp01((float)HP / maxHP);

        // 更新血条
        hpFill.fillAmount = ratio;


    }


}