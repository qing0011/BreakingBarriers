using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class BeginPanel : BasePanel
{
    
    public Button btnBegin;
    public Button btnSetting;
    public Button btnSignIn;
    public Button btnHome;
    public Button btnRank;
    public Button btnEmail;
    public TextMeshProUGUI BestScore;

    public TextMeshProUGUI totalScore;
    //测试游戏
    public Button btnResetScore;
    public Button btnResetTotalScore;


    public override void Init()
    {

        btnResetTotalScore.onClick.RemoveAllListeners();
        btnResetTotalScore.onClick.AddListener(() =>
        {

            //GameDataMgr.Instance.ResetMaxScore();

            //  重置累计积分
            GameDataMgr.Instance.ResetTotalScore();
            SetTatalScore(GameDataMgr.Instance.scoreData.haveScore);
        });

        btnResetScore.onClick.RemoveAllListeners();
        btnResetScore.onClick.AddListener(() =>
        {

            GameDataMgr.Instance.ResetMaxScore();

            //  显示最高积分
            SetBestScore(GameDataMgr.Instance.scoreData.maxScore);
        });

        //锁定Game试图
        Cursor.lockState = CursorLockMode.Confined;
        //开始按钮
        btnBegin.onClick.AddListener(() =>
        {
            
            GameDataMgr.Instance.currentSceneId = 1;

            // 检查场景数据是否加载完成
            if (GameDataMgr.Instance.sceneDataList == null || GameDataMgr.Instance.sceneDataList.Count == 0)
            {
                Debug.LogError("场景数据未加载完成");
                return;
            }

            SceneData first = GameDataMgr.Instance.sceneDataList
                .Find(s => s.id == 1);

            if (first == null)
            {
                Debug.LogError("找不到id为1的场景数据");
                return;
            }

            if (string.IsNullOrEmpty(first.sceneName))
            {
                Debug.LogError("场景名称为空");
                return;
            }

            SceneManager.LoadScene(first.sceneName);

            UIManager.Instance.HidePanel<BeginPanel>();
            GameDataMgr.Instance.ResetGameData();

        });
        //设置按钮
        btnSetting.onClick.AddListener(() =>
        {
            UIManager.Instance.ShowPanel<SettingPanel>();
        });

        ////主界面
        btnHome.onClick.AddListener(() =>
        {
            UIManager.Instance.ShowPanel<BeginPanel>();
        });

        //排行榜
        btnRank.onClick.AddListener(() =>
        {
             UIManager.Instance.ShowPanel<RankPanel>();

            //隐藏主界面panel
            UIManager.Instance.HidePanel<BeginPanel>();
        });
        //签到
        btnSignIn.onClick.AddListener(() =>
        {
            UIManager.Instance.ShowPanel<SignInPanel>();
        });
        // 重置游戏时间缩放，确保游戏速度正常
        Time.timeScale = 1.0f;

        int bestScore = GameDataMgr.Instance.scoreData.maxScore;
        SetBestScore(bestScore);
        int TotalScore = GameDataMgr.Instance.scoreData.haveScore;
        SetTatalScore(TotalScore);
    }
    public void SetBestScore(int basetScore)
    {

        BestScore.text = basetScore.ToString();
    }
    public void SetTatalScore(int TotalScore)
    {

        totalScore.text = TotalScore.ToString();
    }

}
