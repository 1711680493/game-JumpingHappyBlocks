using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    /** 子弹移速 */
    public float speed;
    /** 攻击力 */
    public float attack;

    /** 销毁时间倒计时 */
    private float timeTotol = 50 * 10;

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
        transform.position += new Vector3(0, speed);

        timeTotol--;
        // 存在时间不超过10秒
        if (timeTotol <= 0)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 只对方块生效
        if (collision.tag == "Box" || collision.tag == "BoxWall")
        {
            
            // 武器是火箭筒则格外处理
            if (State.playerCurWeapon == "3")
            {
                Collider2D[] colliders = Physics2D.OverlapBoxAll(collision.transform.position, collision.transform.localScale, 0);
                for (var i = 0; i < colliders.Length; i++)
                {
                    var item = colliders[i];
                    // 只对方块生效
                    if (item.tag == "Box" || item.tag == "BoxWall")
                    {
                        var box = item.GetComponent<Box>();
                        box.health -= attack;
                        if (box.health <= 0)
                        {
                            Destroy(item.gameObject);
                            Main._instance.KillBox();
                        }
                    }
                }
            }
            else
            {
                var box = collision.GetComponent<Box>();
                box.health -= attack;
                if (box.health <= 0)
                {
                    Destroy(collision.gameObject);
                    Main._instance.KillBox();
                }
            }


            // 销毁子弹
            Destroy(gameObject);
        }
    }

}
