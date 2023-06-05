using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Rocker : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{

    private Image rockerImg;
    private Image rockerCenterImg;

    /** 摇杆第一次点击的位置 */
    private Vector2 firstPos;

    // 当前摇杆拖拽位置
    public static Vector2 curPos;

    // Start is called before the first frame update
    void Start()
    {
        rockerImg = transform.GetChild(0).GetComponent<Image>();
        rockerCenterImg = transform.GetChild(0).GetChild(0).GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 处理按下事件
    public void OnPointerDown(PointerEventData eventData)
    {
        // eventData.position是按下的位置
        rockerImg.transform.position = eventData.position;
        firstPos = eventData.position;

        OnDrag(eventData);

        rockerImg.gameObject.SetActive(true);
    }

    // 处理拖放事件
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position = Vector2.zero;
        // 当前位置为点击位置减去初始位置
        var curPos = new Vector2(eventData.position.x - firstPos.x, eventData.position.y - firstPos.y);

        curPos = (curPos.magnitude > 1) ? curPos.normalized : curPos;
        // 限制能移动的区域
        rockerCenterImg.rectTransform.anchoredPosition = new Vector2(curPos.x * (rockerImg.rectTransform.sizeDelta.x / 3), curPos.y * (rockerImg.rectTransform.sizeDelta.y) / 3);

        Rocker.curPos = curPos;
    }

    // 处理按下后放手事件
    public void OnPointerUp(PointerEventData eventData)
    {
        rockerImg.gameObject.SetActive(false);
        rockerCenterImg.transform.position = Vector2.zero;
        Rocker.curPos = Vector2.zero;
    }

}
