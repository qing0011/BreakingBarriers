using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("进入触发器: " + other.name);
        if (other.CompareTag("Player"))
        {
            Debug.Log("检测到玩家进入终点！");

            //通关逻辑
            //打开胜利界面
            SceneManager.LoadScene("GameScene01");
            // WinPanel.Instance.ShowMe();
           // Time.timeScale = 0;

        }
    }
}
