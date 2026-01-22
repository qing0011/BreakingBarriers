using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MonsterTower : TankBaseObj
{
    // 寻路组件
    private NavMeshAgent agent;

    //攻击相关
    //间隔时间
    public float fireOffsetTime = 1;
    //记录累加时间，记录开火判断
    private float nowTime = 0;
    //发射位置
    public Transform[] shootPos;
    //关联子弹
    public GameObject bulletObj;


    //移动相关
   
    public float attackRange = 10f;
    private Transform target; // 攻击目标
    // 状态
    private bool isDead = false;


    [Header("血条预制体")]
    public GameObject hpBarPrefab;

    private Transform hpBarRoot;
    private Image hpFill;
    private float showTime = 0;

    private void Start()
    {
        // 初始化寻路组件
        agent = GetComponent<NavMeshAgent>();
        //hpFill = hpObj.transform.Find("Fill").GetComponent<Image>();


        // 设置寻路参数
        if (agent != null)
        {
            agent.speed = moveSpeed;
        }
        if (hpBarRoot != null)
            hpBarRoot.gameObject.SetActive(false);
        CreateHpBar();   // 这一行你漏掉了
        UpdateHpUI();
        // 寻找攻击目标（比如玩家的主塔）
        FindTarget();
    }
    private void FindTarget()
    {
        // 根据你的游戏逻辑寻找目标
        // 这里假设玩家主塔有 "Player" 标签
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;
            // 设置寻路目标
            if (agent != null && target != null)
            {
                agent.SetDestination(target.position);
            }
        }
    }
    void LateUpdate()
    {
        if (hpBarRoot == null) return;

        hpBarRoot.forward = Camera.main.transform.forward;
    }
    // Update is called once per frame
    void Update()
    {
        ////累加时间
        //nowTime += Time.deltaTime;
        ////时间超过间隔时间就开火
        //if(nowTime >= fireOffsetTime)
        //{
        //    Fire();
        //    nowTime = 0;
        //}
        if (isDead) return;
        // 如果找到了目标，检查距离
        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);

            // 如果在攻击范围内，停止移动并攻击
            if (distance <= attackRange)
            {
                // 停止移动
                if (agent != null)
                    agent.isStopped = true;

                // 攻击逻辑（原有逻辑）
                nowTime += Time.deltaTime;
                if (nowTime >= fireOffsetTime)
                {
                    Fire();
                    nowTime = 0;
                }
            }
            else
            {
                // 继续移动
                if (agent != null && !agent.isStopped)
                {
                    agent.SetDestination(target.position);
                }
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

    void CreateHpBar()
    {
        if (hpBarPrefab == null) return;

        // 实例化
        GameObject hpObj = Instantiate(hpBarPrefab);

        // 唯一正确的 Fill 获取血条方式
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
        Debug.Log($"HP = {hp} / {maxHp}");
        Debug.Log(hpFill.name);

    }

    public override void Fire()
    {
        for (int i = 0; i < shootPos.Length; i++)
        {
            //实例化子弹
            GameObject obj= Instantiate(bulletObj, shootPos[i].position, shootPos[i].rotation);
            BulletObj bullet = obj.GetComponent<BulletObj>();
            bullet.SetFather(this);
        }
    }
    // 修改 Wound 方法，现在怪物可以死亡了
    public override void Wound(TankBaseObj other)
    {
        if (isDead) return;
      
       
        // 计算伤害
        int dmg = other.atk - this.def;
        if (dmg <= 0) return;

        // 减血
        this.hp -= dmg;

        // 播放受伤动画
       

        // 检查死亡
        if (this.hp <= 0)
        {
            this.hp = 0;
            Dead();
        }
        showTime = 3f;
        if (hpBarRoot != null)
            hpBarRoot.gameObject.SetActive(true);
        UpdateHpUI();
        Debug.Log($"怪物受伤：{dmg}，当前HP：{hp}/{maxHp}");
    }

    // 修改 Dead 方法
    public override void Dead()
    {
        if (isDead) return;
        isDead = true;

        // 停止移动
        if (agent != null)
            agent.isStopped = true;

        // 播放死亡动画
       

     

        // 等待动画播放后销毁
        StartCoroutine(DestroyAfterAnimation());
    }

    private IEnumerator DestroyAfterAnimation()
    {
        // 等待2秒让死亡动画播放
        yield return new WaitForSeconds(2f);

        // 播放死亡特效（如果有）
        if (deadEff != null)
        {
            Instantiate(deadEff, transform.position, transform.rotation);
        }

        // 销毁对象
        Destroy(gameObject);
    }

    // 动画事件 - 用于同步攻击（可选）
    public void OnAttackAnimationEvent()
    {
        // 如果需要在动画特定帧触发攻击逻辑
        // 这里可以调用 Fire() 方法
    }
}
