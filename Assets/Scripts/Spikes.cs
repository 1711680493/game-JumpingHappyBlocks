using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        // 是角色直接游戏结束
        if (collision.gameObject.tag == "Player")
        {
            Main._instance.StopGame();
        }
    }
}
