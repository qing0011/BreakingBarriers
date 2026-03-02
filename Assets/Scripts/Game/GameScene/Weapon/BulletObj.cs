using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletObj : MonoBehaviour
{
    public float moveSpeed = 50;//移动速度
    public TankBaseObj fatherObj;//子弹父物体
    public GameObject effObj;//特效物体
    public AudioClip hitClip;
    // Update is called once per frame
    void Update()
    {
       this.transform.Translate(Vector3.forward * moveSpeed*Time.deltaTime); 
    }
    //子弹发生碰撞时
    private void OnTriggerEnter(Collider other)
    {
        // ========== 安全检查 ==========
        // 检查碰撞对象是否有效
        if (other == null || other.gameObject == null) return;
        
        // 检查父对象是否已被销毁
        if (fatherObj == null || fatherObj.gameObject == null)
        {
            Debug.LogWarning("子弹的父对象已被销毁");
            Destroy(gameObject);
            return;
        }
        
        // ========== 碰撞过滤 ==========
        // 避免子弹与发射它的坦克发生碰撞
        if (other.gameObject == fatherObj.gameObject || other.transform.IsChildOf(fatherObj.transform))
        {
            return;
        }
        
        // ========== 碰撞判断 ==========
        bool shouldExplode = false;
        
        try
        {
            // 判断是否击中障碍物
            if (other.CompareTag("Cube"))
            {
                shouldExplode = true;
            }
            // 玩家打怪物
            else if (other.CompareTag("Monster") && fatherObj.CompareTag("Player"))
            {
                shouldExplode = true;
            }
            // 怪物打玩家
            else if (other.CompareTag("Player") && fatherObj.CompareTag("Monster"))
            {
                shouldExplode = true;
            }
        }
        catch (MissingReferenceException)
        {
            // 当标签比较时出现空引用异常
            Debug.Log("碰撞对象已被销毁");
            Destroy(gameObject);
            return;
        }
        
        // ========== 产生爆炸 ==========
        if (shouldExplode)
        {
            // 先创建特效对象使目标被击中时也能看到特效
            if (effObj != null)
            {
                GameObject eff = Instantiate(effObj, transform.position, transform.rotation);
            }
            
            // 播放音效
            if (hitClip != null && GameDataMgr.Instance != null)
            {
                GameDataMgr.Instance.PlaySound(hitClip);
            }
            
            // 处理目标受伤（如果需要的话）
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
                Debug.Log("目标对象受伤前已被销毁");
            }
        }
        
        // 销毁子弹
        Destroy(gameObject);
    }
    public void SetFather(TankBaseObj obj)
    {
        fatherObj = obj;
    }
    // 销毁前处理
    private void OnDestroy()
    {
        fatherObj = null;
    }
}