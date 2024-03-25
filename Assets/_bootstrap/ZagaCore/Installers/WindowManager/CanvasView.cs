using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasGroup))]
public class CanvasView : MonoBehaviour
{
    public GraphicRaycaster GraphicRaycaster => graphicRaycaster;
    [SerializeField] protected GraphicRaycaster graphicRaycaster;
    public Canvas Canvas => canvas;
    [SerializeField] protected Canvas canvas;
    public CanvasGroup CanvasGroup => canvasGroup;
    [SerializeField] protected CanvasGroup canvasGroup;
    public Animator Animator => animator;
    [SerializeField] protected Animator animator;

    public bool HasAnimator => animator != null;
    public bool IsActive = false;
}
