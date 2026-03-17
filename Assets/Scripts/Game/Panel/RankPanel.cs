using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class RankPanel : BasePanel
{
    public Button btnClose;

    private List<TMP_Text> labName = new List<TMP_Text>();
    private List<TMP_Text> labScore = new List<TMP_Text>();
    private List<TMP_Text> labTime = new List<TMP_Text>();

    // 添加一个标志，表示UI组件是否初始化成功
    private bool isUIAvailable = false;

    public override void Init()
    {
        try
        {
            // 清空列表，防止重复添加
            labName.Clear();
            labScore.Clear();
            labTime.Clear();

            // 查找UI组件并验证
            bool allFound = true;

            for (int i = 1; i <= 10; i++)
            {
                // 查找Name文本
                Transform nameTrans = this.transform.Find("Name/labName" + i);
                if (nameTrans != null)
                {
                    TMP_Text nameText = nameTrans.GetComponent<TMP_Text>();
                    if (nameText != null)
                    {
                        labName.Add(nameText);
                    }
                    else
                    {
                        Debug.LogError($"找不到 Name/labName{i} 的 TMP_Text 组件");
                        allFound = false;
                    }
                }
                else
                {
                    Debug.LogError($"找不到 Name/labName{i} 对象");
                    allFound = false;
                }

                // 查找Score文本
                Transform scoreTrans = this.transform.Find("Score/labScore" + i);
                if (scoreTrans != null)
                {
                    TMP_Text scoreText = scoreTrans.GetComponent<TMP_Text>();
                    if (scoreText != null)
                    {
                        labScore.Add(scoreText);
                    }
                    else
                    {
                        Debug.LogError($"找不到 Score/labScore{i} 的 TMP_Text 组件");
                        allFound = false;
                    }
                }
                else
                {
                    Debug.LogError($"找不到 Score/labScore{i} 对象");
                    allFound = false;
                }

                // 查找Time文本
                Transform timeTrans = this.transform.Find("Time/labTime" + i);
                if (timeTrans != null)
                {
                    TMP_Text timeText = timeTrans.GetComponent<TMP_Text>();
                    if (timeText != null)
                    {
                        labTime.Add(timeText);
                    }
                    else
                    {
                        Debug.LogError($"找不到 Time/labTime{i} 的 TMP_Text 组件");
                        allFound = false;
                    }
                }
                else
                {
                    Debug.LogError($"找不到 Time/labTime{i} 对象");
                    allFound = false;
                }
            }

            isUIAvailable = allFound && labName.Count == 10 && labScore.Count == 10 && labTime.Count == 10;

            if (!isUIAvailable)
            {
                Debug.LogError("排行榜UI初始化失败，请检查场景中的对象命名和层级");
            }

            // 关闭按钮事件
            btnClose.onClick.RemoveAllListeners();
            btnClose.onClick.AddListener(() =>
            {
                UIManager.Instance.HidePanel<RankPanel>();
            });

            // 不要在Init中隐藏面板
            // UIManager.Instance.HidePanel<RankPanel>();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"RankPanel初始化异常: {e.Message}");
            isUIAvailable = false;
        }
    }

    public override void ShowMe()
    {
        base.ShowMe();

        // 检查UI是否可用
        if (!isUIAvailable)
        {
            Debug.LogError("UI组件未就绪，无法显示排行榜");
            // 可以选择显示错误提示或自动关闭
            StartCoroutine(ShowErrorAndClose());
            return;
        }

        UpdatePanelInfo();
    }

    private IEnumerator ShowErrorAndClose()
    {
        // 等待一帧让UI显示
        yield return null;

        // 显示错误提示（如果你有提示面板的话）
        // UIManager.Instance.ShowPanel<MessagePanel>("排行榜加载失败");

        // 3秒后自动关闭
        yield return new WaitForSeconds(3f);
        UIManager.Instance.HidePanel<RankPanel>();
    }

    public void UpdatePanelInfo()
    {
        // 安全检查1：UI组件是否可用
        if (!isUIAvailable)
        {
            Debug.LogError("UI组件未初始化，无法更新排行榜");
            return;
        }

        // 安全检查2：GameDataMgr是否存在
        if (GameDataMgr.Instance == null)
        {
            Debug.LogError("GameDataMgr.Instance 为空");
            return;
        }

        // 安全检查3：rankData是否存在
        if (GameDataMgr.Instance.rankData == null)
        {
            Debug.LogError("rankData 为空");
            return;
        }

        // 安全检查4：排行榜列表是否存在
        List<RankInfo> list = GameDataMgr.Instance.rankData.list;
        if (list == null)
        {
            Debug.LogError("排行榜列表为空");
            return;
        }

        // 先清空所有显示
        for (int i = 0; i < labName.Count; i++)
        {
            if (labName[i] != null) labName[i].text = "";
            if (labScore[i] != null) labScore[i].text = "";
            if (labTime[i] != null) labTime[i].text = "";
        }

        // 显示排行榜数据
        int count = Mathf.Min(list.Count, labName.Count);
        for (int i = 0; i < count; i++)
        {
            try
            {
                // 名字
                if (labName[i] != null)
                    labName[i].text = list[i].name ?? "";

                // 分数
                if (labScore[i] != null)
                    labScore[i].text = list[i].score.ToString();

                // 时间
                if (labTime[i] != null)
                {
                    int time = (int)list[i].time;
                    labTime[i].text = FormatTime(time);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"更新第{i}条排行榜数据时出错: {e.Message}");
            }
        }
    }

    // 格式化时间的辅助方法
    private string FormatTime(int totalSeconds)
    {
        if (totalSeconds <= 0)
            return "0秒";

        List<string> parts = new List<string>();

        int hours = totalSeconds / 3600;
        if (hours > 0)
        {
            parts.Add(hours + "时");
        }

        int minutes = totalSeconds % 3600 / 60;
        if (minutes > 0 || hours > 0)
        {
            parts.Add(minutes + "分");
        }

        int seconds = totalSeconds % 60;
        parts.Add(seconds + "秒");

        return string.Join("", parts);
    }
}