using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

/**
 * 包含全局属性/状态
 */
public class State
{

    /** 背景是否播放 */
    public static bool isBackgroundPlay = true;

    /** 游戏是否开始 */
    public static bool isGameStart = false;
    /** 游戏是否暂停 */
    public static bool isGamePause = false;

    /** 当前金币 */
    public static int gold = 0;

    /// <summary>
    /// 声音是否开启
    /// </summary>
    public static bool isAudio = true;

    #region 角色部分
    /** 角色实例 */
    public static GameObject playerInstance;
    /** 角色当前武器 */
    public static GameObject playerWeapon = Resources.Load<GameObject>("Prefabs/Weapon/WeaponDefault");
    /** 角色当前武器的序号名称 */
    public static string playerCurWeapon = "1";
    #endregion

    #region 预制体部分
    /** 小方块 */
    public static GameObject box = Resources.Load<GameObject>("Prefabs/方块");
    /** 角色 */
    public static GameObject player = Resources.Load<GameObject>("Prefabs/Player");
    /** 历史分数面板 */
    public static GameObject historyPanel = Resources.Load<GameObject>("Prefabs/scorePanel");

    public static GameObject audioBox = Resources.Load<GameObject>("Prefabs/方块摧毁的声音");
    public static GameObject audioGameOver = Resources.Load<GameObject>("Prefabs/游戏结束的声音");
    #endregion

    #region 方块部分
    /** 小方块的大小 */
    public static float boxSize = box.GetComponent<Renderer>().bounds.size.x;

    /** 当前方块生成的速度 */
    public static float boxCurSpawnSpeed = boxSpawnSpeed;
    /** 方块的生成速度默认值 */
    public static float boxSpawnSpeed = 50 * 5;
    /** 方块的生成速度最大值 */
    public static float boxSpawnSpeedMax = 50;
    /** 方块的生成速度调整间隔(50为1秒) */
    public static float boxSpawnSpeedTime = 50*10;
    /** 方块的生成速度每次调整的大小 */
    public static float boxSpawnSpeedTimeNum = 25;

    /** 当前方块落下速度 */
    public static float boxCurSpeed = boxSpeed;
    /** 方块落下初始速度 */
    public static float boxSpeed = boxSize / 50;
    /** 方块落下最大速度 */
    public static float boxSpeedMax = 4 / 50;
    /** 方块落下初始速度调整间隔 */
    public static float boxSpeedTime = 50*10;
    /** 方块落下初始速度每次调整的大小 */
    public static float boxSpeedTimeNum = 0.1f / 50;

    /** 当前方块血量值 */
    public static float boxCurHealth = boxHealth;
    /** 方块血量值 */
    public static float boxHealth = 1;
    /** 方块血量值调整的间隔 */
    public static float boxHealthTime = 50*10;
    /** 方块血量值每次调整的大小 */
    public static float boxHealthTimeNum = 0.2f;

    /** 小方块数组中心位置 */
    private const int xCenter = 5, yCenter = 1;
    #endregion

    /** 获取随机的方块整体,通过小方块 */
    public static GameObject CreateRanBox()
    {
        // 随机小方块数量，随机颜色
        var ranNum = Random.Range(1, 7);
        var ranColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

        GameObject parentObj = new GameObject("方块集");

        // 记录已经使用过的位置
        Vector2[,] posMap = new Vector2[3,11];
        Vector2 centerPos = Vector2.zero;
        // 上一次位置
        int upX = -1, upY = -1;

        for (int i = 0; i < ranNum; i++)
        {
            GameObject obj = GameObject.Instantiate(box);

            var render = obj.GetComponent<Renderer>();
            render.material.color = ranColor;

            obj.transform.parent = parentObj.transform;

            // 记录第一个位置
            if (i == 0) {
                centerPos = new Vector2(Random.Range(-2.55f, 2.56f), 0);
                posMap[yCenter, xCenter] = centerPos;
                obj.transform.localPosition = centerPos;
            } else {
                if (upX == -1 || upY == -1)
                {
                    upX = xCenter;
                    upY = yCenter;
                }

                // 随机上下左右,0上,1下,2左,3右
                List<int> dirs = new List<int>();
                dirs.Add(0); dirs.Add(1); dirs.Add(2); dirs.Add(3);
                for (int j = 0; j < 4; j++)
                {
                    int direction = dirs[Random.Range(0, dirs.Count)];
                    // x减往左,加往右,y减往下,加往上
                    switch (direction)
                    {
                        case 0: {
                            int y = upY + 1;
                            if (y >= 3 || posMap[y, upX] != Vector2.zero)
                            {
                                dirs.Remove(direction);
                            }
                            else
                            {
                                // 1代表0.5大小,以数组中间点为界x=5,y=1,得出差距,然后*0.5得到实际位置
                                float realX = (upX - 5) * 0.5f, realY = (y - 1) * 0.5f;

                                posMap[y, upX] = new Vector2(centerPos.x + realX, centerPos.y + realY);
                                obj.transform.localPosition = posMap[y, upX];

                                upY = y;
                                goto posforEnd;
                            }
                            break;
                        }
                        case 1: {
                            int y = upY - 1;
                            if (y < 0 || posMap[y, upX] != Vector2.zero)
                            {
                                dirs.Remove(direction);
                            }
                            else
                            {
                                float realX = (upX - 5) * 0.5f, realY = (y - 1) * 0.5f;

                                posMap[y, upX] = new Vector2(centerPos.x + realX, centerPos.y + realY);
                                obj.transform.localPosition = posMap[y, upX];

                                upY = y;
                                goto posforEnd;
                            }
                            break;
                        }
                        case 2: {
                            int x = upX - 1;
                            if (x < 0 || posMap[upY, x] != Vector2.zero)
                            {
                                dirs.Remove(direction);
                            }
                            else
                            {
                                float realX = (x - 5) * 0.5f, realY = (upY - 1) * 0.5f;

                                // x位置是否超出宽度 -2.55和2.55
                                float resultX = centerPos.x + realX;
                                if (resultX < -2.55 || resultX > 2.55)
                                {
                                    dirs.Remove(direction);
                                    break;
                                }

                                posMap[upY, x] = new Vector2(resultX, centerPos.y + realY);
                                obj.transform.localPosition = posMap[upY, x];

                                upX = x;
                                goto posforEnd;
                            }
                            break;
                        }
                        case 3: {
                            int x = upX + 1;
                            if (x >= 11 || posMap[upY, x] != Vector2.zero)
                            {
                                dirs.Remove(direction);
                            }
                            else
                            {
                                float realX = (x - 5) * 0.5f, realY = (upY - 1) * 0.5f;

                                // x位置是否超出宽度 -2.55和2.55
                                float resultX = centerPos.x + realX;
                                if (resultX < -2.55 || resultX > 2.55)
                                {
                                    dirs.Remove(direction);
                                    break;
                                }

                                    posMap[upY, x] = new Vector2(resultX, centerPos.y + realY);
                                obj.transform.localPosition = posMap[upY, x];

                                upX = x;
                                goto posforEnd;
                            }
                            break;
                        }
                    }
                }

                // 如果没有合适方向的话从中心方向开始
                if (dirs.Count == 0)
                {
                    // 直接销毁当前方块,初始化值,然后重新开始本次循环即可
                    GameObject.Destroy(obj);
                    // 避免死循环,直接少生成一个方块了
                    // i--;
                    upX = -1;
                    upY = -1;
                }

                posforEnd:;
            }
        }

        return parentObj;
    }

    /** 角色初始化,初始化完成后 playerInstance 将为实例化的角色 */
    public static GameObject PlayerInit()
    {
        playerInstance = GameObject.Instantiate(player);

        playerInstance.GetComponent<Player>().weapon = GameObject.Instantiate(playerWeapon, playerInstance.transform).GetComponent<Weapon>();

        return playerInstance;
    }

    /** 角色销毁 */
    public static void PlayerDestory()
    {
        if (playerInstance != null)
        {
            GameObject.Destroy(playerInstance);
            playerInstance = null;
        }
    }

    /** 结算 */
    public static void Settle()
    {
        int scoreGold = Main._instance.score / 50, resultGold = (int) (scoreGold + Main._instance.buffGold);
        var goldText = resultGold + "(" + scoreGold + " + " + (int)(Main._instance.buffGold) + "(格外))";
        Main._instance.settleGold.text = "金币：" + goldText;

        gold += resultGold;

        Main._instance.settleScore.text = "得分：" + Main._instance.score;
        Main._instance.settleTime.text = "时间：" + (int)(Main._instance.time/50) + "秒";

        // 是否为最高分，是则替换
        if (Main._instance.score > Main._instance.maxScore)
        {
            Main._instance.maxScore = Main._instance.score;
        }

        // 是否为最长时，是则替换
        if ((int)(Main._instance.time / 50) > Main._instance.maxTime)
        {
            Main._instance.maxTime = (int)(Main._instance.time / 50);
        }

        Dictionary<string, string> map = new Dictionary<string, string>();
        map.Add("gold", goldText);
        map.Add("score", ""+Main._instance.score);
        map.Add("time", (int)(Main._instance.time / 50) + "秒");
        map.Add("weapon", playerWeapon.GetComponent<Weapon>().name);
        Main._instance.scoreHisotry.Add(map);
    }

}
