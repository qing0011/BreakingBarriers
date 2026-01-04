using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndPoint_End : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("进入触发器: " + other.name);
        if (other.CompareTag("Player"))
        {
            Debug.Log("检测到玩家进入终点！");
            if (WinPanel.Instance != null)
            {
                WinPanel.Instance.ShowMe();
            }
            else
            {
                Debug.LogWarning("WinPanel 已经被销毁，无法显示。");
            }
            //通关逻辑
            //打开胜利界面
            //WinPanel.Instance.ShowMe();
            
            Time.timeScale = 0;

        }
    }
}
