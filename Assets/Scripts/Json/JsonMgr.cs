using LitJson;
using System.Collections;
using System.IO;
using UnityEngine;

public class JsonMgr
{
    private static JsonMgr instance = new JsonMgr();
    public static JsonMgr Instance => instance;

    private JsonMgr() { }

    // 判断是否是存档文件
    private bool IsSaveFile(string fileName)
    {
        return fileName == "SignInSave"
            || fileName == "scoreData"
            || fileName == "Rank";
    }

    // 保存数据（统一用 LitJson）
    public void SaveData(object data, string fileName)
    {
        string path = Application.persistentDataPath + "/" + fileName + ".json";
        string jsonStr = JsonMapper.ToJson(data);
        File.WriteAllText(path, jsonStr);
    }

    // 异步加载
    public IEnumerator LoadDataAsync<T>(string fileName, System.Action<T> callback) where T : new()
    {
        T data = new T();

        // ===== 存档文件 =====
        if (IsSaveFile(fileName))
        {
            string path = Application.persistentDataPath + "/" + fileName + ".json";

            if (File.Exists(path))
            {
                string jsonStr = File.ReadAllText(path);
                data = JsonMapper.ToObject<T>(jsonStr);
            }

            callback?.Invoke(data);
            yield break;
        }

        // ===== 配置文件（Resources）=====
        TextAsset ta = Resources.Load<TextAsset>("Data/" + fileName);

        if (ta != null)
        {
            data = JsonMapper.ToObject<T>(ta.text);
        }
        else
        {
            Debug.LogError("Resources中找不到文件: " + fileName);
        }

        callback?.Invoke(data);
        yield return null;
    }
}