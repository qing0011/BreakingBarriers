using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class GameMain : MonoBehaviour
{
    private void Start()
    {
        UIManager.Instance.ShowPanel<GamePanel>();
        Debug.Log(SystemInfo.supports32bitsIndexBuffer);
        Debug.Log(SystemInfo.GetGraphicsFormat(DefaultFormat.DepthStencil));
    }
}
