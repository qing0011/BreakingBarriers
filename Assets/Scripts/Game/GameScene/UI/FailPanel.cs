using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FailPanel : BasePanel<FailPanel>
{
    public CustomGUIButton btnBack;
    public CustomGUIButton btnGoOn;
    // Start is called before the first frame update
    void Start()
    {
        btnBack.clickEvent += () =>
        {
            //取消暂停
            Time.timeScale = 1;
            //切换场景
            SceneManager.LoadScene("BeginScene");
        };
        btnGoOn.clickEvent += () =>
        {
            //取消暂停
            Time.timeScale = 1;
            //再次切换场景
            SceneManager.LoadScene("GameScene");
        };
        HideMe();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
