using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Input01 : MonoBehaviour
{
    public Texture2D tex;
    // Start is called before the first frame update
    void Start()
    {
        //隐藏鼠标
        Cursor.visible = true;
        //锁定鼠标
        Cursor.lockState = CursorLockMode.Confined;
        //设置鼠标图片
        Cursor.SetCursor(tex, Vector2.zero, CursorMode.Auto);


       //随机数
        //Unity中的随机数
        //Unity当中 的Random类 此Random(Unity)非彼Random（C#）
        //使用随机数 int重载 规则是 左包含 右不包含
        //0~99之间的数
        int randomNum = Random.Range(0, 100);
        print(randomNum);
        //float重载 规则是 左右都包含
        float randomNumF = Random.Range(1.1f, 99.9f);

        //C#中的随机数
        //System.Random r = new System.Random();
        //r.Next(0, 100);
  
//       知识点二 委托
        //C#的自带委托
        System.Action ac = () =>
        {
            print("123");
        };

        System.Action<int, float> ac2 = (i, f) =>
        {

        };

        System.Func<int> fun1 = () =>
        {
            return 1;
        };

        System.Func<int, string> fun2 = (i) =>
        {
            return "123";
        };
        //Unity的自带委托
        //UnityAction uac = () =>
        //{

        //};

        //UnityAction<string> uac1 = (s) =>
        //{

        //};

    }

    // Update is called once per frame
    void Update()
    {
        //切换场景
        if (Input.GetKeyDown(KeyCode.W))
        {
            SceneManager.LoadScene("Test01");
        }
        //退出游戏
        if (Input.GetKeyDown(KeyCode.S))
        {
            Application.Quit();
           // UnityEditor.EditorApplication.isPlaying = false;
        }

       

    }
}
