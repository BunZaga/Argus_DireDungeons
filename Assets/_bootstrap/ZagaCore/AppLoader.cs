using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using ZagaCore.Events.Loading;

namespace ZagaCore
{
    public class AppLoader : MonoBehaviour
    {
        [SerializeField]
        private GameObject splash;
        [SerializeField]
        private RootContext rootContext;
        [SerializeField] float splashDelay = 1.0f;
        [SerializeField] PlayableDirector director;

        private ushort progressMask = 0;
        private ushort goalMask = 3;

        private void Awake()
        {
            progressMask = 0;
            splash.SetActive(true);
            
            Refs.Instance.WaitFor<EventService>().Then((eventService) =>
            {
                eventService.OnNextDispatch<AllServicesLoaded>(() =>
                {
                    DestroyIfReady(2);
                    if (director != null)
                    {
                        director.Play();
                        DelayThenDestroy(1);
                    }
                });
            });

            // set splash screen active
            // First install core services like login, config, input, etc.
            // Next install non-core things like window manager, camera stuff,
            // sound, quest, game root, pop up stuff, achievements, etc
            // Finally install 'weak referenced' things
            Instantiate(rootContext, null, false);
        }

        private async void DelayThenDestroy(ushort flag)
        {
            await Task.Delay(TimeSpan.FromSeconds(splashDelay));
            DestroyIfReady(flag);
        }

        private void DestroyIfReady(ushort flag)
        {
            progressMask |= flag;
            if ((progressMask & goalMask) == goalMask)
            {
                SceneManager.UnloadSceneAsync("Bootstrap", UnloadSceneOptions.None);
                Destroy(gameObject);
            }
        }
    }
}