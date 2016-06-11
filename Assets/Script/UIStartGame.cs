using UnityEngine;
using System.Collections;

public class UIStartGame : MonoBehaviour 
{
    void Awake()
    { 
        UIButton btn = transform.Find("StartBtn").GetComponent<UIButton>();
        EventDelegate.Add(btn.onClick, OnBtnClick);
        btn = transform.Find("CloseBtn").GetComponent<UIButton>();
        EventDelegate.Add(btn.onClick, OnBtnClick);
    }

    private void OnBtnClick()
    {
        switch (UIButton.current.name)
        {
            case "StartBtn":
                {
                    Application.LoadLevel("UIChooseCheckpoint");
                    Application.LoadLevelAdditive("UISetting");
                }
                break;

            case "CloseBtn":
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit ();
#endif
                }
                break;
        }
    }
}
