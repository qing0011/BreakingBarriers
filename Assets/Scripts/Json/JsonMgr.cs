using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// 序列化和反序列化Json时  使用的是哪种方案
/// </summary>
public enum JsonType
{
    JsonUtlity,
    LitJson,
}

/// <summary>
/// Json数据管理类 主要用于进行 Json的序列化存储到硬盘 和 反序列化从硬盘中读取到内存中
/// </summary>
public class JsonMgr
{
    private static JsonMgr instance = new JsonMgr();
    public static JsonMgr Instance => instance;

    private JsonMgr() { }

    //存储Json数据 序列化
    public void SaveData(object data, string fileName, JsonType type = JsonType.LitJson)
    {
        //WebGL平台不支持写入文件
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            // WebGL平台不支持直接的文件系统写入，使用浏览器存储
            // 这里可以添加基于localStorage的实现
            return;
        }
        
        //确定存储路径
        string path = Application.persistentDataPath + "/" + fileName + ".json";
        //序列化 得到Json字符串
        string jsonStr = "";
        switch (type)
        {
            case JsonType.JsonUtlity:
                jsonStr = JsonUtility.ToJson(data);
                break;
            case JsonType.LitJson:
                jsonStr = JsonMapper.ToJson(data);
                break;
        }
        //把序列化的Json字符串 存储到指定路径的文件中
        File.WriteAllText(path, jsonStr);
    }

    //读取指定文件中的 Json数据 反序列化
    public T LoadData<T>(string fileName, JsonType type = JsonType.LitJson) where T : new()
    {
        //根据平台选择不同的加载方式
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            // WebGL平台使用异步加载，但这里返回默认值，实际加载需要通过回调或协程处理
            // 为了保持API兼容，这里先返回默认值，实际项目中应该使用LoadDataAsync方法
            Debug.LogWarning("WebGL平台使用异步加载，这里返回默认值，请使用LoadDataAsync方法");
            return new T();
        }
        else
        {
            return LoadDataLocal<T>(fileName, type);
        }
    }
    
    //异步加载数据（用于所有平台）
    public IEnumerator LoadDataAsync<T>(string fileName, System.Action<T> callback, JsonType type = JsonType.LitJson) where T : new()
    {
        T data = new T();
        
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            //WebGL平台使用UnityWebRequest加载StreamingAssets中的数据
            string url = Application.streamingAssetsPath + "/" + fileName + ".json";
            
            //在WebGL平台上，Application.streamingAssetsPath已经是正确的URL格式
            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                //获取加载的数据
                string jsonStr = request.downloadHandler.text;
                
                //进行反序列化
                switch (type)
                {
                    case JsonType.JsonUtlity:
                        data = JsonUtility.FromJson<T>(jsonStr);
                        break;
                    case JsonType.LitJson:
                        data = JsonMapper.ToObject<T>(jsonStr);
                        break;
                }
            }
            else
            {
                Debug.LogError("加载Json数据失败: " + request.error);
            }
        }
        else
        {
            //本地平台加载数据
            string path = Application.streamingAssetsPath + "/" + fileName + ".json";
            //先判断 是否存在这个文件
            //如果不存在默认文件 就从 读写文件夹中去寻找
            if(!File.Exists(path))
                path = Application.persistentDataPath + "/" + fileName + ".json";
            //如果读写文件夹中都还没有 那就返回一个默认对象
            if (!File.Exists(path))
            {
                callback?.Invoke(data);
                yield break;
            }

            //进行反序列化
            string jsonStr = File.ReadAllText(path);
            //数据对象
            switch (type)
            {
                case JsonType.JsonUtlity:
                    data = JsonUtility.FromJson<T>(jsonStr);
                    break;
                case JsonType.LitJson:
                    data = JsonMapper.ToObject<T>(jsonStr);
                    break;
            }
        }
        
        //调用回调
        callback?.Invoke(data);
    }
    
    //本地平台加载数据
    private T LoadDataLocal<T>(string fileName, JsonType type = JsonType.LitJson) where T : new()
    {
        //确定从哪个路径读取
        //首先先判断 默认数据文件夹中是否有我们想要的数据 如果有 就从中获取
        string path = Application.streamingAssetsPath + "/" + fileName + ".json";
        //先判断 是否存在这个文件
        //如果不存在默认文件 就从 读写文件夹中去寻找
        if(!File.Exists(path))
            path = Application.persistentDataPath + "/" + fileName + ".json";
        //如果读写文件夹中都还没有 那就返回一个默认对象
        if (!File.Exists(path))
            return new T();

        //进行反序列化
        string jsonStr = File.ReadAllText(path);
        //数据对象
        T data = default(T);
        switch (type)
        {
            case JsonType.JsonUtlity:
                data = JsonUtility.FromJson<T>(jsonStr);
                break;
            case JsonType.LitJson:
                data = JsonMapper.ToObject<T>(jsonStr);
                break;
        }

        //把对象返回出去
        return data;
    }
}
