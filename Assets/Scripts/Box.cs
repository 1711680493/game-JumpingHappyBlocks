using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

public class Box : MonoBehaviour
{
    // 方块是否执行完成
    public bool isOk = false;

    /** 当前方块的血量 */
    public float health = State.boxCurHealth;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void FixedUpdate()
    {
        if (State.isGameStart && !State.isGamePause && !isOk)
        {
            transform.position -= new Vector3(0, State.boxCurSpeed);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Wall":
            case "BoxWall":
                isOk = true;
                tag = "BoxWall";

                GetComponent<Rigidbody2D>().gravityScale = 1;
                //Destroy(GetComponent<Rigidbody2D>());
                break;
            case "Player":
                if (gameObject.tag == "Box")
                {
                    Main._instance.StopGame();
                }
                else if (gameObject.tag == "BoxWall")
                {
                    if (Math.Abs(GetComponent<Rigidbody2D>().velocity.y) > 0.3f)
                    {
                        Main._instance.StopGame();
                    }
                }
                break;
        }
    }

}
