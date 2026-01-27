using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObj : TankBaseObj
{
    //当前装备的武器
    public WeaponObj nowWeapon;
    //武器父对象的位置
    public Transform weaponPos;

    private GamePanel cachedGamePanel;

    void Start()
    {
        // 缓存UI引用
        cachedGamePanel = FindObjectOfType<GamePanel>();
    }
    void Update()
    {
        //1，ws控制前进后退

        //1，transform位移
        //2，Input轴向输入检测
        float v = Input.GetAxis("Vertical");
        this.transform.Translate(v* Vector3.forward * moveSpeed * Time.deltaTime);
        // 只要 WS 被按住，就尝试开火
        //if (Mathf.Abs(v) > 0.01f)
        //{
        //    Fire();
        //}
        //2，ad控制旋转

        //1，transform旋转
        //2，Input轴向输入检测
        this.transform.Rotate(Input.GetAxis("Horizontal") *Vector3.up *roundSpeed* Time.deltaTime);

        //鼠标左右移动 控制炮台旋转
    
        //1，transform旋转
        //2，Input鼠标轴向输入检测
        this.transform.Rotate(Input.GetAxis("Mouse X") * Vector3.up * headRandSpeed * Time.deltaTime);
        //4，开火 Input
        if (Mathf.Abs(v) > 0.01f || Input.GetMouseButton(0))
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
       // UIManager.Instance.ShowPanel<FailPanel>();
        if (UIManager.Instance.ShowPanel<FailPanel>())
        {
            return;
        }
        else
        {
            UIManager.Instance.ShowPanel<LevelCompletePanel>();
        }



    }
    public override void Wound(TankBaseObj other)
    {
        base.Wound(other);

        int dmg = Mathf.Max(1, other.atk - this.def);
        //Debug.Log($"计算伤害: {other.atk} - {this.def} = {dmg}");

        // 伤害<=0的特殊情况：立即死亡（比如秒杀技能）
        if (dmg <= 0)
        {
           // Debug.LogWarning($"伤害为{dmg}，触发即死效果！");
            this.hp = 0; // 关键：先把HP设为0
            this.Dead(); // 然后死亡

            // 更新UI
            GamePanel gamePanel = FindObjectOfType<GamePanel>();
            if (gamePanel != null)
            {
                gamePanel.UpdateHP(this.maxHp, this.hp);
            }
            return; // 直接返回，不执行后面的代码
        }

        // 正常伤害处理
        int beforeHP = this.hp;
        this.hp -= dmg;
       // Debug.Log($"受伤: {beforeHP} -> {this.hp} (-{dmg})");

        if (this.hp <= 0)
        {
            this.hp = 0;
           // Debug.Log("玩家死亡!");
            this.Dead();
        }

        // 更新UI
        GamePanel gamePanel2 = FindObjectOfType<GamePanel>();
        if (gamePanel2 != null)
        {
            gamePanel2.UpdateHP(this.maxHp, this.hp);
        }
    }
    public void OnPickWeapon(int weaponId)
    {
        //写入“持久数据”
        GameDataMgr.Instance.playerData.weaponId = weaponId;

        //立刻换外观 + 功能
        ChangeWeaponById(weaponId);
    }

    public void ChangeWeapon(GameObject weapon, int weaponId)
    {


        //删除当前拥有的物体
        if (nowWeapon != null)
        {
            Destroy(nowWeapon.gameObject);
            //nowWeapon = null;
        }

        //切换武器
        //创建出武器设置他的父对象 并且保证缩放没有问题
        GameObject weaponObj = Instantiate(weapon, weaponPos, false);
        nowWeapon = weaponObj.GetComponent<WeaponObj>();

        //设置武器拥有者
        nowWeapon.SetFather(this);

        // 关键：写回数据层
        GameDataMgr.Instance.playerData.weaponId = weaponId;

        //Debug.Log("【装备武器】weaponId = " + weaponId);

    }
    public void ChangeWeaponById(int weaponId)
    {
        if (weaponId < 0) return;

        if (weaponId >= GameDataMgr.Instance.weaponPrefabList.Count)
        {
           // Debug.LogError("武器ID越界：" + weaponId);
            return;
        }

        GameObject prefab = GameDataMgr.Instance.weaponPrefabList[weaponId];
        ChangeWeapon(prefab, weaponId);
    }

    public void ApplyRuntimeData()
    {
        var data = GameDataMgr.Instance.playerData;

        // 同步属性
        this.atk = data.attack;
        this.def = data.defense;
        this.maxHp = data.maxHp;
        this.hp = data.hp;

       // Debug.Log($"【玩家同步】攻击={atk} 防御={def} HP={hp}/{maxHp}");
    }

    public void PickWeapon(int weaponId)
    {
        // 写入持久数据（这是继承的根）
        GameDataMgr.Instance.playerData.weaponId = weaponId;
        // 换枪时，直接补满子弹
        GameDataMgr.Instance.playerData.bulletCount = 9999;
        //  立刻装备
        ChangeWeaponById(weaponId);

       // Debug.Log("【拾取武器】weaponId = " + weaponId);
    }

}
