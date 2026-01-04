using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class WinPanel : BasePanel<WinPanel>
{
    //关联控件
    public CustomGUIInput inputInfo;
    public CustomGUIButton btnSure;

    // Start is called before the first frame update
    void Start()
    {
        btnSure.clickEvent += () =>
        {
            //取消游戏暂停
            Time.timeScale = 1;
           // Debug.Log("排行榜长度: " + GameDataMgr.Instance.rankData.list.Count);

            //把数据记录到排行榜中 并且 回到主场景中
            GameDataMgr.Instance.AddRankInfo(inputInfo.content.text,
                 GamePanel.Instance.nowScore,//GamePanel.Instance.nowScore,
                 GamePanel.Instance.nowTime);//GamePanel.Instance.nowTime
                                             //Debug.Log("已添加排行榜数据: " + inputInfo.content.text +
                                             //" 分数: " + GamePanel.Instance.nowScore +
                                             //" 时间: " + GamePanel.Instance.nowTime);
                                             //接着 就返回我们的 开始界面即可
            SceneManager.LoadScene("BeginScene");
        };

        HideMe();
    }
}


