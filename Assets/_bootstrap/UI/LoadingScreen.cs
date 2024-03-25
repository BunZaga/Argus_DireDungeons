using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using ZagaCore.Events.Loading;

namespace ZagaCore
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private Canvas[] views;
        [SerializeField] private List<Image> progressImage;
        [SerializeField] private List<TMPro.TextMeshProUGUI> outputs;
        
        private EventService eventService => _eventService ??  (_eventService = Refs.Instance.Get<EventService>());
        private EventService _eventService;

        private ScreenManager screenManager => _screenManager?? (_screenManager = Refs.Instance.Get<ScreenManager>());
        private ScreenManager _screenManager;

        private StringBuilder outputString = new StringBuilder();

        private void Awake()
        {
            progressImage.ForEach(img=>img.fillAmount = 0.01f);
            screenManager.SetScreenOrientationEnabled(false);

            views[0].enabled = screenManager.IsVertical;
            views[1].enabled = screenManager.IsHorizontal;

            eventService.OnNextDispatch<AllServicesLoaded>(OnAllServicesLoaded);
            eventService.Subscribe<ServiceInitializedProgress>(OnServiceInitializedProgress);
            eventService.Subscribe<AddToDebugOutput>(OnAddToDebugOutput);
            eventService.Dispatch<TitleScreenLoaded>();
        }

        private void OnAddToDebugOutput(AddToDebugOutput evnt)
        {
            if (!string.IsNullOrEmpty(evnt.Message))
            {
                outputString.Append(evnt.Message);
                outputString.Append("\n");
                outputs.ForEach(output => output.text = outputString.ToString());
            }
        }

        private void OnServiceInitializedProgress(ServiceInitializedProgress eventServiceInitializedProgress)
        {
            
            var fillAmount = Mathf.Clamp(eventServiceInitializedProgress.Progress, 0.0f, 1.0f);
            progressImage.ForEach(img => img.fillAmount = fillAmount);
        }

        private void OnAllServicesLoaded()
        {
            eventService.UnSubscribe<ServiceInitializedProgress>(OnServiceInitializedProgress);
            eventService.UnSubscribe<AddToDebugOutput>(OnAddToDebugOutput);
            var screenManager = Refs.Instance.Get<ScreenManager>();
            screenManager.SetScreenOrientationEnabled(true);
            Destroy(gameObject);
        }
    }
}