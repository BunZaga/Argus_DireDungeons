using UnityEngine;
using UnityEngine.UI;
using ZagaCore;
using ZagaCore.Events.Screen;

public class ViewStackControl : MonoBehaviour
{
    [SerializeField] private CanvasScaler canvasScaler;
    [SerializeField] private Vector2 baseResolution;
    [SerializeField] private ScreenOrientation currentOrientation;

    private void Awake()
    {
        baseResolution = canvasScaler.referenceResolution;
        currentOrientation = baseResolution.x > baseResolution.y
            ? ScreenOrientation.LandscapeLeft
            : ScreenOrientation.Portrait;

        if (ScreenManager.CurrentScreenOrientation != currentOrientation)
            InvertScalers(ScreenManager.CurrentScreenOrientation);
        
        Refs.Instance.Get<EventService>().Subscribe<ScreenOrientationChanged>(OnScreenOrientationChanged, 0);
        Refs.Instance.Get<EventService>().Subscribe<ScreenResolutionChanged>(OnScreenResolutionChanged, 0);
    }

    private void InvertScalers(ScreenOrientation screenOrientation)
    {
        baseResolution.x = canvasScaler.referenceResolution.y;
        baseResolution.y = canvasScaler.referenceResolution.x;

        canvasScaler.referenceResolution = baseResolution;
        canvasScaler.matchWidthOrHeight = screenOrientation == ScreenOrientation.Portrait ? 0 : 1;
        currentOrientation = screenOrientation;
        
        Debug.Log("matchWidthOrHeight:"+canvasScaler.matchWidthOrHeight);
        Debug.Log("currentOrientation:"+currentOrientation);
        Debug.Log("baseResolution:"+baseResolution.x+","+baseResolution.y);
    }
    
    private void OnDestroy()
    {
        Refs.Instance.Get<EventService>().UnSubscribe<ScreenOrientationChanged>(OnScreenOrientationChanged);
        Refs.Instance.Get<EventService>().UnSubscribe<ScreenResolutionChanged>(OnScreenResolutionChanged);
    }
    private void OnScreenOrientationChanged(ScreenOrientationChanged screenOrientationChanged)
    {
        ForceUpdateScreenOrientation(screenOrientationChanged.NewScreenOrientation);
    }

    private void OnScreenResolutionChanged(ScreenResolutionChanged screenResolutionChanged)
    {
        ForceUpdateScreenOrientation(ScreenManager.CurrentScreenOrientation);
    }
    
    private void ForceUpdateScreenOrientation(ScreenOrientation screenOrientation)
    {
        if(currentOrientation != screenOrientation)
            InvertScalers(screenOrientation);
    }
}
