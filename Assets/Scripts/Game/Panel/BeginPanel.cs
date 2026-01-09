using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class BeginPanel : BasePanel
{
    //声明变量
    public Button btnBegin;
    public Button btnSetting;
    public Button btnSignIn;
    public Button btnHome;
    public Button btnRank;
    public Button btnEmail;
    public TextMeshProUGUI BestScore;

    public Button btnResetScore;

    public override void Init()
    {
        btnResetScore.onClick.RemoveAllListeners();
        btnResetScore.onClick.AddListener(() =>
        {

            GameDataMgr.Instance.ResetMaxScore();

            //  立刻刷新 UI
            SetBestScore(GameDataMgr.Instance.scoreData.maxScore);
        });

        //在Game试图锁定鼠标
        //Cursor.lockState = CursorLockMode.Confined;
        //启动开始面板事件
        btnBegin.onClick.AddListener(() =>
        {
            GameDataMgr.Instance.ResetGameData();
            GameDataMgr.Instance.currentSceneId = 1;

            SceneData first = GameDataMgr.Instance.sceneDataList
                .Find(s => s.id == 1);

            SceneManager.LoadScene(first.sceneName);

            UIManager.Instance.HidePanel<BeginPanel>();
            GameDataMgr.Instance.ResetGameData();

        });
        //开启设置面板事件
        btnSetting.onClick.AddListener(() =>
        {
            UIManager.Instance.ShowPanel<SettingPanel>();
        });

        ////退出按钮事件
        btnHome.onClick.AddListener(() =>
        {
            UIManager.Instance.ShowPanel<BeginPanel>();
        });

        //排行榜按钮事件
        btnRank.onClick.AddListener(() =>
        {
             UIManager.Instance.ShowPanel<RankPanel>();

            //避免穿透 隐藏自己
            UIManager.Instance.HidePanel<BeginPanel>();
        });
        //开启签到面板事件
        btnSignIn.onClick.AddListener(() =>
        {
            UIManager.Instance.ShowPanel<SignInPanel>();
        });
        int bestScore = GameDataMgr.Instance.scoreData.maxScore;
        SetBestScore(bestScore);

    }
    public void SetBestScore(int basetScore)
    {

        BestScore.text = basetScore.ToString();
    }


}
