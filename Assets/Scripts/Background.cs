using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Background : MonoBehaviour
{
    /** 单例 */
    public static Background _instance;
    private void Awake()
    {
        _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CreateRanBox());
    }

    /** 创建方块，三秒后销毁 */
    public IEnumerator CreateRanBox()
    {
        while (true)
        {
            // 等待随机时间
            yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));

            // 创建方块
            if (State.isBackgroundPlay) {
                var obj = State.CreateRanBox();
                obj.transform.parent = transform;
                obj.transform.localPosition = Vector2.zero;

                Rigidbody rigidbody = obj.AddComponent<Rigidbody>();
                rigidbody.velocity = Vector3.down * Random.Range(0.1f, 2f);

                Destroy(obj, 2f);
            }
            else
            {
                break;
            }
        }
    }

}
