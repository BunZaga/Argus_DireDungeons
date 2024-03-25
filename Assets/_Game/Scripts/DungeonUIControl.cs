using UnityEngine;
using ZagaCore;
using ZagaCore.Events.Camera;
using ZagaCore.Events.Screen;

public class DungeonUIControl : AutoRotatingCanvasController
{ 
    private CameraService cameraService => _cameraService ?? (_cameraService = Refs.Instance.Get<CameraService>());
    private CameraService _cameraService;
    
    protected override void Awake()
    {
        base.Awake();
        views.ForEach((canvas)=>canvas.Canvas.worldCamera = cameraService.CameraUI);
        Refs.Instance.Get<EventService>().Subscribe<EventUICameraChanged>(OnUICameraChanged);
        Refs.Instance.Get<EventService>().Subscribe<ScreenOrientationChanged>(OnScreenOrientationChanged);
    }

    private void OnUICameraChanged(EventUICameraChanged camChanged)
    {
        views.ForEach((canvas)=>canvas.Canvas.worldCamera = cameraService.CameraUI);
    }
    
    protected override void OnScreenOrientationChanged(ScreenOrientationChanged screenOrientationChanged)
    {
        base.OnScreenOrientationChanged(screenOrientationChanged);
    }

    private void OnDestroy()
    {
        Refs.Instance.Get<EventService>().UnSubscribe<EventUICameraChanged>(OnUICameraChanged);
        Refs.Instance.Get<EventService>().UnSubscribe<ScreenOrientationChanged>(OnScreenOrientationChanged);
    }
}
