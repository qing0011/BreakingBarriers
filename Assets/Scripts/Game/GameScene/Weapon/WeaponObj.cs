using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponObj : MonoBehaviour
{
    public GameObject bullet;//用于实例化的子对象
    public Transform[] shootObj;//外部决定有几个发射位置
    public TankBaseObj fatherObj;//武器拥有者

    [Header("射击参数")]
    public float fireInterval = 0.2f;    // 两次射击间隔（秒）

    private float fireTimer = 0f;

    private void Update()
    {
        fireTimer += Time.deltaTime;
    }



    public void Fire()
    {
        // 射速限制
        if (fireTimer < fireInterval)
            return;

        fireTimer = 0f;

        //根据位置创建相应的子弹
        for (int i = 0; i < shootObj.Length; i++)
        {
            //创建子弹预制体
            GameObject obj = Instantiate(bullet, shootObj[i].position, shootObj[i].rotation);
            //控制子弹做什么
            BulletObj bulletObj = obj.GetComponent<BulletObj>();
            bulletObj.SetFather(fatherObj);

        }

    }
    public void SetFather(TankBaseObj obj)
    {
        fatherObj = obj;
    }
}
