using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletObj : MonoBehaviour
{
    public float moveSpeed = 50;//移动速度
    public TankBaseObj fatherObj;//发射父物体
    public GameObject effObj;//特效对象
    public AudioClip hitClip;
    // Update is called once per frame
    void Update()
    {
       this.transform.Translate(Vector3.forward * moveSpeed*Time.deltaTime); 
    }
    //和别人碰撞触发
    //private void OnTriggerEnter(Collider other)
    //{
    //    //子弹射击到立方体会爆炸
    //    //子弹射击不同阵营会爆炸
    //    if (
    //         other.CompareTag("Cube") ||
    //         (other.CompareTag("Player") && fatherObj.CompareTag("Monster")) ||
    //         (other.CompareTag("Monster") && fatherObj.CompareTag("Player"))
    //       )

    //    {
    //        //判断是否受伤
    //        //里氏替换原则查看是否有坦克脚本在碰撞到的对象身上
    //        //通过父类获取
    //        TankBaseObj obj = other.GetComponent<TankBaseObj>();
    //        if(obj != null)
    //        {
    //            obj.Wound(fatherObj);
    //        }
    //        //挡子弹销毁时，创建一个爆炸特效
    //        GameObject eff = Instantiate(effObj, this.transform.position, this.transform.rotation);
    //        //修改音效的音量和开启状态

    //        //播放“本物体专属音效”
    //        if (hitClip != null)
    //        {
    //            GameDataMgr.Instance.PlaySound(hitClip);
    //        }

    //    }
    //    Destroy(this.gameObject);
    //}
    //设置拥有着

       //和别人碰撞触发
    private void OnTriggerEnter(Collider other)
    {
        // ========== 安全检查 ==========
        // 检查碰撞对象是否有效
        if (other == null || other.gameObject == null) return;
        
        // 检查发射者是否已被销毁
        if (fatherObj == null || fatherObj.gameObject == null)
        {
            Debug.LogWarning("子弹的发射者已被销毁");
            Destroy(gameObject);
            return;
        }
        
        // ========== 碰撞检测 ==========
        bool shouldExplode = false;
        
        try
        {
            // 检查是否是立方体
            if (other.CompareTag("Cube"))
            {
                shouldExplode = true;
            }
            // 检查玩家打怪物
            else if (other.CompareTag("Monster") && fatherObj.CompareTag("Player"))
            {
                shouldExplode = true;
            }
            // 检查怪物打玩家
            else if (other.CompareTag("Player") && fatherObj.CompareTag("Monster"))
            {
                shouldExplode = true;
            }
        }
        catch (MissingReferenceException)
        {
            // 如果标签比较时报错，说明对象已被销毁
            Debug.Log("碰撞对象已被销毁");
            Destroy(gameObject);
            return;
        }
        
        // ========== 处理爆炸 ==========
        if (shouldExplode)
        {
            // 先创建特效（即使目标被销毁也显示特效）
            if (effObj != null)
            {
                GameObject eff = Instantiate(effObj, transform.position, transform.rotation);
            }
            
            // 播放音效
            if (hitClip != null && GameDataMgr.Instance != null)
            {
                GameDataMgr.Instance.PlaySound(hitClip);
            }
            
            // 尝试造成伤害（需要额外检查）
            try
            {
                if (other.gameObject != null)
                {
                    TankBaseObj obj = other.GetComponent<TankBaseObj>();
                    if (obj != null && obj.gameObject != null)
                    {
                        obj.Wound(fatherObj);
                    }
                }
            }
            catch (MissingReferenceException)
            {
                Debug.Log("目标在造成伤害前已被销毁");
            }
        }
        
        // 最后销毁子弹
        Destroy(gameObject);
    }
    public void SetFather(TankBaseObj obj)
    {
        fatherObj = obj;
    }
    // 在销毁前清理
    private void OnDestroy()
    {
        fatherObj = null;
    }
}
