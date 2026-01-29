// ScoreFlyEffect.cs
using UnityEngine;
using TMPro;
using System.Collections;

public class ScoreFlyEffect : MonoBehaviour
{
    [Header("组件设置")]
    public TextMeshProUGUI scoreText;    // 显示分数的文本
    public float flySpeed = 8f;          // 飞行速度
    public float fadeSpeed = 2f;         // 淡出速度

    private Vector3 targetPosition;      // 目标位置
    private int scoreValue;              // 分数值
    private bool isFlying = false;
    private CanvasGroup canvasGroup;     // 控制透明度

    void Awake()
    {
        // 确保有CanvasGroup组件
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    void Update()
    {
        if (!isFlying) return;

        // 1. 向目标位置移动
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            flySpeed * Time.deltaTime
        );

        // 2. 飞行过程中逐渐缩小
        float distance = Vector3.Distance(transform.position, targetPosition);
        float scaleFactor = Mathf.Clamp01(distance / 10f); // 根据距离计算缩放
        transform.localScale = Vector3.one * (0.5f + scaleFactor * 0.5f);

        // 3. 接近目标时淡出
        if (distance < 2f)
        {
            canvasGroup.alpha -= fadeSpeed * Time.deltaTime;
        }

        // 4. 到达目标或完全透明后销毁
        if (distance < 0.1f || canvasGroup.alpha <= 0)
        {
            OnReachedTarget();
        }
    }

    // 初始化特效
    public void Initialize(int value, Vector3 targetPos)
    {
        scoreValue = value;
        targetPosition = targetPos;

        // 显示分数
        if (scoreText != null)
            scoreText.text = "+" + value.ToString();

        // 初始透明度
        canvasGroup.alpha = 1f;

        // 开始飞行
        isFlying = true;

        // 3秒后强制销毁（防止卡住）
        Destroy(gameObject, 3f);
    }

    void OnReachedTarget()
    {
        // 通知加分（可以通过事件或直接调用）
        // 这里先简单处理，稍后会在GamePanel中完善
        Destroy(gameObject);
    }
}