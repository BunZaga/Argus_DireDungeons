using FunZaga.Events.Window;
using FunZaga.Events.Audio;
using FunZaga.WindowService;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZagaCore;
using ZagaCore.Events.Camera;
using ZagaCore.Events.Loading;

namespace FunZaga
{
    public class LoadLevel : GameEvent<LoadLevel>
    {
        public string Level;

        public override void Recycle()
        {
            Level = string.Empty;
        }
    }

    public class UnloadLevel : GameEvent<UnloadLevel>
    {
        public string Level;

        public override void Recycle()
        {
            Level = string.Empty;
        }
    }

    public class LevelLoaded : GameEvent<LevelLoaded>
    {
        public string Level;

        public override void Recycle()
        {
            Level = string.Empty;
        }
    }

    public class LevelUnoaded : GameEvent<LevelUnoaded>
    {
        public string Level;

        public override void Recycle()
        {
            Level = string.Empty;
        }
    }

    public class LevelLoadProgress : GameEvent<LevelLoadProgress>
    {
        public float Progress;

        public override void Recycle()
        {
            Progress = 0.0f;
        }
    }

    public class AppRoot : MonoBehaviour
    { private Refs refs => _refs ?? (_refs = Refs.Instance);
        private Refs _refs;

        private EventService eventService => _eventService ?? (_eventService = Refs.Instance.Get<EventService>());
        private EventService _eventService;

        [SerializeField] private Camera cameraUI;
        [SerializeField] private AudioListener audioListener;
        [SerializeField] private Transform mainViewStack;

        public GameObject ServiceRoot => serviceRoot;
        [SerializeField] private GameObject serviceRoot;
        
        [SerializeField] private string landingSceneName;
        
        private void Awake()
        {
            if (refs.Contains<AppRoot>())
            {
                Destroy(gameObject);
                return;
            }
            
            Time.fixedDeltaTime = 1.0f / 30.0f;
            DontDestroyOnLoad(gameObject);
            eventService.Subscribe<AllServicesLoaded>(OnApplicationLoaded);
        }

        private void Start()
        {
            refs.Bind(this);
        }

        private async void OnApplicationLoaded()
        {
            Debug.Log("App Loaded");
            var camEvent = eventService.GetPooledEvent<EventChangeUICamera>();
            camEvent.NewUICamera = cameraUI;
            eventService.Dispatch(camEvent);

            eventService.Dispatch(new EventChangeAudioListener
            {
                AudioListener = audioListener
            });

            refs.Get<WindowManager>().SetMainViewStack(mainViewStack);
            
            // LoadLevel game event...
            SceneManager.LoadSceneAsync(landingSceneName, LoadSceneMode.Additive).completed += handle =>
            {
                
            };
        }
    }
}