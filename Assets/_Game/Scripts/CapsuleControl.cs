using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ZagaCore;
using ZagaCore.Events.Update;

public class CapsuleControl : MonoBehaviour
{
    private float playerScale = 1.0f;
    private float playerDT = 0.0f;
    private float enemyScale = 1.0f;
    private float enemyDT = 0.0f;
    
    [SerializeField] private ClientInput clientInput;
    [SerializeField] private InputActionAsset inputActionAsset;
    [SerializeField] private float speed;
    [SerializeField] private Transform camTarget;
    [SerializeField] private Transform camTrans;
    [SerializeField] private Transform proxyTrans;
    [SerializeField] private Transform visTrans;
    [SerializeField] private Transform visBottomTrans;
    
    [SerializeField] private List<SkinnedMeshRenderer> attachPiece;
    
    // make this a singleton or static
    [SerializeField] private AttachMesh attachMesh;


    [SerializeField] private AnimationClip idleAnim;
    [SerializeField] private AnimationClip runAnim;
    
    private AnimationClip currentAnim = null;
    
    private const float TORSO_SPEED_SCALE = 1.45f;
    private const float BASE_SPEED_SCALE = 1.15f;

    private SmoothFollowCam smoothFollowCam;
    private Vector3 normalizedVelocity = Vector3.zero;
    private TorsoTurn torsoTurn;
    private GamePlayControl controls;

    public class FakeDungeonLoaded : GameEvent<FakeDungeonLoaded> { };

    private void Awake()
    {
        Refs.Instance.Get<EventService>().Subscribe<FakeDungeonLoaded>(DungeonLoaded);
        controls = new GamePlayControl();

        controls.GamePlay.RunForward.started += ForwardStarted;
        controls.GamePlay.RunForward.canceled += ForwardStopped;
        controls.GamePlay.RunBack.started += BackStarted;
        controls.GamePlay.RunBack.canceled += BackStopped;
        controls.GamePlay.RunLeft.started += LeftStarted;
        controls.GamePlay.RunLeft.canceled += LeftStopped;
        controls.GamePlay.RunRight.started += RightStarted;
        controls.GamePlay.RunRight.canceled += RightStopped;
        controls.GamePlay.AutoAction.performed += AutoAction;
    }

    private void AutoAction(InputAction.CallbackContext callbackContext)
    {
        Refs.Instance.Get<EventService>().Dispatch<TryAutoAction>();
        ChangeAnimation(idleAnim);
    }
    
    private void ForwardStarted(InputAction.CallbackContext ctx)
    {
        clientInput.InputZ = 1;
    }
    
    private void ForwardStopped(InputAction.CallbackContext ctx)
    {
        clientInput.InputZ = controls.GamePlay.RunBack.inProgress ? -1 : 0;
    }
    
    private void BackStarted(InputAction.CallbackContext ctx)
    {
        clientInput.InputZ = -1;
    }
    
    private void BackStopped(InputAction.CallbackContext ctx)
    {
        clientInput.InputZ = controls.GamePlay.RunForward.inProgress ? 1 : 0;
    }
    
    private void LeftStarted(InputAction.CallbackContext ctx)
    {
        clientInput.InputX = -1;
    }
    
    private void LeftStopped(InputAction.CallbackContext ctx)
    {
        clientInput.InputX = controls.GamePlay.RunRight.inProgress ? 1 : 0;
    }
    
    private void RightStarted(InputAction.CallbackContext ctx)
    {
        clientInput.InputX = 1;
    }
    
    private void RightStopped(InputAction.CallbackContext ctx)
    {
        clientInput.InputX = controls.GamePlay.RunLeft.inProgress ? -1 : 0;
    }
    
    private void DungeonLoaded(FakeDungeonLoaded fakeDungeonLoaded)
    {
        clientInput = Refs.Instance.Get<ClientInput>();
        if (clientInput == null)
        {
            clientInput = new ClientInput();
            Refs.Instance.Bind(clientInput);
        }

        for (int i = 0, ilen = attachPiece.Count; i < ilen; ++i)
        {
            attachMesh.CreateAndAttachMeshPiece(attachPiece[i]);
        }
        
        Refs.Instance.Get<EventService>().Subscribe<EventGameUpdate>(GameUpdate);
        Refs.Instance.Get<EventService>().Subscribe<EventGameLateUpdate>(GameLateUpdate);

        smoothFollowCam = new SmoothFollowCam(camTarget, camTrans);
        torsoTurn = attachMesh.TransformMap.gameObject.GetComponent<TorsoTurn>();
        ChangeAnimation(idleAnim);
        controls?.GamePlay.Enable();
    }
    
    private float wantRot = 0.0f;
    private float curentWantRot = 0.0f;
    private float oldRot = 0.0f;
    private float oldBaseRot = 0.0f;
    private float tic = 0.0f;
    private float baseTic = 0.0f;
    
    private float wantVel;
    AnimationClip wantAnim = null;

    private void ChangeAnimation(AnimationClip animClip)
    {
        if (currentAnim != animClip)
        {
            currentAnim = animClip;
            torsoTurn.SetAnimation(animClip);
            Debug.Log("changed anim to "+animClip.name);
        }
    }
    
    private void GameUpdate()
    {
        var pos = proxyTrans.position;
        normalizedVelocity.x = clientInput.InputX;
        normalizedVelocity.z = clientInput.InputZ;
        normalizedVelocity.y = 0;
        if (normalizedVelocity.magnitude > 0)
        {
            clientInput.ControlState |= ActionState.Moving;
            clientInput.ControlState &= ~ActionState.Idle;
            normalizedVelocity.Normalize();
            wantAnim = runAnim;
        }
        else
        {
            clientInput.ControlState &= ~ActionState.Moving;
            clientInput.ControlState |= ActionState.Idle;
            wantAnim = idleAnim;
        }

        pos.x += normalizedVelocity.x * speed * Time.deltaTime;
        pos.z += normalizedVelocity.z * speed * Time.deltaTime;

        if ((clientInput.ControlState & ActionState.MovingLocked) == 0)
        {
            proxyTrans.position = pos;
            visTrans.position = pos;
            visBottomTrans.position = pos;

            if (normalizedVelocity.x != 0 || normalizedVelocity.z != 0)
            {
                var newRot = Mathf.Atan2(normalizedVelocity.x, normalizedVelocity.z) * Mathf.Rad2Deg;

                if (Mathf.Abs(newRot - wantRot) > 0.01f)
                {
                    wantRot = newRot;
                    oldRot = visTrans.rotation.eulerAngles.y;
                    tic = 0;
                }

                var diffCurRotWantRot = curentWantRot - wantRot;

                if (Mathf.Abs(diffCurRotWantRot) > 0.01f)
                {
                    if (diffCurRotWantRot > 180)
                        diffCurRotWantRot -= 360;

                    if (diffCurRotWantRot < -180)
                        diffCurRotWantRot += 360;

                    curentWantRot = wantRot;
                }

                if (tic > -1)
                {
                    if (tic < 1.0f)
                    {
                        visTrans.rotation = Quaternion.Euler(0,
                            Mathf.LerpAngle(oldRot, curentWantRot, tic += Time.deltaTime * speed * TORSO_SPEED_SCALE),
                            0);
                    }
                    else
                    {
                        tic = -1;
                        visTrans.rotation = Quaternion.Euler(0, curentWantRot, 0);
                    }
                }
            }


            var wantBaseRot = visTrans.rotation.eulerAngles.y;
            var currentBaseRot = visBottomTrans.rotation.eulerAngles.y;

            var diffRot = Mathf.Abs(wantBaseRot - currentBaseRot);
            if (diffRot > 0.01f)
            {
                baseTic = 0.0f;
                oldBaseRot = currentBaseRot;
            }

            if (baseTic > -1)
            {
                if (baseTic < 1.0f)
                {
                    visBottomTrans.rotation = Quaternion.Euler(0,
                        Mathf.LerpAngle(oldBaseRot, wantBaseRot, baseTic += Time.deltaTime * speed * BASE_SPEED_SCALE),
                        0);
                }
                else
                {
                    baseTic = -1;
                    visBottomTrans.rotation = Quaternion.Euler(0, wantBaseRot, 0);
                }
            }

            torsoTurn.UpdateRoot();

            proxyTrans.position = visBottomTrans.position;
        }

        if((clientInput.ControlState & ActionState.ActionLocked) == 0)
            ChangeAnimation(wantAnim);
    }

    private void GameLateUpdate()
    {
        smoothFollowCam.ControlledLateUpdate();
    }
    
    private void OnDestroy()
    {
        Refs.Instance?.Get<EventService>()?.UnSubscribe<EventGameUpdate>(GameUpdate);
        Refs.Instance.Get<EventService>().UnSubscribe<EventGameLateUpdate>(GameLateUpdate);
        controls?.GamePlay.Disable();
    }
}
