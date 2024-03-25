using UnityEngine;
using ZagaCore.Events.Camera;
using ZagaCore.Events.Screen;

namespace ZagaCore
{
    public class CameraService
    {
        public Camera CameraUI { get; private set; }
        public Camera Camera3D { get; private set; }
        
        private EventService eventService => _eventService ?? (_eventService = Refs.Instance.Get<EventService>());
        private EventService _eventService;

        private ScreenManager screenManager => _screenManager ?? (_screenManager = Refs.Instance.Get<ScreenManager>());
        private ScreenManager _screenManager;
        
        public CameraService()
        {
            eventService.Subscribe<EventChangeUICamera>(SetUICamera);
            eventService.Subscribe<EventChange3DCamera>(Set3DCamera);
            eventService.Subscribe<ScreenOrientationChanged>(OnScreenOrientationChanged);
            Refs.Instance.Bind(this);
        }

        private void OnScreenOrientationChanged(ScreenOrientationChanged evnt)
        {
            if (CameraUI == null) return;
            CameraUI.orthographicSize = screenManager.IsHorizontal ? 2.7f : 5f;
        }
        
        private void SetUICamera(EventChangeUICamera camChangeEvnt)
        {
            var cam = camChangeEvnt.NewUICamera;
            if (CameraUI != cam)
            {
                CameraUI = cam;
                var evnt = eventService.GetPooledEvent<EventUICameraChanged>();
                evnt.NewUICamera = cam;
                eventService.Dispatch(evnt);
            }
            CameraUI.orthographicSize = screenManager.IsHorizontal ? 2.7f : 5f;
        }
        
        private void Set3DCamera(EventChange3DCamera camChangeEvnt)
        {
            var cam = camChangeEvnt.New3DCamera;
            if (Camera3D != cam)
            {
                Camera3D = cam;
                var evnt = eventService.GetPooledEvent<Event3DCameraChanged>();
                evnt.New3DCamera = cam;
                eventService.Dispatch(evnt);
            }
        }
    }
}