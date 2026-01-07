using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FailPanel : BasePanel
{
    public Button btnBack;
    public Button btnGoOn;
    

    public override void Init()
    {
        btnBack.onClick.AddListener(() =>
        {
            //取消暂停
           Time.timeScale = 1f;
            //跳转场景
            SceneManager.LoadScene("BeginScene");
            UIManager.Instance.HidePanel<FailPanel>();    
        });
        btnGoOn.onClick.AddListener(() =>
        {
            //ȡ����ͣ
            Time.timeScale = 1f;
            //�ٴ��л�����   �����ӿ�ͷ�浱ǰ�ؿ�  
            //������Ҫ���ߣ��������ӣ�
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            UIManager.Instance.HidePanel<FailPanel>();

        });
       
     
    }

 
}
