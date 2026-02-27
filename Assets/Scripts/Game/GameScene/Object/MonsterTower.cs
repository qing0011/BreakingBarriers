using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MonsterTower : TankBaseObj
{
    // Ѱ·���
    private NavMeshAgent agent;

    //�������
    //���ʱ��
    public float fireOffsetTime = 1;
    //��¼�ۼ�ʱ�䣬��¼�����ж�
    private float nowTime = 0;
    //����λ��
    public Transform[] shootPos;
    //�����ӵ�
    public GameObject bulletObj;

    private Camera mainCam;


    //�ƶ����

    public float attackRange = 10f;
    private Transform target; // ����Ŀ��
    // ״̬
    private bool isDead = false;

    public int monsterTowerScore = 20;

    [Header("Ѫ��Ԥ����")]
    public GameObject hpBarPrefab;

    [Header("�ȼ���ʾ")]
    public int monsterLevel = 1; // ����ȼ�
    public Color safeColor = Color.green; // �ȼ��������ʱ����ɫ
    public Color dangerColor = Color.red; // �ȼ��������ʱ����ɫ

    private Text levelText; // �ȼ��ı�

    private Transform hpBarRoot;
    private Image hpFill;
    private float showTime = 0;
   [Header("������Ч")]
    public AudioClip monsterHitClip;
    private void Start()
    {
        // ��ʼ��Ѱ·���
        agent = GetComponent<NavMeshAgent>();
        //hpFill = hpObj.transform.Find("Fill").GetComponent<Image>();

        mainCam = Camera.main;

        // ����Ѱ·����
        if (agent != null)
        {
            agent.speed = moveSpeed;
        }
      
        CreateHpBar();   // ����Ѫ��
     
        UpdateHpUI();
      
        // Ѱ�ҹ���Ŀ�꣨������ҵ�������
        FindTarget();
    }
    private void FindTarget()
    {
        // ���������Ϸ�߼�Ѱ��Ŀ��
        // ���������������� "Player" ��ǩ
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;
            // ����Ѱ·Ŀ��
            if (agent != null && target != null)
            {
                agent.SetDestination(target.position);
            }
        }
    }
    void LateUpdate()
    {
        //if (hpBarRoot == null) return;

        //hpBarRoot.forward = Camera.main.transform.forward;
        if (hpBarRoot == null || mainCam == null) return;
        hpBarRoot.forward = mainCam.transform.forward;

    }
    // Update is called once per frame
    void Update()
    {
       
        if (isDead) return;
        if (target == null)
        {
            FindTarget();
            return;
        }
        // ����ҵ���Ŀ�꣬������
        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);

            // ����ڹ�����Χ�ڣ�ֹͣ�ƶ�������
            if (distance <= attackRange)
            {
                // ֹͣ�ƶ�
                if (agent != null)
                    agent.isStopped = true;

                // �����߼���ԭ���߼���
                nowTime += Time.deltaTime;
                if (nowTime >= fireOffsetTime)
                {
                    Fire();
                    nowTime = 0;
                }
            }
            else
            {
                // �����ƶ�
                if (agent != null && !agent.isStopped)
                {
                    agent.isStopped = false;
                    agent.SetDestination(target.position);
                }
            }
        }

        // ========= Ѫ����ʾ��ʱ =========
        if (showTime > 0)
        {
            showTime -= Time.deltaTime;
        }
        else if (hpBarRoot != null && hpBarRoot.gameObject.activeSelf)
        {
            hpBarRoot.gameObject.SetActive(false);
        }
    }
    // ����Ѫ��UI
    void CreateHpBar()
    {
        if (hpBarPrefab == null) return;

        // ʵ����
        GameObject hpObj = Instantiate(hpBarPrefab);

        // Ψһ��ȷ�� Fill ��ȡѪ����ʽ
        HpBar bar = hpObj.GetComponent<HpBar>();
        if (bar == null || bar.fill == null)
        {
           // Debug.LogError(" HpBar ����� fill δ�󶨣�");
            return;
        }
        hpFill = bar.fill;

        // ֱ�ӹҵ���������
        hpObj.transform.SetParent(this.transform);

        // ����λ�ã�ͷ����
        hpObj.transform.localPosition = new Vector3(0, 0f, 0);
        hpObj.transform.localRotation = Quaternion.identity;
        hpObj.transform.localScale = Vector3.one;

        hpBarRoot = hpObj.transform;
        hpBarRoot.gameObject.SetActive(false);
        //������ʾ���  UI / World Space / ����ȫ���� OK ��
        //hpBarRoot.gameObject.SetActive(true);
        //showTime = 999f; // ��ֹ�� Update ����
        UpdateHpUI();
    }

    void UpdateHpUI()
    {
        if (hpFill != null)
            hpFill.fillAmount = (float)hp / maxHp;
        //Debug.Log($"HP = {hp} / {maxHp}");
        //Debug.Log(hpFill.name);

    }


    public override void Fire()
    {
        for (int i = 0; i < shootPos.Length; i++)
        {
            //ʵ�����ӵ�
            GameObject obj= Instantiate(bulletObj, shootPos[i].position, shootPos[i].rotation);
            BulletObj bullet = obj.GetComponent<BulletObj>();
            bullet.SetFather(this);
        }
    }
    // �޸� Wound ���������ڹ������������
    public override void Wound(TankBaseObj other)
    {
        if (isDead) return;
      
       
        // �����˺�
        int dmg = other.atk - this.def;
        if (dmg <= 0) return;

        // ��Ѫ
        this.hp -= dmg;

        // �������˶���
       

        // �������
        if (this.hp <= 0)
        {
            this.hp = 0;
            Dead();
        }
        showTime = 3f;
        if (hpBarRoot != null)
            hpBarRoot.gameObject.SetActive(true);
        UpdateHpUI();
       // Debug.Log($"�������ˣ�{dmg}����ǰHP��{hp}/{maxHp}");
    }

    // �޸� Dead ����
    public override void Dead()
    {
        if (isDead) return;
        isDead = true;

        // ֹͣ�ƶ�
        if (agent != null)
            agent.isStopped = true;

        // ��ȡ����ֵ��������Ĺ������ݣ�
        int scoreValue = monsterTowerScore;
        GamePanel gamePanel = UIManager.Instance.GetPanel<GamePanel>();
        if (gamePanel != null)
        {
            gamePanel.CreateScoreEffect(transform.position, scoreValue);
        }



        // ������������




        // �ȴ��������ź�����
        StartCoroutine(DestroyAfterAnimation());
    }

    private IEnumerator DestroyAfterAnimation()
    {
        // �ȴ�2����������������
        yield return new WaitForSeconds(2f);

        // ����������Ч������У�
        if (deadEff != null)
        {
            Instantiate(deadEff, transform.position, transform.rotation);
        }

        if (monsterHitClip != null)
        {
            GameDataMgr.Instance.PlaySound(monsterHitClip);
        }
        // ���ٶ���
        Destroy(gameObject);
    }

    // �����¼� - ����ͬ����������ѡ��
    public void OnAttackAnimationEvent()
    {
        // �����Ҫ�ڶ����ض�֡���������߼�
        // ������Ե��� Fire() ����
    }


}
