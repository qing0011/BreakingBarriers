using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FailPanel : BasePanel
{
    public Button btnBack;
    public Button btnGoOn;

    private int currentScore; 

    public override void Init()
    {
        btnBack.onClick.RemoveAllListeners();
        btnBack.onClick.AddListener(() =>
        {
            GameDataMgr.Instance.TryRefreshMaxScore(currentScore);
            //取消暂停
            Time.timeScale = 1f;
            
            //跳转场景
            SceneManager.LoadScene("BeginScene");
            UIManager.Instance.HidePanel<FailPanel>();    
        });
        btnGoOn.onClick.RemoveAllListeners();
        btnGoOn.onClick.AddListener(() =>
        {
            
            Time.timeScale = 1f;
           
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            UIManager.Instance.HidePanel<FailPanel>();

        });
       
     
    }
    public void SetScore(int score)
    {
        currentScore = score;
    }

}
