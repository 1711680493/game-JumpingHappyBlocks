using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeDestory : MonoBehaviour
{

    /// <summary>
    /// 多少秒后销毁,50一秒
    /// </summary>
    public int totol = 50;

    private void FixedUpdate()
    {
        if (--totol < 0)
        {
            Destroy(gameObject);
        }
    }

}
