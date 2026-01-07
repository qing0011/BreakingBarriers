using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMain : MonoBehaviour
{
    private void Start()
    {
        UIManager.Instance.ShowPanel<GamePanel>();
    }
}
