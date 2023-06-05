using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Weapon : MonoBehaviour
{

    /** 攻击力 */
    public float attack;
    /** 冷却,单位秒 */
    public float cool;
    /** 攻速.单位方块 */
    public float speed;
    /** 武器名称 */
    public string name;
    /** 商店内的金额 */
    public int gold;

    /** 子弹 */
    public GameObject bullet;

    /** 用于冷却的计数 */
    private float coolTotol = 0;

    void FixedUpdate()
    {
        if (State.isGameStart)
        {
            coolTotol++;

            if (coolTotol / 50 >= cool)
            {
                var obj = GameObject.Instantiate(bullet);
                obj.transform.position = transform.position;

                var script = obj.GetComponent<Bullet>();
                script.speed = speed * State.boxSize / 50;
                script.attack = attack;

                coolTotol = 0;
            }
        }
    }

}
