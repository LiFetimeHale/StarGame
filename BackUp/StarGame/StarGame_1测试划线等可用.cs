using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StarGame : MonoBehaviour 
{
    private class MapInfo
    {
        public int posX;
        public int posY;
        public int occupy;

        public MapInfo(int newPosX, int newPosY, int newOccupy)
        {
            posX = newPosX;
            posY = newPosY;
            occupy = newOccupy;
        }
    }

    public int col = 5;
    public int row = 5;
    public Vector2 littleStarPos;
    public Vector2 bigStarPos;

    public List<Vector2> obstacleList = new List<Vector2>();
    private List<MapInfo> mapInfoList = new List<MapInfo>();
    private Transform littleStarTrans;
    private Transform bigStarTrans;

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
        bigStarTrans = transform.Find("BigStar");
    }

    private void DrawMapLine()
    {
        GameObject prefab = Resources.Load("Prefab/Line") as GameObject;
        if (prefab == null)
        {
            Debug.LogError("获取不到线预制体");
            return;
        }

        int i;
        GameObject mapObj = new GameObject("Map");
        for (i = 0; i < col + 1; i++)
        {
            GameObject lineClone = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
            lineClone.transform.SetParent(mapObj.transform);
            lineClone.transform.localScale = Vector3.one;
            lineClone.transform.localPosition = new Vector2((-(float)col / 2 + i) * iUnitSize, 0);
            
            UISprite lineSp = lineClone.transform.GetComponent<UISprite>();
            lineSp.SetDimensions(iUnitSize, iUnitSize * row);
        }

        for (i = 0; i < row + 1; i++)
        {
            GameObject lineClone = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
            lineClone.transform.SetParent(mapObj.transform);
            lineClone.transform.localScale = Vector3.one;
            lineClone.transform.localEulerAngles = new Vector3(0, 0, 90.0f);
            lineClone.transform.localPosition = new Vector2(0, (-(float)row / 2 + i) * iUnitSize);

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
            obstacleClone.transform.localPosition = new Vector2((-(float)col / 2 + obstacleList[i].x + 0.5f) * iUnitSize, 
                (-(float)row / 2 + obstacleList[i].y + 0.5f) * iUnitSize);
        }
    }


    void Start()
    {
        InitData();
    }

    private void InitData()
    {
        littleStarTrans.localPosition = new Vector2((-(float)col / 2 + littleStarPos.x + 0.5f) * iUnitSize,
                (-(float)row / 2 + littleStarPos.y + 0.5f) * iUnitSize);

        bigStarTrans.localPosition = new Vector2((-(float)col / 2 + bigStarPos.x + 0.5f) * iUnitSize,
                (-(float)row / 2 + bigStarPos.y + 0.5f) * iUnitSize);
    }
}
