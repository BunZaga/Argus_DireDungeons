using System.Collections.Generic;
using UnityEngine;
using ZagaCore;
using ZagaCore.Events.Camera;
using ZagaCore.Events.Input;
using ZagaCore.Events.Update;

public class InputService : MonoBehaviour
{
    private Refs refs => _refs ?? Refs.Instance;
    private Refs _refs;

    private EventService eventService => _eventService ?? (_eventService = Refs.Instance.Get<EventService>());
    private EventService _eventService;

    private Dictionary<int, GameObject> objectDownUI = new Dictionary<int, GameObject>();
    private GameObject objectDown3D;
    
    private Camera currentUICam;
    private Camera current3DCam;

    private Ray rayUI;
    private RaycastHit2D hitUI;
    
    private Ray ray3D;
    private RaycastHit hit3D;

    private void Awake()
    {
        eventService.Subscribe<Event3DCameraChanged>(On3DCamChanged);
        eventService.Subscribe<EventUICameraChanged>(OnUICamChanged);
        
        eventService.Subscribe<EventStartUIInput>(OnStartUIInput);
        eventService.Subscribe<EventStopUIInput>(OnStopUIInput);
        refs.Bind(this);
    }

    private void OnStartUIInput()
    {
        OnStopUIInput();
        if (currentUICam != null)
        {
            eventService.Subscribe<EventGameUpdate>(UpdateUI, 100);
        }
    }

    private void OnStopUIInput()
    {
        eventService.UnSubscribe<EventGameUpdate>(UpdateUI);
    }
    
    private void OnUICamChanged(EventUICameraChanged evnt)
    {
        currentUICam = evnt.NewUICamera;
    }
    
    private void On3DCamChanged(Event3DCameraChanged evnt)
    {
        if (current3DCam == null)
        {
            eventService.Subscribe<EventGameUpdate>(Update3D, 9000);
        }
        current3DCam = evnt.New3DCamera;
    }

    private void HandleTouch(int touchId, Vector2 touchPosition, TouchPhase touchPhase)
    {
        switch (touchPhase)
        {
            case TouchPhase.Began:
                if (objectDownUI.TryGetValue(touchId, out var objBegan))
                {
                    var touchUp = objBegan.GetComponent<ITouchUp>();
                    if (touchUp != null)
                    {
                        var touchEvent = eventService.GetPooledEvent<TouchEvent>();
                        touchEvent.TouchId = touchId;
                        touchEvent.TouchPhase = TouchPhase.Ended;
                        touchEvent.TouchPoint = touchPosition;
                        touchUp.OnTouchUp(touchEvent);
                    }
                }
                objectDownUI.Remove(touchId);
                
                rayUI = currentUICam.ScreenPointToRay(touchPosition);
                hitUI = Physics2D.GetRayIntersection(rayUI, 1000);
                if(hitUI.collider != null)
                {
                    var hitObject = hitUI.transform.gameObject;
                    if (hitObject != null)
                    {
                        var touchDown = hitObject.GetComponent<ITouchDown>();
                        if (touchDown != null)
                        {
                            var touchEvent = eventService.GetPooledEvent<TouchEvent>();
                            touchEvent.TouchId = touchId;
                            touchEvent.TouchPhase = TouchPhase.Began;
                            touchEvent.TouchPoint = touchPosition;
                            touchDown.OnTouchDown(touchEvent);
                        }
                        
                        objectDownUI[touchId] = hitObject;
                    }
                }
                else
                {
                    var touchEvent = eventService.GetPooledEvent<TouchEvent>();
                    touchEvent.TouchId = touchId;
                    touchEvent.TouchPhase = TouchPhase.Began;
                    touchEvent.TouchPoint = touchPosition;
                    eventService.Dispatch(touchEvent);
                }
                break;
            case TouchPhase.Moved:
                if (objectDownUI.TryGetValue(touchId, out var objMoved))
                {
                    var drag = objMoved.GetComponent<IDrag>();
                    if (drag != null)
                    {
                        var touchEvent = eventService.GetPooledEvent<TouchEvent>();
                        touchEvent.TouchId = touchId;
                        touchEvent.TouchPhase = TouchPhase.Moved;
                        touchEvent.TouchPoint = touchPosition;
                        drag.OnDrag(touchEvent);
                    }
                }
                else
                {
                    var touchEvent = eventService.GetPooledEvent<TouchEvent>();
                    touchEvent.TouchId = touchId;
                    touchEvent.TouchPhase = TouchPhase.Moved;
                    touchEvent.TouchPoint = touchPosition;
                    eventService.Dispatch(touchEvent);
                }
                break;
            case TouchPhase.Ended:
                if (objectDownUI.TryGetValue(touchId, out var objEnded))
                {
                    var touchUp = objEnded.GetComponent<ITouchUp>();
                    if (touchUp != null)
                    {
                        var touchEvent = eventService.GetPooledEvent<TouchEvent>();
                        touchEvent.TouchId = touchId;
                        touchEvent.TouchPhase = TouchPhase.Ended;
                        touchEvent.TouchPoint = touchPosition;
                        touchUp.OnTouchUp(touchEvent);
                    }
                }
                else
                {
                    var touchEvent = eventService.GetPooledEvent<TouchEvent>();
                    touchEvent.TouchId = touchId;
                    touchEvent.TouchPhase = TouchPhase.Ended;
                    touchEvent.TouchPoint = touchPosition;
                    eventService.Dispatch(touchEvent);
                }

                rayUI = currentUICam.ScreenPointToRay(touchPosition);
                hitUI = Physics2D.GetRayIntersection(rayUI, 1000);
                if (hitUI.collider != null)
                {
                    var hitObject = hitUI.collider.gameObject;
                    if (hitObject != null)
                    {
                        if (objectDownUI.ContainsKey(touchId) && objectDownUI[touchId] == hitObject)
                        {
                            var tapped = hitObject.GetComponent<ITapped>();
                            if (tapped != null)
                            {
                                var touchEvent = eventService.GetPooledEvent<TouchEvent>();
                                touchEvent.TouchId = touchId;
                                touchEvent.TouchPhase = TouchPhase.Ended;
                                touchEvent.TouchPoint = touchPosition;
                                tapped.OnTapped(touchEvent);
                            }
                        }
                    }
                }

                objectDownUI.Remove(touchId);

                break;
            case TouchPhase.Canceled:
                if (objectDownUI.TryGetValue(touchId, out var objCancelled))
                {
                    var touchUp = objCancelled.GetComponent<ITouchUp>();
                    if (touchUp != null)
                    {
                        var touchEvent = eventService.GetPooledEvent<TouchEvent>();
                        touchEvent.TouchId = touchId;
                        touchEvent.TouchPhase = TouchPhase.Canceled;
                        touchEvent.TouchPoint = touchPosition;
                        touchUp.OnTouchUp(touchEvent);
                    }
                }
                else
                {
                    var touchEvent = eventService.GetPooledEvent<TouchEvent>();
                    touchEvent.TouchId = touchId;
                    touchEvent.TouchPhase = TouchPhase.Canceled;
                    touchEvent.TouchPoint = touchPosition;
                    eventService.Dispatch(touchEvent);
                }
                break;
        }
    }
    
#if UNITY_ANDROID
    private void UpdateUI()
    {
        if (Input.touches.Length > 0)
        {
            for (int i = 0, ilen = Input.touches.Length; i < ilen; i++)
            {
                var touch = Input.touches[i];
                HandleTouch(touch.fingerId, touch.position, touch.phase);
            }
        }
    }
    #endif
    
    #if ((UNITY_STEAM || UNITY_EDITOR) && !(UNITY_ANDROID || UNITY_IOS))
    private void UpdateUI()
    {
        if (Input.GetMouseButtonUp(0))
        {
            HandleTouch(999, Input.mousePosition, TouchPhase.Ended);
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            HandleTouch(999, Input.mousePosition, TouchPhase.Began);
            return;
        }

        if (Input.GetMouseButton(0))
        {
            HandleTouch(999, Input.mousePosition, TouchPhase.Moved);
        }
    }
    #endif

    private void Update3D()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (objectDown3D != null)
            {
                var touchUp = objectDown3D.GetComponent<ITouchUp>();
                if (touchUp != null)
                {
                    var touchEvent = eventService.GetPooledEvent<TouchEvent>();
                    touchEvent.TouchId = 9999;
                    touchEvent.TouchPhase = TouchPhase.Ended;
                    touchEvent.TouchPoint = Input.mousePosition;
                    touchUp.OnTouchUp(touchEvent);
                }
            }
            
            ray3D = current3DCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray3D, out hit3D, 100.0f))
            {
                var hitObject = hit3D.transform.gameObject;
                if (hitObject != null)
                {
                    if (objectDown3D == hitObject)
                    {
                        var tapped = objectDown3D.GetComponent<ITapped>();
                        if (tapped != null)
                        {
                            var touchEvent = eventService.GetPooledEvent<TouchEvent>();
                            touchEvent.TouchId = 9999;
                            touchEvent.TouchPhase = TouchPhase.Ended;
                            touchEvent.TouchPoint = Input.mousePosition;
                            tapped.OnTapped(touchEvent);
                        }
                    }
                }
            }
            objectDown3D = null;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (objectDown3D != null)
            {
                var touchUp = objectDown3D.GetComponent<ITouchUp>();
                if (touchUp != null)
                {
                    var touchEvent = eventService.GetPooledEvent<TouchEvent>();
                    touchEvent.TouchId = 9999;
                    touchEvent.TouchPhase = TouchPhase.Ended;
                    touchEvent.TouchPoint = Input.mousePosition;
                    touchUp.OnTouchUp(touchEvent);
                }
            }
            
            objectDown3D = null;
            
            ray3D = current3DCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray3D, out hit3D, 100.0f))
            {
                var hitObject = hit3D.transform.gameObject;
                if (hitObject != null)
                {
                    var touchDown = hitObject.GetComponent<ITouchDown>();
                    if (touchDown != null)
                    {
                        var touchEvent = eventService.GetPooledEvent<TouchEvent>();
                        touchEvent.TouchId = 9999;
                        touchEvent.TouchPhase = TouchPhase.Began;
                        touchEvent.TouchPoint = Input.mousePosition;
                        touchDown?.OnTouchDown(touchEvent);
                    }
                    
                    objectDown3D = hitObject;
                }
            }
        }
    }
}