using UnityEngine;
using System.Collections;

public class UIChooseCheckpoint : MonoBehaviour
{
    private static UIChooseCheckpoint m_Instance;
    public static UIChooseCheckpoint Instance
    {
        get { return m_Instance; }
    }

    private UILabel cpDesc;
    private UIButton lastBtn;
    private UIButton nextBtn;
    private int iCurCheckpoint;
    private int iMaxCheckpoint;
    private int iUnlockedCheckpoint;

    void Awake()
    {
        m_Instance = this;

        iUnlockedCheckpoint = PlayerPrefs.GetInt(DataName.UnlockedCheckpoint);
        iCurCheckpoint = iUnlockedCheckpoint;
        iMaxCheckpoint = MainData.Instance.MaxCheckpoint;         

        UIButton btn = transform.Find("ChooseBtn/ChooseBtn").GetComponent<UIButton>();
        EventDelegate.Add(btn.onClick, OnBtnClick);
        lastBtn = transform.Find("LastBtn").GetComponent<UIButton>();
        EventDelegate.Add(lastBtn.onClick, OnBtnClick);
        nextBtn = transform.Find("NextBtn").GetComponent<UIButton>();
        EventDelegate.Add(nextBtn.onClick, OnBtnClick);

        if (iCurCheckpoint <= 1)
            lastBtn.gameObject.SetActive(false);

        if (iCurCheckpoint >= iUnlockedCheckpoint)
            nextBtn.gameObject.SetActive(false);

        cpDesc = transform.Find("ChooseBtn/Label").GetComponent<UILabel>();
        cpDesc.text = iCurCheckpoint.ToString();
    }

    void OnDestroy()
    {
        m_Instance = null;
    }

    private void OnBtnClick()
    {
        switch(UIButton.current.name)
        {
            case "ChooseBtn":
                {
                    MainData.Instance.CurCheckpoint = iCurCheckpoint;
                    Application.LoadLevel("UIStarGame");
                }
                break;

            case "LastBtn":
                {
                    iCurCheckpoint--;
                    cpDesc.text = iCurCheckpoint.ToString();
                    if (iCurCheckpoint <= 1)
                        lastBtn.gameObject.SetActive(false);

                    if(!nextBtn.gameObject.activeSelf && iCurCheckpoint < iMaxCheckpoint)
                        nextBtn.gameObject.SetActive(true);
                }
                break;

            case "NextBtn":
                {
                    iCurCheckpoint++;
                    cpDesc.text = iCurCheckpoint.ToString();
                    if (iCurCheckpoint >= iUnlockedCheckpoint)
                        nextBtn.gameObject.SetActive(false);

                    if (!lastBtn.gameObject.activeSelf && iCurCheckpoint > 1)
                        lastBtn.gameObject.SetActive(true);
                }
                break;
        }
    }
}
