using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class Transfer
{
    public Vector2 from;
    public Vector2 to;
}

[Serializable]
public class GameData
{
    public int col = 5;
    public int row = 5;

    public Vector2 littleStarPos;
    public Vector2 bigStarPos;

    public List<Vector2> obstacleList = new List<Vector2>();

    public List<Transfer> transferList = new List<Transfer>();
}

public class StarGame : MonoBehaviour
{
    public int level;

    public float unitMoveSpeed = 0.2f;

    public List<GameData> gameDatas = new List<GameData>();

    private GameData gameData;

    private float halfCol;
    private float halfRow;

    private Vector2 curLStarPos;

    private Transform littleStarTrans;
    private Transform bigStarTrans;
    private Transform directionBtnFrame;
    private Transform[] directionBtns = new Transform[4];
    private TweenPosition tweenPos;
    private UIPlayTween playTween;

    private int iUnitSize = 50;

    void Awake()
    {
        InitCom();
    }

    private void InitCom()
    {
        if (gameDatas.Count <= 0)
        {
            Debug.LogError("没有游戏数据");
            return;
        }

        level = Mathf.Max(0, Mathf.Min(level, gameDatas.Count));
        gameData = gameDatas[level];
        if (gameData == null)
        {
            Debug.LogError("该关卡的游戏数据为空");
            return;
        }

        DrawMapLine();

        //初始化已有的UI
        littleStarTrans = transform.Find("LittleStar");
        tweenPos = littleStarTrans.GetComponent<TweenPosition>();
        playTween = littleStarTrans.GetComponent<UIPlayTween>();
        EventDelegate.Add(playTween.onFinished, OnTweenPosFinished);
        bigStarTrans = transform.Find("BigStar");

        directionBtnFrame = transform.Find("DirectionBtnFrame");
        UIButton directionBtn;
        for (int i = 0; i < 4; i++)
        {
            if (i == 0)
                directionBtns[i] = directionBtnFrame.Find("UpBtn");
            else if (i == 1)
                directionBtns[i] = directionBtnFrame.Find("DownBtn");
            else if (i == 2)
                directionBtns[i] = directionBtnFrame.Find("LeftBtn");
            else if (i == 3)
                directionBtns[i] = directionBtnFrame.Find("RightBtn");

            directionBtn = directionBtns[i].GetComponent<UIButton>();
            EventDelegate.Add(directionBtn.onClick, OnBtnClick);
        }
    }

    private void DrawMapLine()
    {
        GameObject prefab = Resources.Load("Prefab/Line") as GameObject;
        if (prefab == null)
        {
            Debug.LogError("获取不到线预制体");
            return;
        }

        halfCol = gameData.col * 0.5f;
        halfRow = gameData.row * 0.5f;

        int i;
        GameObject mapObj = new GameObject("Map");
        for (i = 0; i < gameData.col + 1; i++)
        {
            GameObject lineClone = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
            lineClone.transform.SetParent(mapObj.transform);
            lineClone.transform.localScale = Vector3.one;
            lineClone.transform.localPosition = new Vector2((-halfCol + i) * iUnitSize, 0);

            UISprite lineSp = lineClone.transform.GetComponent<UISprite>();
            lineSp.SetDimensions(iUnitSize, iUnitSize * gameData.row);
        }

        for (i = 0; i < gameData.row + 1; i++)
        {
            GameObject lineClone = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
            lineClone.transform.SetParent(mapObj.transform);
            lineClone.transform.localScale = Vector3.one;
            lineClone.transform.localEulerAngles = new Vector3(0, 0, 90.0f);
            lineClone.transform.localPosition = new Vector2(0, (-halfRow + i) * iUnitSize);

            UISprite lineSp = lineClone.transform.GetComponent<UISprite>();
            lineSp.SetDimensions(iUnitSize, iUnitSize * gameData.col);
        }

        prefab = Resources.Load("Prefab/Obstacle") as GameObject;
        if (prefab == null)
        {
            Debug.LogError("获取不到障碍物预制体");
            return;
        }

        for (i = 0; i < gameData.obstacleList.Count; i++)
        {
            GameObject obstacleClone = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
            obstacleClone.transform.SetParent(mapObj.transform);
            obstacleClone.transform.localScale = Vector3.one;
            SetPos(obstacleClone.transform, gameData.obstacleList[i].x, gameData.obstacleList[i].y);
        }
    }


    void Start()
    {
        InitData();
    }

    private void InitData()
    {
        curLStarPos = gameData.littleStarPos;
        SetPos(littleStarTrans, gameData.littleStarPos.x, gameData.littleStarPos.y);

        SetPos(bigStarTrans, gameData.bigStarPos.x, gameData.bigStarPos.y);

        ShowDirectionBtn();
    }

    private void SetPos(Transform trans, float posX, float poxY)
    {
        if (trans != null)
            trans.localPosition = GetPos(posX, poxY);
    }

    private Vector2 GetPos(float posX, float poxY)
    {
        return new Vector2((-halfCol + posX + 0.5f) * iUnitSize, (-halfRow + poxY + 0.5f) * iUnitSize);
    }

    private void ShowDirectionBtn()
    {
        directionBtnFrame.gameObject.SetActive(true);

        bool[] isHasObstacle = new bool[4] { false, false, false, false };

        for (int i = 0; i < gameData.obstacleList.Count; i++)
        {
            if (gameData.obstacleList[i].x == curLStarPos.x)
            {
                if (gameData.obstacleList[i].y == curLStarPos.y + 1)
                    isHasObstacle[0] = true;

                if (gameData.obstacleList[i].y == curLStarPos.y - 1)
                    isHasObstacle[1] = true;
            }

            if (gameData.obstacleList[i].y == curLStarPos.y)
            {
                if (gameData.obstacleList[i].x == curLStarPos.x + 1)
                    isHasObstacle[3] = true;

                if (gameData.obstacleList[i].x == curLStarPos.x - 1)
                    isHasObstacle[2] = true;
            }
        }

        //上
        if (!isHasObstacle[0] && curLStarPos.y >= 0 && curLStarPos.y < gameData.row)
        {
            directionBtns[0].gameObject.SetActive(true);
            SetPos(directionBtns[0], curLStarPos.x, curLStarPos.y + 1);
        }
        else
            directionBtns[0].gameObject.SetActive(false);

        //下
        if (!isHasObstacle[1] && curLStarPos.y > 0 && curLStarPos.y <= gameData.row)
        {
            directionBtns[1].gameObject.SetActive(true);
            SetPos(directionBtns[1], curLStarPos.x, curLStarPos.y - 1);
        }
        else
            directionBtns[1].gameObject.SetActive(false);

        //左
        if (!isHasObstacle[2] && curLStarPos.x > 0 && curLStarPos.x <= gameData.col)
        {
            directionBtns[2].gameObject.SetActive(true);
            SetPos(directionBtns[2], curLStarPos.x - 1, curLStarPos.y);
        }
        else
            directionBtns[2].gameObject.SetActive(false);

        //右
        if (!isHasObstacle[3] && curLStarPos.x >= 0 && curLStarPos.x < gameData.col)
        {
            directionBtns[3].gameObject.SetActive(true);
            SetPos(directionBtns[3], curLStarPos.x + 1, curLStarPos.y);
        }
        else
            directionBtns[3].gameObject.SetActive(false);
    }

    private void OnBtnClick()
    {
        switch (UIButton.current.name)
        {
            case "UpBtn":
                {
                    float tempMaxPosY = gameData.row + 1;
                    for (int i = 0; i < gameData.obstacleList.Count; i++)
                    {
                        if (gameData.obstacleList[i].x == curLStarPos.x && gameData.obstacleList[i].y > curLStarPos.y)
                        {
                            tempMaxPosY = Mathf.Min(tempMaxPosY, gameData.obstacleList[i].y);
                        }
                    }

                    tempMaxPosY--;

                    tweenPos.from = littleStarTrans.localPosition;
                    if (curLStarPos.x == gameData.bigStarPos.x && gameData.bigStarPos.y < tempMaxPosY)
                    {
                        tweenPos.to = GetPos(curLStarPos.x, gameData.bigStarPos.y);
                        tweenPos.duration = unitMoveSpeed * Mathf.Abs(gameData.bigStarPos.y - curLStarPos.y);
                        curLStarPos = new Vector2(curLStarPos.x, gameData.bigStarPos.y);
                    }
                    else
                    {
                        tweenPos.to = GetPos(curLStarPos.x, tempMaxPosY);
                        tweenPos.duration = unitMoveSpeed * Mathf.Abs(tempMaxPosY - curLStarPos.y);
                        curLStarPos = new Vector2(curLStarPos.x, tempMaxPosY);
                    }

                    playTween.Play(true);

                    directionBtnFrame.gameObject.SetActive(false);
                }
                break;

            case "DownBtn":
                {
                    float tempMaxPosY = -2;
                    for (int i = 0; i < gameData.obstacleList.Count; i++)
                    {
                        if (gameData.obstacleList[i].x == curLStarPos.x && gameData.obstacleList[i].y < curLStarPos.y)
                        {
                            tempMaxPosY = Mathf.Max(tempMaxPosY, gameData.obstacleList[i].y);
                        }
                    }

                    tempMaxPosY++;

                    tweenPos.from = littleStarTrans.localPosition;
                    if (curLStarPos.x == gameData.bigStarPos.x && gameData.bigStarPos.y > tempMaxPosY)
                    {
                        tweenPos.to = GetPos(curLStarPos.x, gameData.bigStarPos.y);
                        tweenPos.duration = unitMoveSpeed * Mathf.Abs(gameData.bigStarPos.y - curLStarPos.y);
                        curLStarPos = new Vector2(curLStarPos.x, gameData.bigStarPos.y);
                    }
                    else
                    {
                        tweenPos.to = GetPos(curLStarPos.x, tempMaxPosY);
                        tweenPos.duration = unitMoveSpeed * Mathf.Abs(tempMaxPosY - curLStarPos.y);
                        curLStarPos = new Vector2(curLStarPos.x, tempMaxPosY);
                    }

                    playTween.Play(true);

                    directionBtnFrame.gameObject.SetActive(false);
                }
                break;

            case "LeftBtn":
                {
                    float tempMaxPosX = -2;
                    for (int i = 0; i < gameData.obstacleList.Count; i++)
                    {
                        if (gameData.obstacleList[i].y == curLStarPos.y && gameData.obstacleList[i].x < curLStarPos.x)
                        {
                            tempMaxPosX = Mathf.Max(tempMaxPosX, gameData.obstacleList[i].x);
                        }
                    }

                    tempMaxPosX++;

                    tweenPos.from = littleStarTrans.localPosition;
                    if (curLStarPos.y == gameData.bigStarPos.y && gameData.bigStarPos.x > tempMaxPosX)
                    {
                        tweenPos.to = GetPos(gameData.bigStarPos.x, curLStarPos.y);
                        tweenPos.duration = unitMoveSpeed * Mathf.Abs(gameData.bigStarPos.x - curLStarPos.x);
                        curLStarPos = new Vector2(gameData.bigStarPos.x, curLStarPos.y);
                    }
                    else
                    {
                        tweenPos.to = GetPos(tempMaxPosX, curLStarPos.y);
                        tweenPos.duration = unitMoveSpeed * Mathf.Abs(tempMaxPosX - curLStarPos.x);
                        curLStarPos = new Vector2(tempMaxPosX, curLStarPos.y);
                    }

                    playTween.Play(true);

                    directionBtnFrame.gameObject.SetActive(false);
                }
                break;

            case "RightBtn":
                {
                    float tempMaxPosX = gameData.col + 1;
                    for (int i = 0; i < gameData.obstacleList.Count; i++)
                    {
                        if (gameData.obstacleList[i].y == curLStarPos.y && gameData.obstacleList[i].x > curLStarPos.x)
                        {
                            tempMaxPosX = Mathf.Min(tempMaxPosX, gameData.obstacleList[i].x);
                        }
                    }

                    tempMaxPosX--;

                    tweenPos.from = littleStarTrans.localPosition;
                    if (curLStarPos.y == gameData.bigStarPos.y && gameData.bigStarPos.x < tempMaxPosX)
                    {
                        tweenPos.to = GetPos(gameData.bigStarPos.x, curLStarPos.y);
                        tweenPos.duration = unitMoveSpeed * Mathf.Abs(gameData.bigStarPos.x - curLStarPos.x);
                        curLStarPos = new Vector2(gameData.bigStarPos.x, curLStarPos.y);
                    }
                    else
                    {
                        tweenPos.to = GetPos(tempMaxPosX, curLStarPos.y);
                        tweenPos.duration = unitMoveSpeed * Mathf.Abs(tempMaxPosX - curLStarPos.x);
                        curLStarPos = new Vector2(tempMaxPosX, curLStarPos.y);
                    }

                    playTween.Play(true);

                    directionBtnFrame.gameObject.SetActive(false);
                }
                break;
        }
    }

    private void OnTweenPosFinished()
    {
        if (curLStarPos == gameData.bigStarPos)
        {
            Debug.Log("胜利");
            return;
        }

        //判断输赢
        if (curLStarPos.x < 0 || curLStarPos.x > gameData.col - 1
            || curLStarPos.y < 0 || curLStarPos.y > gameData.row - 1)
        {
            Debug.Log("失败");
            return;
        }

        ShowDirectionBtn();
    }
}
