using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StarGame : MonoBehaviour 
{
    //private class MapInfo
    //{
    //    public int posX;
    //    public int posY;
    //    public int occupy;

    //    public MapInfo(int newPosX, int newPosY, int newOccupy)
    //    {
    //        posX = newPosX;
    //        posY = newPosY;
    //        occupy = newOccupy;
    //    }
    //}

    public int col = 5;
    public int row = 5;

    public float unitMoveSpeed = 0.2f;

    private float halfCol;
    private float halfRow;

    public Vector2 littleStarPos;
    public Vector2 bigStarPos;
    //private Vector2 curLStarPos;
    public Vector2 curLStarPos;

    public List<Vector2> obstacleList = new List<Vector2>();
    //private List<MapInfo> mapInfoList = new List<MapInfo>();
    private Transform littleStarTrans;
    private Transform bigStarTrans;
    private Transform directionBtnFrame;
    private Transform[] directionBtns = new Transform[4];
    private TweenPosition tweenPos;

    private int iUnitSize = 50;

    void Awake()
    {
        InitCom();
    }

    private void InitCom()
    {
        DrawMapLine();

        //初始化已有的UI
        littleStarTrans = transform.Find("LittleStar");
        tweenPos = littleStarTrans.GetComponent<TweenPosition>();
        EventDelegate.Add(tweenPos.onFinished, OnTweenPosFinished);
        bigStarTrans = transform.Find("BigStar");

        directionBtnFrame = transform.Find("DirectionBtnFrame");
        UIButton directionBtn;
        for (int i = 0; i < 4; i++)
        {
            if (i == 0)
                directionBtns[i] = directionBtnFrame.Find("UpBtn");
            else if(i == 1)
                directionBtns[i] = directionBtnFrame.Find("DownBtn");
            else if(i == 2)
                directionBtns[i] = directionBtnFrame.Find("LeftBtn");
            else if(i == 3)
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

        halfCol = col * 0.5f;
        halfRow = row * 0.5f;

        int i;
        GameObject mapObj = new GameObject("Map");
        for (i = 0; i < col + 1; i++)
        {
            GameObject lineClone = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
            lineClone.transform.SetParent(mapObj.transform);
            lineClone.transform.localScale = Vector3.one;
            lineClone.transform.localPosition = new Vector2((-halfCol + i) * iUnitSize, 0);
            
            UISprite lineSp = lineClone.transform.GetComponent<UISprite>();
            lineSp.SetDimensions(iUnitSize, iUnitSize * row);
        }

        for (i = 0; i < row + 1; i++)
        {
            GameObject lineClone = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
            lineClone.transform.SetParent(mapObj.transform);
            lineClone.transform.localScale = Vector3.one;
            lineClone.transform.localEulerAngles = new Vector3(0, 0, 90.0f);
            lineClone.transform.localPosition = new Vector2(0, (-halfRow + i) * iUnitSize);

            UISprite lineSp = lineClone.transform.GetComponent<UISprite>();
            lineSp.SetDimensions(iUnitSize, iUnitSize * col);
        }

        prefab = Resources.Load("Prefab/Obstacle") as GameObject;
        if (prefab == null)
        {
            Debug.LogError("获取不到障碍物预制体");
            return;
        }

        for (i = 0; i < obstacleList.Count; i++)
        {
            GameObject obstacleClone = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
            obstacleClone.transform.SetParent(mapObj.transform);
            obstacleClone.transform.localScale = Vector3.one;
            SetPos(obstacleClone.transform, obstacleList[i].x, obstacleList[i].y);
        }
    }


    void Start()
    {
        InitData();
    }

    private void InitData()
    {
        curLStarPos = littleStarPos;
        SetPos(littleStarTrans, littleStarPos.x, littleStarPos.y);

        SetPos(bigStarTrans, bigStarPos.x, bigStarPos.y);

        ShowDirectionBtn();
    }

    private void SetPos(Transform trans, float posX, float poxY)
    {
        if(trans != null)
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

        for (int i = 0; i < obstacleList.Count; i++)
        {
            if (obstacleList[i].x == curLStarPos.x)
            {
                if (obstacleList[i].y == curLStarPos.y + 1)
                    isHasObstacle[0] = true;

                if(obstacleList[i].y == curLStarPos.y - 1)
                    isHasObstacle[1] = true;
            }

            if (obstacleList[i].y == curLStarPos.y)
            {
                if (obstacleList[i].x == curLStarPos.x + 1)
                    isHasObstacle[3] = true;

                if (obstacleList[i].x == curLStarPos.x - 1)
                    isHasObstacle[2] = true;
            }
        }

        //上
        if (!isHasObstacle[0] && curLStarPos.y >= 0 && curLStarPos.y < row)
        {
            directionBtns[0].gameObject.SetActive(true);
            SetPos(directionBtns[0], curLStarPos.x, curLStarPos.y + 1);
        }
        else
            directionBtns[0].gameObject.SetActive(false);

        //下
        if (!isHasObstacle[1] && curLStarPos.y > 0 && curLStarPos.y <= row)
        {
            directionBtns[1].gameObject.SetActive(true);
            SetPos(directionBtns[1], curLStarPos.x, curLStarPos.y - 1);
        }
        else
            directionBtns[1].gameObject.SetActive(false);

        //左
        if (!isHasObstacle[2] && curLStarPos.x > 0 && curLStarPos.x <= col)
        {
            directionBtns[2].gameObject.SetActive(true);
            SetPos(directionBtns[2], curLStarPos.x - 1, curLStarPos.y);
        }
        else
            directionBtns[2].gameObject.SetActive(false);

        //右
        if (!isHasObstacle[3] && curLStarPos.x >= 0 && curLStarPos.x < col)
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
                    float tempMaxPosY = row + 1;
                    for (int i = 0; i < obstacleList.Count; i++)
                    {
                        if (obstacleList[i].x == curLStarPos.x && obstacleList[i].y > curLStarPos.y)
                        {
                            tempMaxPosY = Mathf.Min(tempMaxPosY, obstacleList[i].y);
                        }
                    }

                    tempMaxPosY--;
                    //if (tempMaxPosY <= row)
                    //{
                    //tweenPos.ResetToBeginning();
                        tweenPos.from = littleStarTrans.localPosition;
                        tweenPos.to = GetPos(curLStarPos.x, tempMaxPosY);
                        tweenPos.duration = unitMoveSpeed * Mathf.Abs(tempMaxPosY - curLStarPos.y);
                        curLStarPos = new Vector2(curLStarPos.x, tempMaxPosY);
                        tweenPos.PlayForward();
                    //}

                        directionBtnFrame.gameObject.SetActive(false);
                }
                break;

            case "DownBtn":
                {
                    float tempMaxPosY = -2;
                    for (int i = 0; i < obstacleList.Count; i++)
                    {
                        if (obstacleList[i].x == curLStarPos.x && obstacleList[i].y < curLStarPos.y)
                        {
                            tempMaxPosY = Mathf.Max(tempMaxPosY, obstacleList[i].y);
                        }
                    }

                    tempMaxPosY++;

                    //if (tempMaxPosY <= row)
                    //{
                    //tweenPos.ResetToBeginning();
                    tweenPos.from = littleStarTrans.localPosition;
                    tweenPos.to = GetPos(curLStarPos.x, tempMaxPosY);
                    tweenPos.duration = unitMoveSpeed * Mathf.Abs(tempMaxPosY - curLStarPos.y);
                    curLStarPos = new Vector2(curLStarPos.x, tempMaxPosY);
                    tweenPos.PlayForward();
                    //}

                    directionBtnFrame.gameObject.SetActive(false);
                }
                break;

            case "LeftBtn":
                {
                    float tempMaxPosX = -2;
                    for (int i = 0; i < obstacleList.Count; i++)
                    {
                        if (obstacleList[i].y == curLStarPos.y && obstacleList[i].x < curLStarPos.x)
                        {
                            tempMaxPosX = Mathf.Max(tempMaxPosX, obstacleList[i].x);
                        }
                    }

                    tempMaxPosX++;

                    //if (tempMaxPosY <= row)
                    //{
                    //tweenPos.ResetToBeginning();
                    tweenPos.from = littleStarTrans.localPosition;
                    tweenPos.to = GetPos(tempMaxPosX, curLStarPos.y);
                    tweenPos.duration = unitMoveSpeed * Mathf.Abs(tempMaxPosX - curLStarPos.x);
                    curLStarPos = new Vector2(tempMaxPosX, curLStarPos.y);
                    tweenPos.PlayForward();
                    //}

                    directionBtnFrame.gameObject.SetActive(false);
                }
                break;

            case "RightBtn":
                {
                    float tempMaxPosX = col + 1;
                    for (int i = 0; i < obstacleList.Count; i++)
                    {
                        if (obstacleList[i].y == curLStarPos.y && obstacleList[i].x > curLStarPos.x)
                        {
                            tempMaxPosX = Mathf.Min(tempMaxPosX, obstacleList[i].y);
                        }
                    }

                    tempMaxPosX--;

                    //if (tempMaxPosY <= row)
                    //{
                    //tweenPos.ResetToBeginning();
                    tweenPos.from = littleStarTrans.localPosition;
                    tweenPos.to = GetPos(tempMaxPosX, curLStarPos.y);
                    tweenPos.duration = unitMoveSpeed * Mathf.Abs(tempMaxPosX - curLStarPos.x);
                    curLStarPos = new Vector2(tempMaxPosX, curLStarPos.y);
                    tweenPos.PlayForward();
                    //}

                    directionBtnFrame.gameObject.SetActive(false);
                }
                break;
        }
    }

    private void OnTweenPosFinished()
    {
        //判断输赢
        if (curLStarPos.x < 0 || curLStarPos.x > col - 1
            || curLStarPos.y < 0 || curLStarPos.y > row - 1)
        {
            Debug.Log("失败");
            return;
        }

        tweenPos.enabled = false;
        tweenPos.ResetToBeginning();
        ShowDirectionBtn();
    }
}
