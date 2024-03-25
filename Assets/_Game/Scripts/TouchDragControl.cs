using System.Collections;
using System.Collections.Generic;
using Animancer;
using UnityEngine;
using ZagaCore;
using ZagaCore.Events.Camera;
using ZagaCore.Events.Input;
using ZagaCore.Events.Screen;

public class TouchDragControl : MonoBehaviour
{
    public class TouchState
    {
        public int TouchId;
        public TouchPhase TouchPhase;
        // World Position Tapped
        public Vector3 WorldPosition;
        // Initial World Position Tapped
        //public Vector3 InitialWorldPosition;
        // Touch Position Of Base
        public Vector3 BaseTouchPos;
        // Screen Position Of Base (converted)
        public Vector3 BaseRectPos;
        // Touch Position of Tap
        public Vector3 TapTouchPos;
        // Screen Position of Tap (converted)
        public Vector3 TapRectPos;
        // Rotation from base to tap
        public float BaseToTapRad;
        public Vector3 Direction;
    }
    
    [SerializeField] private RectTransform fullScreenRect;
    [SerializeField] private RectTransform baseRectTrans;
    [SerializeField] private RectTransform tapRectTrans;
    
    private Camera cam2d;
    private Camera cam3d;
    private ClientInput clientInput;
    private Vector2 minWindow;
    private Vector2 maxWindow;
    private Vector2 screenSize;
    private Ray ray;
    private Plane plane = new Plane(Vector3.up, Vector3.zero);
    private float dstToTap;
    private Vector3 touchWorldPos;
    private TouchState dragState = null;
    private TouchState longTapState = null;
    private Dictionary<int, TouchState> touchStateMap = new Dictionary<int, TouchState>();
    private Vector2 localSpace;
    [SerializeField] private float distanceFromEdge = 0.0f;
    [SerializeField] private float distanceFromBase = 0.0f; 
    private float dragBase = 0.0f;
    private float resolutionBase = (1920f + 1080f) * 0.5f;
    private bool dimensionsDirty = true;

    private void Awake()
    {
        clientInput = Refs.Instance.Get<ClientInput>();
        if (clientInput == null)
        {
            clientInput = new ClientInput();
            Refs.Instance.Bind(clientInput);
        }

        distanceFromEdge = dragBase = (baseRectTrans.rect.width + baseRectTrans.rect.height) * 0.25f;

        distanceFromBase = dragBase;
        
        baseRectTrans.gameObject.SetActive(false);
        tapRectTrans.gameObject.SetActive(false);
        
        Refs.Instance.Get<EventService>().Subscribe<ScreenOrientationChanged>(OnScreenOrientationChanged, 100);
        Refs.Instance.Get<EventService>().Subscribe<ScreenResolutionChanged>(OnScreenResolutionChanged, 100);
        Refs.Instance.Get<EventService>().Subscribe<Event3DCameraChanged>(OnEvent3DCameraChanged);
        Refs.Instance.Get<EventService>().Subscribe<TouchEvent>(OnTouchEvent);
        
        ForceUpdateScreenOrientation(ScreenManager.CurrentScreenOrientation);
        Refs.Instance.Get<EventService>().Dispatch<EventStartUIInput>();
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

    private void OnEvent3DCameraChanged(Event3DCameraChanged event3DCameraChanged)
    {
        ForceUpdateScreenOrientation(ScreenManager.CurrentScreenOrientation);
    }

    private void ForceUpdateScreenOrientation(ScreenOrientation screenOrientation)
    {
        dimensionsDirty = true;
        cam3d = Refs.Instance.Get<CameraService>().Camera3D;
        cam2d = Refs.Instance.Get<CameraService>().CameraUI;
        
        baseRectTrans.gameObject.SetActive(false);
        tapRectTrans.gameObject.SetActive(false);
    }
    
    private void OnAbilityAuto(TouchEvent touchEvent)
    {
        Refs.Instance.Get<EventService>().Dispatch<TryAutoAction>();
    }
    
    private void OnTouchEvent(TouchEvent touchEvent)
    {
        if (dimensionsDirty)
        {
            dimensionsDirty = false;
            
            distanceFromBase = dragBase * ((ScreenManager.Width + ScreenManager.Height) * 0.5f / resolutionBase);
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(fullScreenRect, Vector2.zero, cam2d, out minWindow);
            minWindow.x += distanceFromEdge;
            minWindow.y += distanceFromEdge;
            screenSize.x = ScreenManager.Width;
            screenSize.y = ScreenManager.Height;
        
            RectTransformUtility.ScreenPointToLocalPointInRectangle(fullScreenRect, screenSize, cam2d, out maxWindow);
            maxWindow.x -= distanceFromEdge;
            maxWindow.y -= distanceFromEdge;
        }
        
        switch (touchEvent.TouchPhase)
        {
            case TouchPhase.Began:
                OnTouchDown(touchEvent);
                break;
            
            case TouchPhase.Moved:
                OnDrag(touchEvent);
                break;
            
            case TouchPhase.Canceled:
            case TouchPhase.Ended:
                OnTouchUp(touchEvent);
                break;
        }
    }

    private void OnTouchDown(TouchEvent touchEvent)
    {
        if (!touchStateMap.TryGetValue(touchEvent.TouchId, out var touchState))
        {
            touchState = new TouchState();
            touchState.TouchId = touchEvent.TouchId;
            touchStateMap.Add(touchEvent.TouchId, touchState);
        }
        
        var state = touchStateMap[touchEvent.TouchId];
        state.TouchPhase = TouchPhase.Began;

        state.BaseTouchPos = state.TapTouchPos = touchEvent.TouchPoint;
        state.BaseTouchPos.z = state.TapTouchPos.z = 0;
        state.BaseRectPos = CalculateLocalSpace(state.BaseTouchPos, true);
        state.TapRectPos = CalculateLocalSpace(state.TapTouchPos, false);
    }

    public Vector3 CalculateLocalSpace(Vector3 touchPoint, bool inBounds)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(fullScreenRect, touchPoint, cam2d, out localSpace);

        if (inBounds)
        {
            localSpace.x = Mathf.Clamp(localSpace.x, minWindow.x, maxWindow.x);
            localSpace.y = Mathf.Clamp(localSpace.y, minWindow.y, maxWindow.y);
        }

        return localSpace;
    }
    
    private void OnTouchUp(TouchEvent touchEvent)
    {
        if(!touchStateMap.ContainsKey(touchEvent.TouchId))
            return;
         
        var state = touchStateMap[touchEvent.TouchId];
        
        if (state.TouchPhase == TouchPhase.Ended)
        {
            return;
        }
        
        switch (state.TouchPhase)
        {
            case TouchPhase.Began:
                Debug.LogFormat("Tap[{0}]", touchEvent.TouchId);
                
                // we are dragging with the other touch
                // cancel the drag and use this touch instead
                state.TouchPhase = TouchPhase.Ended;
                if ((clientInput.ControlState & ActionState.ActionLocked) == 0)
                {
                    OnAbilityAuto(touchEvent);
                }
                break;
            
            case TouchPhase.Stationary:
                Debug.Log("Ability Tap");
                state.TouchPhase = TouchPhase.Ended;
                break;
            
            case TouchPhase.Moved:
                // drag
                if (dragState != null)
                {
                    if (state == dragState)
                    {
                        baseRectTrans.gameObject.SetActive(false);
                        tapRectTrans.gameObject.SetActive(false);
                        dragState.TouchPhase = TouchPhase.Ended;
                        clientInput.InputX = 0;
                        clientInput.InputZ = 0;
                        dragState = null;
                        clientInput.ControlState |= ActionState.Idle;
                        clientInput.ControlState &= ~ActionState.Moving;
                    }
                }
                state.TouchPhase = TouchPhase.Ended;
                break;
            
        }
    }
    
    private void OnDrag(TouchEvent touchEvent)
    {
        if(!touchStateMap.ContainsKey(touchEvent.TouchId))
            return;
        
        var state = touchStateMap[touchEvent.TouchId];
        
        state.TapTouchPos = touchEvent.TouchPoint;
        state.TapTouchPos.z = 0;
        
        state.Direction = (Vector3)touchEvent.TouchPoint - state.BaseTouchPos;
        
        var distance = state.Direction.magnitude;
        
        if (distance > distanceFromBase * 0.125f)
        {
            switch (state.TouchPhase)
            {
                case TouchPhase.Began:
                    // we are already dragging with the other touch
                    // this drag should take over
                    if (dragState != null)
                    {
                        dragState.TouchPhase = TouchPhase.Ended;
                        clientInput.InputX = 0;
                        clientInput.InputZ = 0;
                        dragState = null;
                    }
                    
                    state.TouchPhase = TouchPhase.Moved;
                    
                    baseRectTrans.gameObject.SetActive(true);
                    tapRectTrans.gameObject.SetActive(true);
                    tapRectTrans.anchoredPosition = state.BaseTouchPos;
                    baseRectTrans.anchoredPosition = state.BaseRectPos;
                    dragState = state;
                    clientInput.ControlState |= ActionState.Moving;
                    clientInput.ControlState &= ~ActionState.Idle;
                    break;
            }
        }

        if (state.TouchPhase == TouchPhase.Moved && state == dragState)
        {
            var dirNormal = state.Direction.normalized;

            if (distance > distanceFromBase)
            {
                state.BaseTouchPos = state.TapTouchPos - (dirNormal *  distanceFromBase);
                state.BaseRectPos = CalculateLocalSpace(state.BaseTouchPos, true);
                
                baseRectTrans.anchoredPosition = state.BaseRectPos;
            }
            
            state.TapRectPos = CalculateLocalSpace(state.TapTouchPos, false);
            
            tapRectTrans.anchoredPosition = state.TapRectPos;
            
            clientInput.InputX = state.Direction.x / distance;
            clientInput.InputZ = state.Direction.y / distance;
        }
    }
    
    private void DetermineWorldPosition(TouchEvent touchEvent)
    {
        if (!touchStateMap.ContainsKey(touchEvent.TouchId))
            return;
        
        var state = touchStateMap[touchEvent.TouchId];

        touchWorldPos = state.TapTouchPos;
        
        ray = cam3d.ScreenPointToRay(touchWorldPos);
        dstToTap = 0.0f;
        plane.Raycast(ray, out dstToTap);
        
        state.WorldPosition = ray.GetPoint(dstToTap);
    }
}
