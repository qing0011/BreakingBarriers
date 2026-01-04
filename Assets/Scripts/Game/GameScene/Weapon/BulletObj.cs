using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletObj : MonoBehaviour
{
    public float moveSpeed = 50;//移动速度
    public TankBaseObj fatherObj;//发射父物体
    public GameObject effObj;//特效对象

    // Update is called once per frame
    void Update()
    {
       this.transform.Translate(Vector3.forward * moveSpeed*Time.deltaTime); 
    }
    //和别人碰撞触发
    private void OnTriggerEnter(Collider other)
    {
        //子弹射击到立方体会爆炸
        //子弹射击不同阵营会爆炸
        if (other.CompareTag("Cube")|| other.CompareTag("Player")&&fatherObj.CompareTag("Monster")||
             other.CompareTag("Monster") && fatherObj.CompareTag("Player"))
        {
            //判断是否受伤
            //里氏替换原则查看是否有坦克脚本在碰撞到的对象身上
            //通过父类获取
            TankBaseObj obj = other.GetComponent<TankBaseObj>();
            if(obj != null)
            {
                obj.Wound(fatherObj);
            }
            //挡子弹销毁时，创建一个爆炸特效
            GameObject eff = Instantiate(effObj, this.transform.position, this.transform.rotation);
            //修改音效的音量和开启状态
            AudioSource audios = eff.GetComponent<AudioSource>();
            audios.volume = GameDataMgr.Instance.musicData.soundValue;
            audios.mute = GameDataMgr.Instance.musicData.isOpenSound;
        }
        Destroy(this.gameObject);
    }
    //设置拥有着
    public void SetFather(TankBaseObj obj)
    {
        fatherObj = obj;
    }
}
