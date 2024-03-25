using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZagaCore.Events.Screen;

public class AutoRotatingCanvasController : CanvasController
{
    [Tooltip("0 = Vertical, 1 = Horizontal")]
    [SerializeField] protected List<CanvasView> views;
    
    public override void Initialize()
    {
        base.Initialize();
        views.ForEach((view) =>
        {
            view.Canvas.enabled = false;
            view.CanvasGroup.alpha = 0.0f;
            view.CanvasGroup.blocksRaycasts = false;
            view.CanvasGroup.interactable = false;
            view.IsActive = false;
            if (view.HasAnimator)
            {
                view.Animator.enabled = false;
            }
        });
    }

    protected override void OnScreenOrientationChanged(ScreenOrientationChanged evnt)
    {
        Show();
    }

    
    public override void ShowView(Action callback = null)
    {
        Debug.Log("AutoRotating ShowView");
        base.ShowView(callback);

        if (views.Count == 0)
        {
            return;
        }
        views[0].Canvas.enabled = views[0].GraphicRaycaster.enabled = screenManager.IsVertical;
        views[1].Canvas.enabled = views[1].GraphicRaycaster.enabled = screenManager.IsHorizontal;

        views.ForEach((view) =>
        {
            if (view.Animator != null)
            {
                view.Animator.enabled = true;
                view.Animator.SetTrigger(showHash);
            }
            else
            {
                if (view.Canvas.enabled)
                {
                    view.CanvasGroup.alpha = 1.0f;
                }
            }
        });
    }

    protected override void Show()
    {
        base.Show();
        if (views.Count == 0)
        {
            return;
        }
        views[0].Canvas.enabled = views[0].CanvasGroup.blocksRaycasts = views[0].CanvasGroup.interactable = views[0].GraphicRaycaster.enabled = screenManager.IsVertical;
        views[0].CanvasGroup.alpha = screenManager.IsVertical ? 1.0f : 0.0f;


        views[1].Canvas.enabled = views[1].CanvasGroup.blocksRaycasts = views[1].CanvasGroup.interactable = views[1].GraphicRaycaster.enabled = screenManager.IsHorizontal;
        views[1].CanvasGroup.alpha = screenManager.IsHorizontal ? 1.0f : 0.0f;
    }

    public override void HideView(Action callback = null)
    {
        base.HideView(callback);

        views.ForEach((view) =>
        {
            if (view.Animator != null)
            {
                view.Animator.enabled = true;
                view.Animator.SetTrigger(hideHash);
            }
        });
    }

    protected override void Hide()
    {
        base.Hide();
        if (views.Count == 0)
        {
            return;
        }
        views[0].Canvas.enabled = views[0].CanvasGroup.blocksRaycasts = views[0].CanvasGroup.interactable = views[0].GraphicRaycaster.enabled = false;
        views[0].CanvasGroup.alpha = 0.0f;


        views[1].Canvas.enabled = views[1].CanvasGroup.blocksRaycasts = views[1].CanvasGroup.interactable = views[1].GraphicRaycaster.enabled = false;
        views[1].CanvasGroup.alpha = 0.0f;
    }
}
