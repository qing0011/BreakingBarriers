using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MonsterObj : TankBaseObj
{
    //目标点
    private Transform targetPos;
    //随机点
    public Transform[] randomPos;
    //朝向的目标
    private Transform lookAtTarget;


    //开火距离
    public float fireDis=5;
    //攻击间隔时间
    public float fireOffsetTime = 0.2f;


    //计时间
    private float nowTime = 0;
    //开火点
    public Transform[] shootPos;
    //子弹预制体
    public GameObject bulletObj;

    private float showTime = 0;

    public int monsterScore = 10;

    [Header("血条预制体")]
    public GameObject hpBarPrefab;

    private Transform hpBarRoot;
    private Image hpFill;


    [Header("触发音效")]
    public AudioClip monshitClip;

    void Start()
    {
        RandomPos();

        if (hpBarRoot != null)
            hpBarRoot.gameObject.SetActive(false);
        CreateHpBar();   
        UpdateHpUI();
    }
    void LateUpdate()
    {
        if (hpBarRoot == null) return;

        hpBarRoot.forward = Camera.main.transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        // ========= 动态寻找玩家 =========
        if (lookAtTarget == null)
        {
            FindPlayer();
        }

        // 计算与玩家的距离（如果玩家存在）
        float disToPlayer = float.MaxValue;
        if (lookAtTarget != null)
        {
            disToPlayer = Vector3.Distance(transform.position, lookAtTarget.position);
        }

        // 决定当前应该看向的目标（用于旋转）
        Transform currentLookTarget = null;

        // ========= 攻击/追逐逻辑 =========
        if (lookAtTarget != null && disToPlayer <= fireDis)
        {
            // 在攻击范围内 - 看向玩家并攻击
            currentLookTarget = lookAtTarget;

            nowTime += Time.deltaTime;
            if (nowTime >= fireOffsetTime)
            {
                Fire();
                nowTime = 0;
            }
        }
        else if (lookAtTarget != null && disToPlayer > fireDis)
        {
            // 玩家在攻击范围外但可见 - 看向玩家（追逐）
            currentLookTarget = lookAtTarget;
        }

        // ========= 巡逻逻辑 =========
        if (targetPos != null)
        {
            // 判断是否需要巡逻
            bool shouldPatrol = (lookAtTarget == null) || (lookAtTarget != null && disToPlayer > fireDis);

            if (shouldPatrol)
            {
                // 巡逻时，应该看向巡逻目标点
                currentLookTarget = targetPos;

                // 向巡逻点移动（保持Y轴不变）
                Vector3 targetPosition = new Vector3(
                    targetPos.position.x,
                    transform.position.y,
                    targetPos.position.z
                );

                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPosition,
                    moveSpeed * Time.deltaTime
                );

                // 到达目标点后选择新的巡逻点
                if (Vector3.Distance(transform.position, targetPos.position) < 0.5f)
                {
                    RandomPos();
                }
            }
        }

        // ========= 统一的朝向处理 =========
        if (currentLookTarget != null)
        {
            // 创建目标位置（保持怪物的Y轴高度，但可以略微调整让视线更自然）
            Vector3 targetPosition = new Vector3(
                currentLookTarget.position.x,
                transform.position.y,  // 保持当前Y轴高度
                currentLookTarget.position.z
            );

            // 转向目标
            transform.LookAt(targetPosition);
        }

        // ========= 血条显示计时 =========
        if (showTime > 0)
        {
            showTime -= Time.deltaTime;
        }
        else if (hpBarRoot != null && hpBarRoot.gameObject.activeSelf)
        {
            hpBarRoot.gameObject.SetActive(false);
        }
    }

    void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            lookAtTarget = player.transform;
        }
    }



    void CreateHpBar()
    {
        if (hpBarPrefab == null) return;

        // 实例化
        GameObject hpObj = Instantiate(hpBarPrefab);
        // 唯一正确的 Fill 获取方式
        HpBar bar = hpObj.GetComponent<HpBar>();
        if (bar == null || bar.fill == null)
        {
            Debug.LogError(" HpBar 组件或 fill 未绑定！");
            return;
        }
        hpFill = bar.fill;
        // 直接挂到怪物身上
        hpObj.transform.SetParent(this.transform);

        // 本地位置（头顶）
        hpObj.transform.localPosition = new Vector3(0, 0f, 0);
        hpObj.transform.localRotation = Quaternion.identity;
        hpObj.transform.localScale = Vector3.one;

        hpBarRoot = hpObj.transform;
        hpBarRoot.gameObject.SetActive(false);
        //测试显示与否  UI / World Space / 挂载全部是 OK 的
        //hpBarRoot.gameObject.SetActive(true);
        //showTime = 999f; // 防止被 Update 隐藏
        UpdateHpUI();
    }


    void UpdateHpUI()
    {
        if (hpFill != null)
            hpFill.fillAmount = (float)hp / maxHp;
    }

    private void RandomPos()
    {
        if (randomPos == null || randomPos.Length == 0)
        {
            Debug.LogWarning($"{name} 没有配置 randomPos");
            return;
        }

        targetPos = randomPos[Random.Range(0, randomPos.Length)];
    }
    public override void Fire()
    {
        
        for (int i = 0; i < shootPos.Length; i++)
        {
            GameObject obj = Instantiate(bulletObj, shootPos[i].position, shootPos[i].rotation);
            //设置子弹拥有着
            BulletObj bullet = obj.GetComponent<BulletObj>();
            bullet.SetFather(this);
        }
    }
    public override void Dead()
    {
        if (monshitClip != null)
        {
            GameDataMgr.Instance.PlaySound(monshitClip);
        }
        ///上面是通过GamePanel的单例模式去获得的。。下面这个用UIManager去获得的。
        ///只是获得的方式不一样，写法出了问题

        if (hpBarRoot != null)
            hpBarRoot.gameObject.SetActive(false);

        //GameLevelMgr.Instance.AddScore(10);删掉后面统一处理

        // 获取分数值（根据你的怪物数据）
        int scoreValue = monsterScore;
        //在前面加10分后，这里就会特效完了之后再加10分，导致了两个10分的情况发生
        GameLevelMgr.Instance.AddScore(scoreValue);
        GamePanel gamePanel = UIManager.Instance.GetPanel<GamePanel>();
        if (gamePanel != null)
        {
            gamePanel.CreateScoreEffect(transform.position, scoreValue);
        }




        base.Dead();

    }

    public override void Wound(TankBaseObj other)
    {
        
        base.Wound(other);
        //设置显示血条的时间
        showTime = 3;

        if (hpBarRoot != null)
            hpBarRoot.gameObject.SetActive(true);

        UpdateHpUI();
    }




}
