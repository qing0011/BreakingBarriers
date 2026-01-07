using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_PropType
{
    Atk,
    Def,
    MaxHp,
    Hp,
}
public class PropReward : MonoBehaviour
{
   public E_PropType type = E_PropType.Atk;

    //默认添加的值 获取道具后
    public int changeValue = 2;
    //音效
    public GameObject getEff;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerObj player = other.GetComponent<PlayerObj>();

            switch (type)
            {
                case E_PropType.Atk:
                    player.atk += changeValue;
                    break;
                case E_PropType.Def:
                    player.def += changeValue;
                    break;
                case E_PropType.MaxHp:
                    player.maxHp += changeValue;
                    //更新血条
                   // GamePanel.Instance.UpdateHP(player.maxHp, player.hp);
                    break;
                case E_PropType.Hp:
                    player.hp += changeValue;
                    //不能超过上限
                    if (player.hp > player.maxHp)
                        player.hp = player.maxHp;
                    //更新血条
                   // GamePanel.Instance.UpdateHP(player.maxHp, player.hp);
                    break;
            }
        }
        
        //播放奖励特效
        GameObject eff = Instantiate(getEff, this.transform.position, this.transform.rotation);
        //控制及获取音效
        AudioSource audioS = eff.GetComponent<AudioSource>();
        audioS.volume = GameDataMgr.Instance.musicData.soundValue;
        audioS.mute = !GameDataMgr.Instance.musicData.soundOpen;
        Destroy(this.gameObject);
    }
    
}
