using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using WeChatWASM;
using System.Runtime.InteropServices;

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

    public Button btnGame;

    public Button btnRetryGameCircle; // 在Inspector中赋值

    [DllImport("__Internal")]
    private static extern void OpenGameCircle();


    private void onGameClubButtonClick()
    {
        Debug.Log("尝试打开游戏圈 - 当前平台: " + Application.platform);

        // 按钮点击动画
        StartCoroutine(ButtonClickEffect());

#if UNITY_WEBGL && !UNITY_EDITOR
    try 
    {
        if (IsWeChatMiniGame() && !IsDeveloperTool())
        {
            Debug.Log("正在调用 OpenGameCircle()");
            ShowToast("正在打开游戏圈...");
            
            // 调用游戏圈
            OpenGameCircle();
            
            // 多次检查是否显示成功
            StartCoroutine(CheckGameCircleDisplayed());
        }
        else
        {
            ShowToast("请在小游戏中打开");
        }
    }
    catch (System.Exception e)
    {
        Debug.LogError("调用 OpenGameCircle 失败: " + e.Message);
        ShowToast("打开游戏圈失败");
    }
#else
        Debug.Log("游戏圈跳转（仅在微信小游戏中生效）");
        ShowToast("仅在微信小游戏中支持");
#endif
    }

    private IEnumerator CheckGameCircleDisplayed()
    {
        // 多次尝试检查
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(2f);
            Debug.Log($"第{i + 1}次检查游戏圈是否显示");

            // 提示用户
            ShowToast("游戏圈已打开，请查看");
        }
    }

    // 添加一个手动重试的方法
    public void RetryGameCircle()
    {
        Debug.Log("手动重试打开游戏圈");
        onGameClubButtonClick();
    }
    private bool IsWeChatMiniGame()
    {
        try
        {
            var systemInfo = WX.GetSystemInfoSync();
            Debug.Log("微信环境检查 - platform: " + systemInfo.platform);
            return !string.IsNullOrEmpty(systemInfo.platform);
        }
        catch
        {
            Debug.Log("不在微信环境中");
            return false;
        }
    }

    private bool IsDeveloperTool()
    {
        try
        {
            var systemInfo = WX.GetSystemInfoSync();
            bool isDevTool = systemInfo.platform == "devtools";
            Debug.Log("是否在开发者工具中: " + isDevTool);
            return isDevTool;
        }
        catch
        {
            return false;
        }
    }

    private void ShowToast(string message)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
            try 
            {
                WX.ShowToast(new ShowToastOption()
                {
                    title = message,
                    icon = "none",
                    duration = 1500
                });
            }
            catch { }
#else
        Debug.Log(message);
#endif
    }

    private IEnumerator ButtonClickEffect()
    {
        if (btnGame != null)
        {
            Vector3 originalScale = btnGame.transform.localScale;
            btnGame.transform.localScale = originalScale * 0.9f;
            yield return new WaitForSeconds(0.1f);
            btnGame.transform.localScale = originalScale;
        }
    }

    public override void Init()
    {
        btnRetryGameCircle.onClick.AddListener(() =>
        {
            RetryGameCircle();
        });
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
        //游戏圈按钮
        // 初始化按钮监听
        btnGame.onClick.RemoveAllListeners();
        btnGame.onClick.AddListener(() =>
        {
            onGameClubButtonClick();
        });

        ////主界面
        btnHome.onClick.AddListener(() =>
        {
            UIManager.Instance.ShowPanel<BeginPanel>();
        });

        //排行榜
        btnRank.onClick.AddListener(() =>
        {
            ShowToast("后续开发中，敬请期待");
            // UIManager.Instance.ShowPanel<RankPanel>();
            //return;
            //隐藏主界面panel
            // UIManager.Instance.HidePanel<BeginPanel>();
        });
        //邮件
        btnEmail.onClick.AddListener(() =>
        {
            ShowToast("后续开发中，敬请期待");
            // UIManager.Instance.ShowPanel<RankPanel>();
            //return;
            //隐藏主界面panel
            // UIManager.Instance.HidePanel<BeginPanel>();
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
