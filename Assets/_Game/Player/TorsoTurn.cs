using System;
using Animancer;
using UnityEngine;

public class TorsoTurn : MonoBehaviour
{
    [SerializeField] private AnimancerComponent animancerComponent;

    protected Animator animator;

    [SerializeField] private bool ikActive = false;
    [SerializeField] private Transform bodyRoot = null;
    [SerializeField] private Transform lookObj = null;
    
    private void Awake()
    {
        animancerComponent.Layers[0].ApplyAnimatorIK = true;
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SetAnimation(AnimationClip newAnimation)
    {
        animancerComponent.Play(newAnimation, 0.25f, FadeMode.FixedDuration);
    }
    
    public void UpdateRoot()
    {
        //transform.position = bodyRoot.position;
        transform.rotation = bodyRoot.rotation;
    }
    
    void OnAnimatorIK()
    {
        if (!animator || !ikActive)
            return;
        
        if(lookObj != null)
        {
            animator.SetLookAtWeight(1, 0.15f, 0.9f, 1, 0.5f);
            animator.SetLookAtPosition(lookObj.position);
        }    
    }
}
