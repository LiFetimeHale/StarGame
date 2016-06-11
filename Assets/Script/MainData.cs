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

public static class DataName
{
    public const string UnlockedCheckpoint = "UnlockedCheckpoint";
    public const string IsPlayMusic = "IsPlayMusic";
    public const string IsPlayEffect = "IsPlayEffect";
}

public class MainData : MonoBehaviour
{
    private static MainData m_Instance;
    public static MainData Instance
    {
        get { return m_Instance; }
    }

    public List<GameData> gameDatas = new List<GameData>();

    private int m_CurCheckpoint;
    public int CurCheckpoint
    {
        get 
        {
            return m_CurCheckpoint; 
        }

        set { m_CurCheckpoint = value; }
    }

    public int MaxCheckpoint
    {
        get{ return gameDatas.Count; }
    }

    private bool m_IsPlayMusic;
    public bool IsPlayMusic 
    {
        get { return m_IsPlayMusic; }
        set
        {
            if (m_IsPlayMusic != value)
            {
                m_IsPlayMusic = value;
                if (m_IsPlayMusic)
                    PlayerPrefs.SetInt(DataName.IsPlayMusic, 1);
                else
                    PlayerPrefs.SetInt(DataName.IsPlayMusic, 0);

                if (StarGame.Instance != null)
                    StarGame.Instance.PlayBGM(value);
            }
        }
    }

    private bool m_IsPlayEffect;
    public bool IsPlayEffect
    {
        get { return m_IsPlayEffect; }
        set
        {
            if (m_IsPlayEffect != value)
            {
                m_IsPlayEffect = value;
                if (m_IsPlayEffect)
                    PlayerPrefs.SetInt(DataName.IsPlayEffect, 1);
                else
                    PlayerPrefs.SetInt(DataName.IsPlayEffect, 0);
            }
        }
    }

    void Awake()
    {
        m_Instance = this;
        DontDestroyOnLoad(gameObject);
        InitData();
    }

    private void InitData()
    {
        if (PlayerPrefs.HasKey(DataName.UnlockedCheckpoint))
        {
            m_CurCheckpoint = PlayerPrefs.GetInt(DataName.UnlockedCheckpoint);
        }
        else
        {
            m_CurCheckpoint = 1;
            PlayerPrefs.SetInt(DataName.UnlockedCheckpoint, 1);
        }

        if (PlayerPrefs.HasKey(DataName.IsPlayMusic))
        {
            if (PlayerPrefs.GetInt(DataName.IsPlayMusic) == 1)
                m_IsPlayMusic = true;
            else
                m_IsPlayMusic = false;
        }
        else
        {
            m_IsPlayMusic = true;
            PlayerPrefs.SetInt(DataName.IsPlayMusic, 1);
        }

        if (PlayerPrefs.HasKey(DataName.IsPlayEffect))
        {
            if (PlayerPrefs.GetInt(DataName.IsPlayEffect) == 1)
                m_IsPlayEffect = true;
            else
                m_IsPlayEffect = false;
        }
        else
        {
            m_IsPlayEffect = true;
            PlayerPrefs.SetInt(DataName.IsPlayEffect, 1);
        }
    }

    void OnDestory()
    {
        m_Instance = null;
    }
}