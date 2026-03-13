using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems; // 用于摇杆事件


public class GamePanel : BasePanel
{
    public TMP_Text labScore;
    public TMP_Text labTime;
    public Button btnSetting;
    public Button btnReturn;
    public Button btnFire;

    // ====== 摇杆相关UI组件 ======
    [Header("摇杆设置")]
    public GameObject joystickPanel;      // 摇杆背景面板
    public Image joystickBg;               // 摇杆背景图
    public Image joystickHandle;           // 摇杆手柄图
    public float joystickRadius = 100f;    // 摇杆移动半径

    // 摇杆数据
    private Vector2 joystickInputDirection; // 摇杆输入方向
    private bool isJoystickPressed = false; // 摇杆是否被按下
    private Vector2 joystickOriginPos;      // 摇杆初始位置
    [Header("摇杆灵敏度设置")]
    public float joystickSensitivity = 1.0f;  // 灵敏度系数，默认1.0
    public float deadZone = 0.1f;  // 死区阈值，避免微小抖动

    // ====== 玩家移动设置 ======
    [Header("玩家移动设置")]
    public float moveSpeed = 5f;            // 移动速度
    public string playerTag = "Player";     // 玩家标签
    public bool autoRotateToMoveDirection = true; // 是否自动面向移动方向
    public float rotationSpeed = 10f;        // 旋转速度

    public TMP_Text hpText;   // HP 数值文字
    public Image hpFill;      // 血条填充

    //public float hpw = 350;
    private int time;

    // ====== 积分特效相关 ======
    [Header("积分特效设置")]
    public Transform scoreEffectTarget;  // 特效飞向的目标位置（在UI中创建一个空对象）
    public TMP_FontAsset scoreEffectFont; // 特效文字字体
    public Color scoreEffectColor = Color.yellow; // 特效文字颜色

    // 对象池相关
    private Queue<GameObject> scoreEffectPool = new Queue<GameObject>();
    private const int EFFECT_POOL_SIZE = 5;
    private List<Coroutine> activeCoroutines = new List<Coroutine>();

    // 缓存玩家对象
    private GameObject cachedPlayer;
    private float playerSearchTimer = 0f;
    private const float PLAYER_SEARCH_INTERVAL = 0.5f; // 每0.5秒搜索一次玩家
    
    // 新增：用于存储移动输入
    private Vector3 moveInput = Vector3.zero;
    private bool hasMoveInput = false;

    // 新增：相机引用
    private CameraMove cameraMove;

    private PlayerController playerController;
    //监听事件按钮
    public override void Init()
    {
        btnSetting.onClick.RemoveAllListeners();
        btnSetting.onClick.AddListener(() =>
        {

            UIManager.Instance.ShowPanel<SettingPanel>();
            //Time.timeScale = 0.1f;
        });

        btnFire.onClick.RemoveAllListeners();
        btnFire.onClick.AddListener(() =>
        {
            OnFireButtonClick();

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

        // 初始化摇杆
        InitializeJoystick();

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
        // 获取相机移动脚本
        FindCameraMove();
    }
    // 新增：查找相机移动脚本
    private void FindCameraMove()
    {
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            cameraMove = mainCam.GetComponent<CameraMove>();
        }
    }
    // 初始化摇杆
    private void InitializeJoystick()
    {
        if (joystickBg == null || joystickHandle == null)
        {
            Debug.LogWarning("请为摇杆分配背景和手柄图片！");
            return;
        }

        // 记录摇杆初始位置（手柄应该在背景中心）
        joystickOriginPos = joystickHandle.rectTransform.anchoredPosition;

        // 为摇杆背景添加事件监听
        EventTrigger bgTrigger = joystickBg.gameObject.GetComponent<EventTrigger>();
        if (bgTrigger == null)
        {
            bgTrigger = joystickBg.gameObject.AddComponent<EventTrigger>();
        }

        // 添加指针按下事件
        EventTrigger.Entry pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((data) => { OnJoystickPointerDown((PointerEventData)data); });
        bgTrigger.triggers.Add(pointerDown);

        // 添加指针抬起事件
        EventTrigger.Entry pointerUp = new EventTrigger.Entry();
        pointerUp.eventID = EventTriggerType.PointerUp;
        pointerUp.callback.AddListener((data) => { OnJoystickPointerUp(); });
        bgTrigger.triggers.Add(pointerUp);

        // 添加拖拽事件
        EventTrigger.Entry drag = new EventTrigger.Entry();
        drag.eventID = EventTriggerType.Drag;
        drag.callback.AddListener((data) => { OnJoystickDrag((PointerEventData)data); });
        bgTrigger.triggers.Add(drag);

        // 默认显示摇杆
        if (joystickPanel != null)
        {
            joystickPanel.SetActive(true);
        }
    }

    // 摇杆按下事件
    private void OnJoystickPointerDown(PointerEventData eventData)
    {
        isJoystickPressed = true;

        // 将摇杆移动到触摸位置
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBg.rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint);

        // 更新手柄位置
        UpdateJoystickHandlePosition(localPoint);
    }

    // 摇杆拖拽事件
    private void OnJoystickDrag(PointerEventData eventData)
    {
        if (!isJoystickPressed) return;

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBg.rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint);

        // 更新手柄位置
        UpdateJoystickHandlePosition(localPoint);
    }

    // 摇杆抬起事件
    private void OnJoystickPointerUp()
    {
        isJoystickPressed = false;
        joystickInputDirection = Vector2.zero;

        // 手柄回到中心位置
        if (joystickHandle != null)
        {
            joystickHandle.rectTransform.anchoredPosition = joystickOriginPos;
        }
    }

    // 更新摇杆手柄位置并计算方向
    private void UpdateJoystickHandlePosition(Vector2 localPoint)
    {
        // 计算方向向量（从中心到手柄）
        Vector2 direction = localPoint - joystickOriginPos;

        // 限制在半径范围内
        float distance = direction.magnitude;
        if (distance > joystickRadius)
        {
            direction = direction.normalized * joystickRadius;
        }

        // 更新手柄位置
        joystickHandle.rectTransform.anchoredPosition = joystickOriginPos + direction;

        // 计算输入方向（线性输入）
        
        Vector2 rawInput = direction / joystickRadius;

        // 限制最大值
        rawInput = Vector2.ClampMagnitude(rawInput, 1f);

        // 死区处理
        if (rawInput.magnitude < deadZone)
        {
            joystickInputDirection = Vector2.zero;
        }
        else
        {
            //// 去掉死区后重新归一化
            //float adjustedMagnitude = (rawInput.magnitude - deadZone) / (1 - deadZone);

            //// ⭐ 曲线增强（手感关键）
            //float curved = adjustedMagnitude * adjustedMagnitude;

            //joystickInputDirection = rawInput.normalized * curved * joystickSensitivity;
            joystickInputDirection = rawInput.normalized * rawInput.magnitude * joystickSensitivity;
        
        }
    }
    // 每帧更新玩家移动
    private void Update()
    {
        //定期查看玩家和相机
        playerSearchTimer -= Time.deltaTime;
        if (playerSearchTimer <= 0f)
        {
            FindPlayer();
            if (cameraMove == null)
            {
                FindCameraMove();
            }
            playerSearchTimer = PLAYER_SEARCH_INTERVAL;
        }
        // 记录移动输入（每帧更新）
        if (isJoystickPressed && joystickInputDirection != Vector2.zero)
        {
            hasMoveInput = true;
            moveInput = joystickInputDirection;
        }
        else
        {
            hasMoveInput = false;
            moveInput = Vector3.zero;
        }
    }

    // 在 LateUpdate 中应用移动（确保相机已经更新）
    private void LateUpdate()
    {
        if (cachedPlayer == null) return;

        if (playerController == null)
            playerController = cachedPlayer.GetComponent<PlayerController>();

        if (playerController == null) return;

        Camera cam = Camera.main;

        Vector3 forward = cam.transform.forward;
        Vector3 right = cam.transform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDir =
            forward * joystickInputDirection.y +
            right * joystickInputDirection.x;

        playerController.SetMoveInput(moveDir);
    }

    // 查找玩家对象
    private void FindPlayer()
    {
        if (cachedPlayer == null || !cachedPlayer.activeInHierarchy)
        {
            cachedPlayer = GameObject.FindGameObjectWithTag(playerTag);
        }
    }

    // 使用摇杆移动玩家
    private void MovePlayerWithJoystick()
    {
        if (cachedPlayer == null) return;
        Rigidbody rb = cachedPlayer.GetComponent<Rigidbody>();
        if (rb == null) return;

        Camera cam = Camera.main;
        if (cam == null) return;
        // 获取相机方向
        Vector3 forward = cam.transform.forward;
        Vector3 right = cam.transform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();
        // 计算移动方向
        Vector3 moveDirection =forward * joystickInputDirection.y +right * joystickInputDirection.x;

        float inputStrength = joystickInputDirection.magnitude;

        if (inputStrength > 0.1f)
        {
            moveDirection.Normalize();

            // ⭐ 直接设置速度，而不是用加速度（消除物理惯性）
            Vector3 targetVelocity = moveDirection * moveSpeed * inputStrength;
            targetVelocity.y = rb.velocity.y; // 保持垂直速度

            // 直接赋值速度，不会有飘逸
            rb.velocity = targetVelocity;

            // 面向移动方向
            if (autoRotateToMoveDirection)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            }
        }
        else
        {
            // 没有输入时，减速停止
            Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.velocity = new Vector3(
                Mathf.Lerp(horizontalVelocity.x, 0, 10f * Time.fixedDeltaTime),
                rb.velocity.y,
                Mathf.Lerp(horizontalVelocity.z, 0, 10f * Time.fixedDeltaTime)
            );
        }
        //Vector3 targetVelocity =
        //    moveDirection.normalized * moveSpeed * inputStrength;

        //Vector3 velocityChange = targetVelocity - rb.velocity;
        //velocityChange.y = 0;

        //float acceleration = 18f;

        //rb.AddForce(velocityChange * acceleration, ForceMode.Acceleration);
    }

    // 开火按钮点击处理方法
    private void OnFireButtonClick()
    {
        if (cachedPlayer == null)
        {
            FindPlayer();
            if (cachedPlayer == null)
            {
                Debug.LogWarning("未找到玩家对象！请确保玩家有正确的标签");
                return;
            }
        }

        // 方法1：通过SendMessage调用（可以调用任何方法）
        cachedPlayer.SendMessage("Fire", SendMessageOptions.DontRequireReceiver);

        // 方法2：查找并调用武器组件
        // 查找玩家身上的武器脚本（根据您的实际脚本名修改）
        MonoBehaviour weapon = cachedPlayer.GetComponent<MonoBehaviour>();
        if (weapon != null)
        {
            // 尝试调用Fire方法
            System.Reflection.MethodInfo method = weapon.GetType().GetMethod("Fire");
            if (method != null)
            {
                method.Invoke(weapon, null);
            }
        }
    }

    // 初始化特效对象池
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

    // 创建积分特效的公共方法
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

        // 设置初始大小更大
        Vector3 initialScale = new Vector3(1.5f, 1.5f, 1f); // 初始放大1.5倍
        effect.transform.localScale = initialScale;

        // 先放大再缩小的动画
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

        // 调整飞行过程中的缩放
        float flyScale = 1.8f; // 飞行时的基础大小

        // 飞行过程
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float easeT = EaseOutCubic(t);

            // 移动到目标位置
            effect.transform.position = Vector3.Lerp(startPos, targetPos, easeT);

            // 调整缩放逻辑
            // 飞行过程中逐渐缩小，但保持更大
            float scaleFactor = 1f - t * 0.5f; // 从1缩小到0.5
            effect.transform.localScale = new Vector3(
                flyScale * scaleFactor,
                flyScale * scaleFactor,
                1f
            );

            // 更晚开始淡出
            if (t > 0.8f) // 从80%开始淡出
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

    //缓动函数
    private float EaseOutCubic(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3);
    }

    // 特效到达目标后的处理
    private void OnEffectReachedTarget(int scoreValue, GameObject effect)
    {
        // 1. 播放UI反馈效果
        StartCoroutine(PlayScoreUIFeedback());

        // 2. 重置特效并放回对象池
        ResetEffect(effect);
        scoreEffectPool.Enqueue(effect);
    }

    // 重置特效
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

    //UI反馈动画
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

    // 清理协程
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

    // 对外接口
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