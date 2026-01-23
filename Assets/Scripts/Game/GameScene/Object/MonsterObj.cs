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


    [Header("血条预制体")]
    public GameObject hpBarPrefab;

    private Transform hpBarRoot;
    private Image hpFill;




    void Start()
    {
        RandomPos();

        if (hpBarRoot != null)
            hpBarRoot.gameObject.SetActive(false);
        CreateHpBar();   // 这一行你漏掉了
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

        // ========= 巡逻移动（必须先判空） =========

        // 决定朝向的目标
        Transform currentLookTarget = null;

        if (lookAtTarget != null)
        {
            float dis = Vector3.Distance(transform.position, lookAtTarget.position);
            if (dis <= fireDis)
            {
                currentLookTarget = lookAtTarget;

                nowTime += Time.deltaTime;
                if (nowTime >= fireOffsetTime)
                {
                    Fire();
                    nowTime = 0;
                }
            }
            else
            {
                // 追逐玩家时，也应该朝玩家看
                currentLookTarget = lookAtTarget;
            }
        }

        // 如果没有玩家目标，看巡逻点
        if (currentLookTarget == null && targetPos != null)
        {
            currentLookTarget = targetPos;
        }

        // 统一朝向处理
        if (currentLookTarget != null)
        {
            // 只改变Y轴旋转，保持怪物在地面上
            Vector3 targetPosition = new Vector3(
                currentLookTarget.position.x,
                transform.position.y-0.2f,//玩家的Y轴向下移动因为玩家缩小一倍/////////////////////////重点回头可能还会修改/////////
                currentLookTarget.position.z
            );
            transform.LookAt(targetPosition);
        }

        // 移动逻辑（只有巡逻时才移动）
        if (lookAtTarget == null && targetPos != null)
        {
            // 使用本地坐标移动
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime, Space.Self);

            if (Vector3.Distance(transform.position, targetPos.position) < 0.5f)
            {
                RandomPos();
            }
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
        ///上面是通过GamePanel的单例模式去获得的。。下面这个用UIManager去获得的。
        ///只是获得的方式不一样，写法出了问题

        if (hpBarRoot != null)
            hpBarRoot.gameObject.SetActive(false);

        GameLevelMgr.Instance.AddScore(10);
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
