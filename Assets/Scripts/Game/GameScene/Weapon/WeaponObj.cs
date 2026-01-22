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

    //很容易造成玩家没有子弹
    private float lastFireTime = -99f;

    public void Fire()
    {
        if (Time.time - lastFireTime < fireInterval)
            return;

        var data = GameDataMgr.Instance.playerData;
        if (data.bulletCount <= 0)
            return;

        lastFireTime = Time.time;

        foreach (var pos in shootObj)
        {
            if (data.bulletCount <= 0)
                break;

            var obj = Instantiate(bullet, pos.position, pos.rotation);
            obj.GetComponent<BulletObj>().SetFather(fatherObj);
            data.bulletCount--;
        }
    }

    public void SetFather(TankBaseObj obj)
    {
        fatherObj = obj;
    }
}
