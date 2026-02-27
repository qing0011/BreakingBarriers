using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MonsterTower2 : TankBaseObj
{
    // 寻路组件
    private NavMeshAgent agent;

    // ================= Animator =================
    private Animator animator;
    private static readonly int HashAttack = Animator.StringToHash("Attack");
    private static readonly int HashDie = Animator.StringToHash("Die");
    private static readonly int HashIsMoving = Animator.StringToHash("IsMoving");

    ///攻击相关
    //间隔时间
    public float fireOffsetTime = 1;
    //记录累加时间，记录开火判断
    private float nowTime = 0;
    //发射位置
    public Transform[] shootPos;
    //关联子弹
    public GameObject bulletObj;


    private Camera mainCam;


    //移动相关

    public float attackRange = 10f;
    private Transform target; // 攻击目标
    // 状态
    private bool isDead = false;

    // ================= UI / 数值 =================
    public int monsterTowerScore = 20;

    [Header("血条预制体")]
    public GameObject hpBarPrefab;

    [Header("等级显示")]
    public int monsterLevel = 1; // 怪物等级
    public Color safeColor = Color.green; // 等级低于玩家时的颜色
    public Color dangerColor = Color.red; // 等级高于玩家时的颜色

    private Text levelText; // 等级文本

    private Transform hpBarRoot;
    private Image hpFill;
    private float showTime = 0;
   [Header("触发音效")]
    public AudioClip monsterHitClip;

    //生命周期
    private void Start()
    {
        // 初始化寻路组件
        agent = GetComponent<NavMeshAgent>();
    
        animator = GetComponent<Animator>();
        mainCam = Camera.main;

        // 设置寻路参数
        if (agent != null)
        {
            agent.speed = moveSpeed;
        }
      
        CreateHpBar();   // 创建血条
     
        UpdateHpUI();
      
        // 寻找攻击目标（比如玩家的主塔）
        FindTarget();
    }
    void LateUpdate()
    {
        if (hpBarRoot == null || mainCam == null) return;
        hpBarRoot.forward = mainCam.transform.forward;

    }
    void Update()
    {

        if (isDead) return;

        if (target == null)
        {
            FindTarget();
            return;
        }
        float distance = Vector3.Distance(transform.position, target.position);
            // 如果在攻击范围内，停止移动并攻击
            if (distance <= attackRange)
            {
                // 停止移动
              if (agent != null)
                    agent.isStopped = true;


            // 攻击前朝向玩家
            Vector3 lookDir = target.position - transform.position;
            lookDir.y = 0;
            transform.rotation = Quaternion.LookRotation(lookDir);

            animator.SetBool(HashIsMoving, false);
            // 攻击逻辑（原有逻辑）
            nowTime += Time.deltaTime;
                if (nowTime >= fireOffsetTime)
                {
                animator.SetTrigger(HashAttack);   // 触发攻击动画
                nowTime = 0;
                }
            }
            else
            {
                // 继续移动
                if (agent != null)
                {
                    agent.isStopped = false;
                    agent.SetDestination(target.position);
                }
            animator.SetBool(HashIsMoving, true);
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
    // ================= 攻击（动画事件调用） =================
    public override void Fire()
    {
        if (target == null) return;
        for (int i = 0; i < shootPos.Length; i++)
        {
            Vector3 dir = (target.position - shootPos[i].position).normalized;
            Quaternion rot = Quaternion.LookRotation(dir);
            //实例化子弹
            GameObject obj = Instantiate(bulletObj, shootPos[i].position, rot);
            BulletObj bullet = obj.GetComponent<BulletObj>();
            bullet.SetFather(this);

        }
    }
    // Animation Event（Attack 动画中调用）
    public void OnAttackAnimationEvent()
    {
        if (isDead) return;
        Fire();
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
        // Debug.Log($"怪物受伤：{dmg}，当前HP：{hp}/{maxHp}");
    }
    // 修改 Dead 方法
    public override void Dead()
    {
        if (isDead) return;
        isDead = true;

        // 停止移动
        if (agent != null)
            agent.isStopped = true;
        animator.SetTrigger(HashDie);   // 播放死亡动画
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
        // 获取分数值（根据你的怪物数据）
        int scoreValue = monsterTowerScore;
        GameLevelMgr.Instance.AddScore(scoreValue);
        GamePanel gamePanel = UIManager.Instance.GetPanel<GamePanel>();
        if (gamePanel != null)
        {
            gamePanel.CreateScoreEffect(transform.position, scoreValue);
        }

        // 播放死亡动画

        // 等待动画播放后销毁
        StartCoroutine(DestroyAfterAnimation());
    }

    private IEnumerator DestroyAfterAnimation()
    {
        yield return null;

        // 等进入 Die 状态
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Die"))
        {
            yield return null;
        }

        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);

        // 等 Die 动画真正播放完成
        yield return new WaitForSeconds(info.length);

        // 再等 1 秒
        yield return new WaitForSeconds(1f);

        Destroy(gameObject);
    }
    // 创建血条UI
    void CreateHpBar()
    {
        if (hpBarPrefab == null) return;

        // 实例化
        GameObject hpObj = Instantiate(hpBarPrefab);

        // 唯一正确的 Fill 获取血条方式
        HpBar bar = hpObj.GetComponent<HpBar>();
        if (bar == null || bar.fill == null)
        {
           // Debug.LogError(" HpBar 组件或 fill 未绑定！");
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
        //Debug.Log($"HP = {hp} / {maxHp}");
        //Debug.Log(hpFill.name);

    }

}
