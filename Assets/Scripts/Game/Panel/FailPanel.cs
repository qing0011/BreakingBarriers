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
            GameDataMgr.Instance.TryRefreshTotalScoreData();
            //取消暂停
            Time.timeScale = 1f;
            
            //跳转场景
            SceneManager.LoadScene("BeginScene");
            UIManager.Instance.HidePanel<FailPanel>();    
        });
        btnGoOn.onClick.RemoveAllListeners();
        btnGoOn.onClick.AddListener(() =>
        {
            bool success = GameDataMgr.Instance.BuyContinueScoreData();
            if (success)
            {
                //复活玩家
                //恢复少量血量 or 重置位置
                Time.timeScale = 1f;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                UIManager.Instance.HidePanel<FailPanel>();
            }
            else
            {
                //提示积分不足
                Debug.Log("积分不足，无法继续");
               UIManager.Instance.ShowPanel<TipPanel>();
            }
           
           
           

        });
       
     
    }
    public void SetScore(int score)
    {
        currentScore = score;
    }

}
