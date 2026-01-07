using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeObj : MonoBehaviour
{
    //奖励物品关联
    public GameObject[] rewardObject;
    //死亡特效预制体关联
    public GameObject deadEff;
    private void OnTriggerEnter(Collider other)
    {

        //子弹碰到就销毁 修改tag即可
        //达到自己随机奖励
        int regeInt = Random.Range(0,100);
        //在物体当前所在位置
        if(regeInt <50 )
        {
            regeInt = Random.Range(0, rewardObject.Length);
            Instantiate(rewardObject[regeInt],this.transform.position,this.transform.rotation);

        }
        //创建特效预制体
        GameObject effObj = Instantiate(deadEff, this.transform.position, this.transform.rotation);
        AudioSource audioS = effObj.GetComponent<AudioSource>();
        audioS.volume = GameDataMgr.Instance.musicData.soundValue;
        audioS.mute = !GameDataMgr.Instance.musicData.soundOpen;

        Destroy(this.gameObject);

    }
}
