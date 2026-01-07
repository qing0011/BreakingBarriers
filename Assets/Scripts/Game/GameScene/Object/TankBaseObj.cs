using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TankBaseObj : MonoBehaviour
{
    public int atk;
    public int def;
    public int maxHp;
    public int hp;
    //所有坦克 都有炮台相关
    public Transform tankHead;
    //移动速度相关
    public float moveSpeed = 10;
    public float roundSpeed = 100;
    public float headRandSpeed = 100;

    //死亡特效
    public GameObject deadEff;

    //三个共用方法，写在共用类里:开火，受伤，死亡

    //用抽象方法写开火。（子类重写方法即可。因为每个开火的逻辑不同）
    public abstract void Fire();

    //虚函数写受伤和死亡。因为需要有同样的逻辑主体

    public virtual void Wound(TankBaseObj other)
    {
        int dmg = other.atk - this.def;
        //伤害大于0 减血，伤害小于等于0死亡
        if (dmg <= 0)
        {
            return;
        }
       ///////////////// int before = this.hp;//////////////////////
        //如果伤害大于0 就应该减血//////////////////////////////////////////
        this.hp -= dmg;

        if (this.hp <= 0)
        {
            this.hp = 0;
            this.Dead();
        }
    }
    public virtual void Dead()
    {
        //对象死亡，消除对象
        Destroy(this.gameObject);
        //死亡对象的音效和特效
        if (deadEff != null)
        {
            //死亡对象的位置
            GameObject effObj = Instantiate(deadEff,this.transform.position,this.transform.rotation);
           //音效定位在特效身上，需要设置特效的位置大小
            AudioSource audioSource = effObj.GetComponent<AudioSource>();
            //音效大小设置
            audioSource.volume = GameDataMgr.Instance.musicData.soundValue;
            //音效是否播放设置
            audioSource.mute = !GameDataMgr.Instance.musicData.soundOpen;

            audioSource.Play();
        }
    }
}
