using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private Vector3 pos;
    public float H = 10;
    public Transform targetPlayer;



    void LateUpdate()
    {
        if (targetPlayer == null)
        {
            pos.x = targetPlayer.position.x;
            pos.z = targetPlayer.position.z;
            //外部调用高度

            pos.y = H;
            this.transform.position = pos;
        }
    }
}
