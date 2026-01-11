using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class RankPanel : BasePanel
{
    public Button btnClose;

    //关联对象（较多对象需要列表用代码查找
    //因为控件较多 拖的话 工作量太大了 我们直接偷懒 通过代码找
    
    private List<TMP_Text> labName = new List<TMP_Text>();
    private List<TMP_Text> labScore = new List<TMP_Text>();
    private List<TMP_Text> labTime = new List<TMP_Text>();
    /// <summary>
    /// /////////////////////////////////////////
    /// </summary>
    public Button btnClear;//轻触按键


    public override void Init()
    {
        //用/可以找到物体在场景中分组里面的分组路径
        //通过transform.Find找到子对象

        ///一定要注意i的初始值是多少。。。这里是1.。。报错的问题是我写了0
        for (int i = 1; i <= 10; i++)
        {


            //labPM.Add(this.transform.Find("PM/labPM" + i).GetComponent<CustomGUILabel>());
            labName.Add(this.transform.Find("Name/labName" + i).GetComponent<TMP_Text>());
            labScore.Add(this.transform.Find("Score/labScore" + i).GetComponent<TMP_Text>());
            labTime.Add(this.transform.Find("Time/labTime" + i).GetComponent<TMP_Text>());

        }

        //关闭按钮事件
        btnClose.onClick.RemoveAllListeners();
        btnClose.onClick.AddListener(() =>
        {
            UIManager.Instance.HidePanel<RankPanel>();
            //开启开始面板
            // UIManager.Instance.ShowPanel<BeginPanel>();
        });
       
        //////////////////////////////////////////////////////////////////////
        //清空按钮
        btnClear.onClick.RemoveAllListeners();
        btnClear.onClick.AddListener(() =>
        {
            GameDataMgr.Instance.rankData.list.Clear();
            //GameDataMgr.Instance.Save();
            // 更新UI
            UpdatePanelInfo();
        });
       
        ////////////////////////////////////////////////////////
        UIManager.Instance.HidePanel<RankPanel>();
    }
  

    public override void ShowMe()
    {
        base.ShowMe();
      

        UpdatePanelInfo();
    }
    public void UpdatePanelInfo()
    {


        //根据排行榜信息更新面板
        //读取GameMgr里面的排行榜列表，更新面板
        //RankInfo里面必须声明这个列表。
        List<RankInfo> list = GameDataMgr.Instance.rankData.list;

        /////////////////////////////////////////////////////////////////
        // 取排行榜数量和UI数量的最小值
        int count = Mathf.Min(list.Count, labName.Count);
        ////////////////////////////////////////////////////////////////////////
        //Debug.Log("刷新排行榜, 当前列表长度: " + list.Count);
        for (int i = 0; i < list.Count ; i++)
        {
            //名字
            labName[i].text = list[i].name;
            //分数
            labScore[i].text = list[i].score.ToString();
            //时间 存储的时间单位是s
            //把秒数 转换成 时  分 秒
            int time = (int)list[i].time;
            labTime[i].text = "";
            //得到 几个小时
            // 8432s  60*60 = 3600
            //8432 / 3600 ≈ 2时
            if (time / 3600 > 0)
            {
                labTime[i].text += time / 3600 + "时";
            }
            //8432-7200 余 1232s
            // 1232s / 60 ≈ 20分  
            if (time % 3600 / 60 > 0 || labTime[i].text != "")
            {
                labTime[i].text += time % 3600 / 60 + "分";
            }
            //1232s-1200 余 32秒
            labTime[i].text += time % 60 + "秒";
        }

        //////////////////////////////////////////////////////////////////////////
        // 把剩余没有数据的UI清空
        for (int i = count; i < labName.Count; i++)
        {
            labName[i].text = "";
            labScore[i].text = "";
            labTime[i].text = "";
        }
        //////////////////////////////////////////////////////////////////
       
    }

    
}
