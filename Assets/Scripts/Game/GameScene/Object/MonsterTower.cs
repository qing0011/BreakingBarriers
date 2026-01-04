using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterTower : TankBaseObj
{
    //间隔时间
    public float fireOffsetTime = 1;
    //记录累加时间，记录开火判断
    private float nowTime = 0;
    //发射位置
    public Transform[] shootPos;
    //关联子弹
    public GameObject bulletObj;
  

    // Update is called once per frame
    void Update()
    {
        //累加时间
        nowTime += Time.deltaTime;
        //时间超过间隔时间就开火
        if(nowTime >= fireOffsetTime)
        {
            Fire();
            nowTime = 0;
        }
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
    public override void Wound(TankBaseObj other)
    {
        //重写之后抛弃这个方法就永远不会死亡
    }
}
