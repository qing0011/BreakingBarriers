using UnityEngine;

public class ClearRankKey : MonoBehaviour
{
    // 设置清空排行榜的按键，默认是 C
    public KeyCode clearKey = KeyCode.C;

    void Update()
    {
        //// 按下按键时清空排行榜
        //if (Input.GetKeyDown(clearKey))
        //{
        //    GameDataMgr.Instance.ClearRankData();
        //    Debug.Log("按键清空排行榜成功");
        //}
    }
}
