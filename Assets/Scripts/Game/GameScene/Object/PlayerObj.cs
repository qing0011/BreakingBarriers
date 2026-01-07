using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObj : TankBaseObj
{
    //当前装备的武器
    public WeaponObj nowWeapon;
    //武器父对象的位置
    public Transform weaponPos;

    // Update is called once per frame
    void Update()
    {
        //1，ws控制前进后退
        //知识点
        //1，transform位移
        //2，Input轴向输入检测
        this.transform.Translate(Input.GetAxis("Vertical") * Vector3.forward * moveSpeed * Time.deltaTime);

        //2，ad控制旋转
        //知识点
        //1，transform旋转
        //2，Input轴向输入检测
        this.transform.Rotate(Input.GetAxis("Horizontal") *Vector3.up *roundSpeed* Time.deltaTime);

        //鼠标左右移动 控制炮台旋转
        //知识点
        //1，transform旋转
        //2，Input鼠标轴向输入检测
        this.transform.Rotate(Input.GetAxis("Mouse X") * Vector3.up * headRandSpeed * Time.deltaTime);
        //4，开火 Input
        if (Input.GetMouseButtonDown(0))
        {
            Fire();
        }
        //特殊处理，，，重写父类行为
   
    //死亡

    //受伤
    }
    public override void Fire()
    {
        if(nowWeapon != null)
        {
            nowWeapon.Fire();
        }
    }
    public override void Dead()
    {
         base.Dead();

        //处理失败逻辑显示失败界面
        Time.timeScale = 0.1f;
        UIManager.Instance.ShowPanel<FailPanel>();
        

    }
    public override void Wound(TankBaseObj other)
    {
        base.Wound(other);

        // 获取GamePanel实例
        GamePanel gamePanel = FindObjectOfType<GamePanel>();
        if (gamePanel != null)
        {
            //更新主面板血条
            gamePanel.UpdateHP(this.maxHp, this.hp);
        }
    

    
    }
    public void ChangeWeapon(GameObject weapon)
    {
        //删除当前拥有的物体
        if(nowWeapon != null)
        {
            Destroy(nowWeapon.gameObject);
            nowWeapon = null;
        }

        //切换武器
        //创建出武器设置他的父对象 并且保证缩放没有问题
        GameObject weaponObj = Instantiate(weapon, weaponPos, false);
        nowWeapon = weaponObj.GetComponent<WeaponObj>();

        //设置武器拥有者
        nowWeapon.SetFather(this);
    
    }
}
