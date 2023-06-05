using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    /** 每次的角色速度,两个小方块大小,每秒执行50次 */
    public float playerSpeed;

    /** 是否可以跳跃 */
    private bool canSkip = true;

    /** 角色的武器 */
    public Weapon weapon;

    private void Awake()
    {
        playerSpeed = State.boxSize * 2 / 50;
    }

    void FixedUpdate()
    {
        var curPos = Rocker.curPos;
        if (curPos.magnitude != 0)
        {
            if (curPos.x < 0)
            {
                transform.position = new Vector3(transform.position.x - playerSpeed, transform.position.y);
            }
            else if (curPos.x > 0)
            {
                transform.position = new Vector3(transform.position.x + playerSpeed, transform.position.y);
            }

            // y大于0.7代表跳跃
            if (curPos.y > 0.7)
            {
                if (canSkip)
                {
                    canSkip = false;
                    transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 4);
                }
            }
            else
            {
                canSkip = true;
            }
        }
        else
        {
            canSkip = true;
        }
    }

}
