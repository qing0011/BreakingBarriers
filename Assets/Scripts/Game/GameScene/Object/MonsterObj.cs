using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterObj : TankBaseObj
{
    //目标点
    private Transform targetPos;
    //随机点
    public Transform[] randomPos;
    //朝向的目标
    public Transform lookAtTarget;


    //开火距离
    public float fireDis=5;
    //攻击间隔时间
    public float fireOffsetTime = 1;


    //计时间
    private float nowTime = 0;
    //开火点
    public Transform[] shootPos;
    //子弹预制体
    public GameObject bulletObj;

    //关联两张血条图
    public Texture maxHpBK;
    public Texture hpBK;

    //显示血条计时用的时间
    private float showTime = 0;

    //Rect是结构体所以不需要new，后面直接赋值即可
    private Rect maxHpRect;
    private Rect hpRect;


    void Start()
    {
        RandomPos();
    }

    // Update is called once per frame
    void Update()
    {
        //看向自己的目标点
        this.transform.LookAt(targetPos);
        //向自己的目标点方向移动
        this.transform.Translate(Vector3.forward*moveSpeed*Time.deltaTime );
        //当距离过小时，认为达到目的地。开始随即下一个点。
        if(Vector3.Distance(this.transform.position,targetPos.position) < 0.05f)
        {
            RandomPos();

        }
        //看向自己的目标
        if(lookAtTarget != null )
        {
            tankHead.LookAt(lookAtTarget);
            //当自己和目标对象的距离小于等于配置的开火距离。
            if(Vector3.Distance(this.transform.position,lookAtTarget.position) <= fireDis)
            {
                nowTime += Time.deltaTime;
                if(nowTime >= fireOffsetTime)
                {
                    Fire();
                    nowTime = 0;
                }
                

            }
        }
    }
    private void RandomPos()
    {
        if (randomPos.Length == 0)
        {
            return;
        }
        targetPos = randomPos[Random.Range(0,randomPos.Length)];
    }
    public override void Fire()
    {
       for(int i = 0; i < shootPos.Length; i++)
        {
            GameObject obj = Instantiate(bulletObj, shootPos[i].position, shootPos[i].rotation);
            //设置子弹拥有着
            BulletObj bullet = obj.GetComponent<BulletObj>();
            bullet.SetFather(this);
        }
    }
    public override void Dead()
    {
       
        //死亡加分
        // 检查GamePanel.Instance是否不为null
        if (GamePanel.Instance != null)
        {
            GamePanel.Instance.AddScore(10);
        }
        // GamePanel.Instance.AddScore(10);// GamePanel.Instance.AddScore(10);
        base.Dead();
    }
    private void OnGUI()
    {
        if(showTime > 0)
        {
            //连续计时
            showTime-= Time.deltaTime;

            //绘制血条
            //1，怪物当前位置转换为屏幕位置
            //可以利用知识：摄像机里提供的API将世界坐标转换为屏幕坐标
            Vector3 screenPos = Camera.main.WorldToScreenPoint(this.transform.position);
            //2,屏幕坐标转换为GUI位置
            //知识点：如何得到当前屏幕的分辨率
            screenPos.y = Screen.height - screenPos.y;

            //开始绘制
            //知识点：GUI中的图片绘制
            //底图
            maxHpRect.x = screenPos.x - 50;
            maxHpRect.y = screenPos.y - 50;
            maxHpRect.width = 100;
            maxHpRect.height = 15;
            //画底图
            GUI.DrawTexture(maxHpRect, maxHpBK);

            hpRect.x = screenPos.x - 50;
            hpRect.y = screenPos.y - 50;
            //根据血量和最大血量的百分比 决定画多宽
            hpRect.width = (float)hp / maxHp * 100f;
            hpRect.height = 15;
            //画血条
            GUI.DrawTexture(hpRect, hpBK);

        }
    }
 
    public override void Wound(TankBaseObj other)
    {
        
        base.Wound(other);
        //设置显示血条的时间
        showTime = 3;
    }




}
