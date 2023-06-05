
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using UnityEngine.Windows;

public class Main : MonoBehaviour
{

    public GameObject pageMain;
    public GameObject pageGame;
    
    public GameObject pageShop;
    /** 商店面板的金币对象 */
    public Text pageShopGold;

    public GameObject pageHistory;
    public GameObject pageAbout;

    /** 武器列表界面 */
    public GameObject weaponScroll;
    /** 角色列表界面 */
    public GameObject roleScroll;
    public Button pageShopWeaponBtn;
    public Button pageShopRoleBtn;

    public GameObject panelPause;
    public GameObject panelSettle;
    /** 结算面板的文本 */
    public Text settleScore;
    public Text settleGold;
    public Text settleTime;


    public GameObject boxs;

    public GameObject scoreObj;
    /** 当前游戏分数 */
    public int score;

    /** 方块生成冷却的计时器,50一秒, */
    private float boxSpawnTotol = 0;
    /** 方块生成速度调整计时器,到达指定大小后调整生成速度 */
    private float boxSpawnTimeTotol = 0;

    /** 方块下落速度调整计时器,到达指定大小后调整下落速度 */
    private float boxSpeedTotol = 0;

    /** 方块血量调整计时器,到达指定大小后调整血量 */
    private float boxHealthTotol = 0;

    /// <summary>
    /// 记录的最开始的攻击
    /// </summary>
    private float buffAttack;
    /// <summary>
    /// 攻击翻倍剩余时间,0代表没有
    /// </summary>
    private float buffAttackTotol = 0;
    /** 攻击翻倍的ui物体 */
    public GameObject buffAttackObj;

    /** 额外金币 */
    public float buffGold = 0;
    /** 额外金币的ui物体 */
    public GameObject buffGoldObj;

    /** 记录的最开始的冷却 */
    private float buffCool;
    /// <summary>
    /// 冷却减半的剩余时间,0代表没有
    /// </summary>
    public float buffCoolTotol = 0;
    /** 冷却减半的ui物体 */
    public GameObject buffCoolObj;

    /** 游戏时间 */
    public int time;
    /** 展示游戏时间的 UI */
    public GameObject timeObj;

    /** 当前已购买武器 */
    public List<string> curBuyWeapon = new List<string> {"1"};

    /** 历史分数 */
    public List<Dictionary<string,string>> scoreHisotry = new List<Dictionary<string, string>>();

    /// <summary>
    /// 最高分数
    /// </summary>
    [HideInInspector]
    public int maxScore;
    /// <summary>
    /// 最高分数的ui
    /// </summary>
    public Text historyMaxScore;
    /// <summary>
    /// 最高时间
    /// </summary>
    [HideInInspector]
    public int maxTime;
    /// <summary>
    /// 最高时间的ui
    /// </summary>
    public Text historyMaxTime;
    /// <summary>
    /// 历史分数的内容部分
    /// </summary>
    public GameObject historyContent;

    /// <summary>
    /// 弹窗
    /// </summary>
    public GameObject dialog;
    /// <summary>
    /// 弹窗点击按钮1做什么操作
    /// </summary>
    [HideInInspector]
    public string dialogOkType;
    /// <summary>
    /// 弹窗点击按钮1携带的参数
    /// </summary>
    [HideInInspector]
    public Dictionary<string, object> dialogOkData;

    /** 单例 */
    public static Main _instance;
    private void Awake()
    {
        _instance = this;
    }

    void Update()
    {
        //计算高宽比
        float x = (float)Screen.height / (float)Screen.width;

        //计算合适的摄像机距离
        float y = 2.56f * x + 0.47f;

        //相机距离自适应
        GetComponent<Camera>().orthographicSize = y;
    }

    void FixedUpdate()
    {
        if (State.isGameStart && !State.isGamePause)
        {
            boxSpawnTotol--;

            boxSpawnTimeTotol++;
            boxSpeedTotol++;
            boxHealthTotol++;

            // 方块生成
            if (boxSpawnTotol <= 0)
            {
                var obj = State.CreateRanBox();
                obj.transform.parent = boxs.transform;
                obj.transform.localPosition = Vector2.zero;

                // 遍历所有子物体给其加上Box脚本组件
                for (var i = 0; i < obj.transform.childCount; i++)
                {
                    var tmp = obj.transform.GetChild(i);
                    tmp.AddComponent<Box>();
                    var boxColidder = tmp.AddComponent<BoxCollider2D>();
                    boxColidder.size -= new Vector2(0.05f, 0.05f);
                    var rigidbody = tmp.AddComponent<Rigidbody2D>();
                    rigidbody.gravityScale = 0;
                }

                boxSpawnTotol = State.boxCurSpawnSpeed;
            }

            // 调整生成间隔
            if (boxSpawnTimeTotol >= State.boxSpawnSpeedTime)
            {
                if (State.boxCurSpawnSpeed > State.boxSpawnSpeedMax)
                {
                    State.boxCurSpawnSpeed -= State.boxSpawnSpeedTimeNum;
                }
                boxSpawnTimeTotol = 0;
            }

            // 调整下落速度
            if (boxSpeedTotol >= State.boxSpawnSpeedTime)
            {
                if (State.boxCurSpeed > State.boxSpeedMax)
                {
                    State.boxCurSpeed += State.boxSpeedTimeNum;
                }
                boxSpeedTotol = 0;
            }

            // 调整方块血量
            if (boxHealthTotol >= State.boxHealthTime)
            {
                State.boxCurHealth += State.boxHealthTimeNum;
                boxHealthTotol = 0;
            }

            // 加成部分
            if (buffAttackTotol > 0)
            {
                buffAttackObj.SetActive(true);
                buffAttackObj.GetComponent<Text>().text = "攻击翻倍：" + (buffAttackTotol / 50) + " 秒";
                if (--buffAttackTotol == 0)
                {
                    // 移除加成
                    State.playerInstance.GetComponent<Player>().weapon.attack = buffAttack;
                }
            }
            else
            {
                buffAttackObj.SetActive(false);
            }

            if (buffCoolTotol > 0)
            {
                buffCoolObj.SetActive(true);
                buffCoolObj.GetComponent<Text>().text = "冷却减半：" + (buffCoolTotol / 50) + " 秒";
                if (--buffCoolTotol == 0)
                {
                    // 移除加成
                    State.playerInstance.GetComponent<Player>().weapon.cool = buffCool;
                }
            }
            else
            {
                buffCoolObj.SetActive(false);
            }

            if (time++ % 50 == 0)
            {
                timeObj.GetComponent<Text>().text = (int)(time/50) + "s";
            }
        }
    }

    /** 开始游戏点击 */
    public void StartGame()
    {
        // 停止背景播放
        State.isBackgroundPlay = false;

        // 隐藏main显示game
        pageMain.SetActive(false);
        pageGame.SetActive(true);

        // 实例化主角
        State.PlayerInit().transform.parent = pageGame.transform;

        // 初始化值
        State.boxCurSpawnSpeed = State.boxSpawnSpeed;
        State.boxCurSpeed = State.boxSpeed;
        State.boxCurHealth = State.boxHealth;
        boxSpawnTotol = 0;
        boxSpawnTimeTotol = 0;
        boxSpeedTotol = 0;

        score = 0;
        scoreObj.GetComponent<Text>().text = "";

        // 额外金币
        buffGold = 0;
        buffGoldObj.GetComponent<Text>().text = "额外金币：0";

        time = 0;
        timeObj.GetComponent<Text>().text = "0s";

        // 初始化加成
        buffAttackTotol = 0;
        buffCoolTotol = 0;

        State.isGameStart = true;
    }

    /** 暂停游戏点击 */
    public void PauseGame()
    {
        State.isGamePause = true;
        Time.timeScale = 0;

        // 显示暂停面板
        panelPause.SetActive(true);
    }

    /** 继续按钮点击 */
    public void ContinueGame()
    {
        State.isGamePause = false;
        Time.timeScale = 1;

        // 隐藏暂停面板
        panelPause.SetActive(false);
    }

    /** 游戏结束 */
    public void StopGame()
    {
        State.isGameStart = false;
        State.isGamePause = false;

        State.Settle();

        // 显示结算面板,隐藏暂停面板
        panelSettle.SetActive(true);
        panelPause.SetActive(false);

        Time.timeScale = 0;

        Instantiate(State.audioGameOver, transform);
    }

    /** 结算完成按钮点击 */
    public void SettleOkClick()
    {
        // 隐藏game显示main
        pageGame.SetActive(false);
        pageMain.SetActive(true);

        // 隐藏结算面板
        panelSettle.SetActive(false);

        // 销毁角色
        State.PlayerDestory();
        // 销毁游戏内生成的方块
        foreach (Transform child in boxs.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        //  显示背景
        State.isBackgroundPlay = true;
        Background._instance.StartCoroutine(Background._instance.CreateRanBox());

        Time.timeScale = 1;
    }
    
    /** 消灭一个方块时触发,消灭方块的结算 */
    public void KillBox()
    {
        // 获取随机分数
        score += Random.Range(1, 16);
        // 渲染
        scoreObj.GetComponent<Text>().text = "分数：" + score;

        Instantiate(State.audioBox, transform);

        // 是否获取随机加成
        if (Random.Range(0, 2) == 0)
        {
            switch (Random.Range(0, 4))
            {
                // 攻击翻倍（1-10秒随机）
                case 0:
                    if (buffAttackTotol <= 0)
                    {
                        var weapon = State.playerInstance.GetComponent<Player>().weapon;
                        buffAttack = weapon.attack;
                        weapon.attack *= 2;
                    }
                    buffAttackTotol = Random.Range(1, 11) * 50;
                    break;
                // 额外金币（1-15随机）
                case 1:
                    buffGold += Random.Range(1, 16);
                    buffGoldObj.GetComponent<Text>().text = "额外金币：" + buffGold;
                    break;
                // 冷却减半（1-10秒随机）
                case 2:
                    if (buffCoolTotol <= 0)
                    {
                        var weapon = State.playerInstance.GetComponent<Player>().weapon;
                        buffCool = weapon.cool;
                        weapon.cool /= 2;
                    }
                    buffCoolTotol = Random.Range(1, 11) * 50;
                    break;
                // 地图拉长（将所有地面方块往下拉一排，最后一排消失）
                case 3:
                    for (var i = 0; i < boxs.transform.childCount; i++)
                    {
                        var childBoxs = boxs.transform.GetChild(i);
                        for (var j = 0; j < childBoxs.childCount; j++)
                        {
                            var box = childBoxs.GetChild(j);
                            var y = box.position.y;
                            if (y <= -4.7)
                            {
                                Destroy(box.gameObject);
                                KillBox();
                            }
                        }
                    }
                    break;
            }
        }
    }

    /** 打开商店 */
    public void OpenShop()
    {
        pageShopGold.text = "金币：" + State.gold;

        pageMain.SetActive(false);
        pageShop.SetActive(true);
    }

    /** 关闭商店 */
    public void CloseShop()
    {
        pageMain.SetActive(true);
        pageShop.SetActive(false);
    }

    /// <summary>
    /// 历史分数页面是否显示
    /// </summary>
    /// <param name="isShow">true则显示，false隐藏</param>
    public void PageScoreHistory(bool isShow)
    {
        if (isShow)
        {
            historyMaxScore.text = "最高分：" + maxScore;
            historyMaxTime.text = "最高时间：" + maxTime;

            // 倒序
            for (var i = scoreHisotry.Count - 1; i >= 0; i--)
            {
                var map = scoreHisotry[i];
                var historyPanel = Instantiate(State.historyPanel, historyContent.transform);
                // 0序号,1得分,2金币,3时间,4武器
                historyPanel.transform.GetChild(0).GetComponent<Text>().text = (i+1) + ".";
                historyPanel.transform.GetChild(1).GetComponent<Text>().text = "得分：" + map["score"];
                historyPanel.transform.GetChild(2).GetComponent<Text>().text = "金币：" + map["gold"];
                historyPanel.transform.GetChild(3).GetComponent<Text>().text = "时间：" + map["time"];
                historyPanel.transform.GetChild(4).GetComponent<Text>().text = "武器：" + map["weapon"];
            }

            // 更新Content高度
            historyContent.GetComponent<RectTransform>().sizeDelta = new Vector2(historyContent.GetComponent<RectTransform>().sizeDelta.x,
                scoreHisotry.Count * State.historyPanel.GetComponent<RectTransform>().sizeDelta.y + scoreHisotry.Count * 20 + 100);

            pageHistory.SetActive(true);
            pageMain.SetActive(false);
        }
        else
        {
            // 销毁 content 除0外的所有子对象
            for (var i = 1; i < historyContent.transform.childCount; i++)
            {
                Destroy(historyContent.transform.GetChild(i).gameObject);
            }

            pageHistory.SetActive(false);
            pageMain.SetActive(true);
        }
    }

    /// <summary>
    /// 关于游戏页面是否显示
    /// </summary>
    /// <param name="isShow">true则显示，false隐藏</param>
    public void PageAbout(bool isShow)
    {
        if (isShow)
        {
            pageAbout.SetActive(true);
            pageMain.SetActive(false);
        }
        else
        {
            pageAbout.SetActive(false);
            pageMain.SetActive(true);
        }
    }

    /** 商店内容切换 */
    public void PageShopSwitch(string type)
    {
        switch (type)
        {
            case "Weapon":
                pageShopWeaponBtn.GetComponent<Image>().color = new Color32(238, 238, 238, 255);
                pageShopRoleBtn.GetComponent<Image>().color = new Color32(255, 255, 255, 255);

                weaponScroll.SetActive(true);
                roleScroll.SetActive(false);
                break;
            case "Role":
                pageShopWeaponBtn.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                pageShopRoleBtn.GetComponent<Image>().color = new Color32(238, 238, 238, 255);

                weaponScroll.SetActive(false);
                roleScroll.SetActive(true);
                break;
        }
    }

    public void ShopWeaponClick(string weapon)
    {
        ShopWeaponClick(weapon, null);
    }

    /** 武器商品点击 */
    public void ShopWeaponClick(string weapon, GameObject clickObj)
    {
        if (clickObj == null) clickObj = EventSystem.current.currentSelectedGameObject;

        // 是否购买
        bool isBuy = false;
        for (var i = 0; i < curBuyWeapon.Count; i++)
        {
            if (curBuyWeapon[i] == weapon)
            {
                isBuy = true;
                break;
            }
        }

        if (!isBuy)
        {
            // 检查是否有足够的金额，没有则弹出提示，这里直接通过按钮的文字获取金币数
            var text = clickObj.transform.GetChild(0).GetComponent<Text>().text;
            var index = text.IndexOf("金币");
            if (index == -1) return;

            int gold = int.Parse(text.Substring(0, index));

            if (State.gold < gold)
            {
                ShowDialog("购买商品", "金币不足, 需要" + gold + "金币", "确定");
                return;
            }

            // 弹出框,提示是否购买
            ShowDialog("购买商品", "需要 " + gold + "金币，是否购买？", "购买", "取消");
            dialogOkType = "buyShop";
            dialogOkData = new Dictionary<string, object>() {{"gold",gold}, {"weapon", weapon}, {"clickObj", clickObj}};
        }

        if (isBuy)
        {
            // 切换武器，将武器按钮内容为已选择的改为选择，然后将当前按钮改为已选择
            for (var i = 0; i < clickObj.transform.parent.parent.childCount; i++)
            {
                var text = clickObj.transform.parent.parent.GetChild(i).GetChild(2).GetChild(0).GetComponent<Text>();
                if (text.text == "已选择")
                {
                    text.text = "选择";
                    break;
                }
            }

            clickObj.transform.GetChild(0).GetComponent<Text>().text = "已选择";
            State.playerCurWeapon = weapon;
            switch (weapon)
            {
                case "1":
                    State.playerWeapon = Resources.Load<GameObject>("Prefabs/Weapon/WeaponDefault");
                    break;
                case "2":
                    State.playerWeapon = Resources.Load<GameObject>("Prefabs/Weapon/Weapon2");
                    break;
                case "3":
                    State.playerWeapon = Resources.Load<GameObject>("Prefabs/Weapon/Weapon3");
                    break;
            }
        }
    }

    /// <summary>
    /// 声音状态切换
    /// </summary>
    public void switchAudio()
    {
        var obj = EventSystem.current.currentSelectedGameObject;
        if (State.isAudio)
        {
            obj.transform.GetChild(0).GetComponent<Text>().text = "开";
            AudioListener.volume = 0;
        }
        else
        {
            obj.transform.GetChild(0).GetComponent<Text>().text = "关";
            AudioListener.volume = 1;
        }
        State.isAudio = !State.isAudio;
    }

    public void ShowDialog(string title, string content, string btn2Name)
    {
        ShowDialog(title, content, null, btn2Name);
    }
    public void ShowDialog(string title, string content, string btn1Name, string btn2Name)
    {
        dialog.transform.GetChild(0).GetComponent<Text>().text = title;
        dialog.transform.GetChild(1).GetComponent<Text>().text = content;
        var btn1 = dialog.transform.GetChild(2);
        var btn1IsShow = btn1Name != null;
        btn1.gameObject.SetActive(btn1IsShow);
        if (btn1IsShow)
        {
            btn1.GetChild(0).GetComponent<Text>().text = btn1Name;
        }
        dialog.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = btn2Name;

        dialog.SetActive(true);
    }

    /// <summary>
    /// 弹窗按钮1点击
    /// </summary>
    public void DialogOk()
    {
        switch (dialogOkType)
        {
            case "buyShop":
                State.gold -= (int) dialogOkData["gold"];
                curBuyWeapon.Add((string)dialogOkData["weapon"]);
                ShopWeaponClick((string)dialogOkData["weapon"], (GameObject)dialogOkData["clickObj"]);
                OpenShop();
                break;
        }

        // 隐藏弹窗
        DialogHide();
    }

    public void DialogHide()
    {
        dialog.SetActive(false);
    }

}
