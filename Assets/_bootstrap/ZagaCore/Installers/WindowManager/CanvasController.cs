using System;
using System.Collections.Generic;
using UnityEngine;
using ZagaCore;
using ZagaCore.Events.Loading;
using ZagaCore.Events.Screen;

public enum CanvasAnimatorState
{
    None,
    Opening,
    Closing
}

public class CanvasController : MonoBehaviour
{
    protected static Refs refs => _refs ?? (_refs = Refs.Instance);
    protected static Refs _refs;
    protected static EventService eventService => _eventService ?? (_eventService = Refs.Instance.Get<EventService>());
    protected static EventService _eventService;
    protected static ScreenManager screenManager => _screenManager ?? (_screenManager = Refs.Instance.Get<ScreenManager>());
    protected static ScreenManager _screenManager;

    protected static int showHash => _showHash == 0 ? _showHash = Animator.StringToHash(SHOW) : _showHash;
    protected static int _showHash;
    protected static int hideHash => _hideHash == 0 ? _hideHash = Animator.StringToHash(HIDE) : _hideHash;
    protected static int _hideHash;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Animator animator;
    

    public CanvasAnimatorState CanvasAnimatorState => canvasTransitionState;
    public CanvasAnimatorState canvasTransitionState = CanvasAnimatorState.None;

    public const string SHOW = "Show";
    public const string HIDE = "Hide";

    private Action onTransitionCompleteCallback;

    public bool IsActive => isActive;
    private bool isActive = false;

    protected virtual void Awake()
    {
        if(animator!= null)
        {
            animator.enabled = false;
        }
        canvasGroup.alpha = 0.0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        isActive = false;
    }

    /// <summary>
    /// Called from WindowManager.OnWindowShow, When Window is created, before anything else.
    /// </summary>
    public virtual void Create()
    { }

    /// <summary>
    /// Called from ShowView, every time window is about to be displayed.
    /// </summary>
    public virtual void Initialize()
    {
        eventService.UnSubscribe<ScreenOrientationChanged>(OnScreenOrientationChanged);
        eventService.Subscribe<ScreenOrientationChanged>(OnScreenOrientationChanged);
    }

    public virtual void Terminate()
    {
        eventService.UnSubscribe<ScreenOrientationChanged>(OnScreenOrientationChanged);
    }

    protected virtual void OnScreenOrientationChanged(ScreenOrientationChanged evnt)
    {

    }

    public virtual void ShowView(Action callback = null)
    {
        Debug.Log("CanvasController ShowView");
        Initialize();
        isActive = true;
        onTransitionCompleteCallback = callback;
        if (animator != null)
        {
            canvasTransitionState = CanvasAnimatorState.Opening;
            animator.enabled = true;
            animator.SetTrigger(showHash);
            return;
        }
        Show();
    }

    protected virtual void Show()
    {
        canvasTransitionState = CanvasAnimatorState.None;
        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        onTransitionCompleteCallback?.Invoke();
        onTransitionCompleteCallback = null;
    }

    public virtual void HideView(Action callback = null)
    {
        isActive = false;
        onTransitionCompleteCallback = callback;

        if (animator != null)
        {
            canvasTransitionState = CanvasAnimatorState.Closing;
            animator.enabled = true;
            animator.SetTrigger(hideHash);
            return;
        }

        Hide();
    }

    protected virtual void Hide()
    {
        canvasTransitionState = CanvasAnimatorState.None;
        canvasGroup.alpha = 0.0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        onTransitionCompleteCallback?.Invoke();
        onTransitionCompleteCallback = null;
        Terminate();
    }
}
