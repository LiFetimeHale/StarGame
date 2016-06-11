using UnityEngine;
using System.Collections;

public class UISetting : MonoBehaviour 
{
    private static UISetting m_Instance;
    public static UISetting Instance
    {
        get { return m_Instance; }
    }

    private GameObject settingFrame;
    private GameObject gameEndFrame;
    private GameObject gameWinObj;
    private GameObject gameLoseObj;
    private UIButton lastBtn;
    private UIButton nextBtn;

    void Awake()
    {
        m_Instance = this;
        DontDestroyOnLoad(gameObject);

        InitCom();
    }

    private void InitCom()
    {
        UIButton btn = transform.Find("BackBtn").GetComponent<UIButton>();
        EventDelegate.Add(btn.onClick, OnBtnClick);
        btn = transform.Find("SettingBtn").GetComponent<UIButton>();
        EventDelegate.Add(btn.onClick, OnBtnClick);

        settingFrame = transform.Find("SettingFrame").gameObject;
        gameEndFrame = transform.Find("GameEndFrame").gameObject;

        gameWinObj = transform.Find("GameEndFrame/GameWin").gameObject;
        gameLoseObj = transform.Find("GameEndFrame/GameLose").gameObject;

        lastBtn = transform.Find("GameEndFrame/LastBtn").GetComponent<UIButton>();
        EventDelegate.Add(lastBtn.onClick, OnBtnClick);
        nextBtn = transform.Find("GameEndFrame/NextBtn").GetComponent<UIButton>();
        EventDelegate.Add(nextBtn.onClick, OnBtnClick);

        btn = transform.Find("GameEndFrame/CloseEndBtn").GetComponent<UIButton>();
        EventDelegate.Add(btn.onClick, OnBtnClick);

        btn = transform.Find("GameEndFrame/RetryBtn").GetComponent<UIButton>();
        EventDelegate.Add(btn.onClick, OnBtnClick);

        btn = transform.Find("SettingFrame/SettingCloseBtn").GetComponent<UIButton>();
        EventDelegate.Add(btn.onClick, OnBtnClick);

        UISlider uiSlider = transform.Find("SettingFrame/MusicToggle").GetComponent<UISlider>();
        if (MainData.Instance.IsPlayMusic)
            uiSlider.value = 1.0f;
        else
            uiSlider.value = 0.0f;
        EventDelegate.Add(uiSlider.onChange, OnDragFinished);
        uiSlider = transform.Find("SettingFrame/EffectToggle").GetComponent<UISlider>();
        if (MainData.Instance.IsPlayEffect)
            uiSlider.value = 1.0f;
        else
            uiSlider.value = 0.0f;
        EventDelegate.Add(uiSlider.onChange, OnDragFinished);
    }

    void OnDestory()
    {
        m_Instance = null;
    }

    private void OnBtnClick()
    {
        switch (UIButton.current.name)
        {
            case "BackBtn":
                {
                    if(UIChooseCheckpoint.Instance == null)
                        Application.LoadLevel("UIChooseCheckpoint");
                }
                break;

            case "SettingBtn":
                {
                    settingFrame.gameObject.SetActive(true);
                }
                break;

            case "SettingCloseBtn":
                {
                    settingFrame.gameObject.SetActive(false);
                }
                break;

            case "RetryBtn":
                {
                    gameEndFrame.SetActive(false);
                    StarGame.Instance.Reset();
                }
                break;

            case "CloseEndBtn":
                {
                    gameEndFrame.SetActive(false);
                    Application.LoadLevel("UIChooseCheckpoint");
                }
                break;

            case "LastBtn":
                {
                    gameEndFrame.SetActive(false);
                    MainData.Instance.CurCheckpoint--;
                    Application.LoadLevel("UIStarGame");
                }
                break;

            case "NextBtn":
                {
                    gameEndFrame.SetActive(false);
                    MainData.Instance.CurCheckpoint++;
                    Application.LoadLevel("UIStarGame");
                }
                break;
        }
    }

    private void OnDragFinished()
    {
        switch (UISlider.current.name)
        {
            case "MusicToggle":
                {
                    if (UISlider.current.value == 1.0f)
                        MainData.Instance.IsPlayMusic = true;
                    else
                        MainData.Instance.IsPlayMusic = false;
                }
                break;

            case "EffectToggle":
                {
                    if (UISlider.current.value == 1.0f)
                        MainData.Instance.IsPlayEffect = true;
                    else
                        MainData.Instance.IsPlayEffect = false;
                }
                break;
        }
    }

    public void ShowGameEnd(bool isSuc)
    {
        int curCheckpoint = MainData.Instance.CurCheckpoint;
        int unlockedCheckpoint = PlayerPrefs.GetInt(DataName.UnlockedCheckpoint);

        gameEndFrame.SetActive(true);

        if (isSuc)
        {
            gameWinObj.SetActive(true);
            gameLoseObj.SetActive(false);
            if (curCheckpoint == unlockedCheckpoint && curCheckpoint < MainData.Instance.MaxCheckpoint)
            {
                unlockedCheckpoint++;
                PlayerPrefs.SetInt(DataName.UnlockedCheckpoint, unlockedCheckpoint);
            }
        }
        else
        {
            gameLoseObj.SetActive(true);
            gameWinObj.SetActive(false);
        }

        if (curCheckpoint <= 1)
            lastBtn.gameObject.SetActive(false);
        else
            lastBtn.gameObject.SetActive(true);

        if (curCheckpoint >= MainData.Instance.MaxCheckpoint)
            nextBtn.gameObject.SetActive(false);
        else
            nextBtn.gameObject.SetActive(true);
    }
}
