using UnityEngine;
using ZagaCore.Events.Camera;
using ZagaCore.Events.Screen;
using ZagaCore.Events.Update;

namespace ZagaCore
{
    public class ScreenManager
    {
        private float lastAspectRatio;

        private EventService eventService =>_eventService ?? (Refs.Instance.Get<EventService>());
        private EventService _eventService;
        private CameraService cameraService => _cameraService ?? (Refs.Instance.Get<CameraService>());
        private CameraService _cameraService;
        private PlatformUtils platformUtils => _platformUtils ?? (Refs.Instance.Get<PlatformUtils>());
        private PlatformUtils _platformUtils;

        private ScreenOrientation lastFrameScreenOrientation;
        private static ScreenOrientation currentScreenOrientation;

        public bool IsVertical => currentScreenOrientation == ScreenOrientation.Portrait;
        public bool IsHorizontal => currentScreenOrientation == ScreenOrientation.LandscapeLeft;

        public static int Width => Screen.width;
        public static int Height => Screen.height;
        public static Rect SafeArea => Screen.safeArea;
        public static ScreenOrientation CurrentScreenOrientation => currentScreenOrientation;

        public ScreenManager()
        {
            ForceScreenOrientation(GetScreenOrientation());

            if (cameraService.CameraUI == null)
            {
                eventService.OnNextDispatch<EventUICameraChanged>(OnMainCameraInit);
            }
            else
            {
                OnUpdate();
            }
            
            Refs.Instance.Bind(this);
        }

        public void SetScreenOrientationEnabled(bool enabled)
        {
            Screen.autorotateToLandscapeLeft = enabled;
            Screen.autorotateToLandscapeRight = enabled;
            Screen.autorotateToPortrait = enabled;
            Screen.autorotateToPortraitUpsideDown = enabled;
        }

        // First checks if aspect ratio (resolution) changed
        // Then checks of orientation changed
        private void OnUpdate()
        {
            if (Mathf.Abs(cameraService.CameraUI.aspect - lastAspectRatio) > Mathf.Epsilon)
            {
                RaiseEventScreenResolutionChanged(Width, Height);
                lastAspectRatio = cameraService.CameraUI.aspect;
            }
            
            var newScreenOrientation = SimplifyScreenOrientation(GetScreenOrientation());
            if (CheckIfScreenOrientationChanged(newScreenOrientation))
            {
                RaiseEventScreenOrientationChanged(newScreenOrientation);
                lastFrameScreenOrientation = newScreenOrientation;
            }
        }

        public void ForceScreenOrientation(ScreenOrientation newScreenOrientation)
        {
            var simplifiedOrientation = SimplifyScreenOrientation(newScreenOrientation);

            if (simplifiedOrientation != CurrentScreenOrientation)
            {
                currentScreenOrientation = simplifiedOrientation;
                RaiseEventScreenOrientationChanged(simplifiedOrientation);
            }
        }

        public ScreenOrientation GetScreenOrientation()
        {
            return Width > Height ? ScreenOrientation.LandscapeLeft : ScreenOrientation.Portrait;
        }

        private void OnMainCameraInit(EventUICameraChanged eventEventUICameraChanged)
        {
            eventService.UnSubscribe<EventGameUpdate>(OnUpdate);
            eventService.Subscribe<EventGameUpdate>(OnUpdate);
        }

        public void RaiseEventScreenOrientationChanged(ScreenOrientation newScreenOrientation)
        {
            var evnt = eventService.GetPooledEvent<ScreenOrientationChanged>();
            evnt.NewScreenOrientation = newScreenOrientation;
            eventService.Dispatch(evnt);
            Debug.Log("OrientationChanged:" + newScreenOrientation);
        }

        public void RaiseEventScreenResolutionChanged(int width, int height)
        {
            var evnt = eventService.GetPooledEvent<ScreenResolutionChanged>();
            evnt.NewScreenWidth = width;
            evnt.NewScreenHeight = height;
            eventService.Dispatch(evnt);
        }
        
        private bool CheckIfScreenOrientationChanged(ScreenOrientation screenOrientation)
        {
            if (lastFrameScreenOrientation != screenOrientation && screenOrientation != CurrentScreenOrientation)
            {
                currentScreenOrientation = screenOrientation;
                return true;
            }
            return false;
        }

        private ScreenOrientation SimplifyScreenOrientation(ScreenOrientation screenOrientation)
        {
            switch (screenOrientation)
            {
                case ScreenOrientation.LandscapeLeft:
                case ScreenOrientation.LandscapeRight:
                    return ScreenOrientation.LandscapeLeft;

                case ScreenOrientation.Portrait:
                case ScreenOrientation.PortraitUpsideDown:
                    return ScreenOrientation.Portrait;

                default:
                    return Width > Height ? ScreenOrientation.LandscapeLeft : ScreenOrientation.Portrait;
            }
        }
    }
}