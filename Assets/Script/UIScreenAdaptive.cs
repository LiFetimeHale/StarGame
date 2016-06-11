using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(UIRoot))]
public class UIScreenAdaptive : MonoBehaviour
{
    public int aspectRatioWidth = 1280;
    public int aspectRatioHeight = 720;
    public bool runOnlyOnce = false;
    private UIRoot mRoot;
    private bool mStarted = false;

    private void Awake()
    {
        UICamera.onScreenResize += OnScreenResize;
    }

    private void OnDestroy()
    {
        UICamera.onScreenResize -= OnScreenResize;
    }

    private void Start()
    {
        mRoot = NGUITools.FindInParents<UIRoot>(this.gameObject);

        mRoot.scalingStyle = UIRoot.Scaling.Flexible;

        this.Update();
        mStarted = true;
    }

    private void OnScreenResize()
    {
        if (mStarted && runOnlyOnce)
        {
            this.Update();
        }
    }

    private void Update()
    {
        float defaultAspectRatio = aspectRatioWidth * 1f / aspectRatioHeight;
        float currentAspectRatio = Screen.width * 1f / Screen.height;

        if (defaultAspectRatio > currentAspectRatio)
        {
            int horizontalManualHeight = Mathf.FloorToInt(aspectRatioWidth / currentAspectRatio);
            mRoot.manualHeight = horizontalManualHeight;
        }
        else
        {
            mRoot.manualHeight = aspectRatioHeight;
        }

        if (runOnlyOnce && Application.isPlaying)
        {
            this.enabled = false;
        }
    }
}
