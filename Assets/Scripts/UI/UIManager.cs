using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager 
{
//    单例模式（懒汉式改进）

//instance 是一个静态对象，程序运行时自动创建

//外部通过 UIManager.Instance 使用 UIManager

//构造函数是私有的，所以外部无法 new UIManager
   private static UIManager instance=new UIManager();
    public static UIManager Instance=>instance;
    //字典保存所有“已经创建的面板”
    //作用： 防止重复创建面板， 帮你保存每个面板实例

    private Dictionary<string,BasePanel> panelDic = new Dictionary<string,BasePanel>();
    private Transform canvasTrans;
    //初始化 UIManager 的时候自动创建 Canvas
    private UIManager() 
    {
        //得到场景中的canvas对象
        GameObject canvas = GameObject.Instantiate(Resources.Load<GameObject>("UI/Canvas"));
        canvasTrans = canvas.transform;
        GameObject.DontDestroyOnLoad(canvas);//确保只有一个canvas
    
    }


    //显示面板
    public T ShowPanel<T>() where T: BasePanel
    {
        //我们只需要保证 泛型T的类型 和面板预设体名字 一样 定一个这样的规则 就可以非常方便的让我们使用了
        string panelName = typeof(T).Name;

        //判断 字典中 是否已经显示了这个面板
        if (panelDic.ContainsKey(panelName))
            return panelDic[panelName] as T;

        //显示面板 根据面板名字 动态的创建预设体 设置父对象
        GameObject panelObj = GameObject.Instantiate(Resources.Load<GameObject>("UI/" + panelName));
        //把这个对象 放到场景中的 Canvas下面
        panelObj.transform.SetParent(canvasTrans, false);

        //指向面板上 显示逻辑 并且应该把它保存起来
        T panel = panelObj.GetComponent<T>();
        //把这个面板脚本 存储到字典中 方便之后的 获取 和 隐藏
        panelDic.Add(panelName, panel);
        //调用自己的显示逻辑
        panel.ShowMe();

        return panel;
    }
    //隐藏面板
    public void HidePanel<T>(bool isFade = true) where T : BasePanel
    {
        //
        string panelName = typeof(T).Name;
        //判断当前现实的面板 
        if (panelDic.ContainsKey(panelName))
        {
                if (isFade)
                {
                    //面板淡出完毕后再删除他
                    panelDic[panelName].HideMe(() =>
                    {
                        //删除对象
                        GameObject.Destroy(panelDic[panelName].gameObject);
                        //删除字典里面存储的面板脚本
                        panelDic.Remove(panelName);
                    });
                }
                else
                {
                    //删除对象
                    GameObject.Destroy(panelDic[panelName].gameObject);
                    //删除字典里面存储的面板脚本
                    panelDic.Remove(panelName);
                }
            
        }
     
    }
    //得到面板   不会创建，只检查字典。
    public T GetPanel<T>() where T : BasePanel
    {
        string panelName = typeof (T).Name;

        if(panelDic.ContainsKey(panelName)) 
            return panelDic[panelName] as T;
        //没有返回空
        return null;
    }
}
