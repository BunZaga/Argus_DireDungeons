using FunZaga.Events.Window;
using System.Collections.Generic;
using UnityEngine;
using ZagaCore;

namespace FunZaga.WindowService
{
    /* Center position, otherwise scroll
        var diff = scrollRect.viewport.rect.width - scrollRect.content.rect.width;
        if (diff <= 0)
        {
            scrollRect.content.pivot.x = 0; //default scroll position
        }
        else
        {
            scrollRect.content.pivot.x = 0.5f; //horizontally align middle
        }

        var position = badgeContainer.anchoredPosition;
        position.x = 0;
        if (badgeContainer.rect.width > scrollRect.viewport.rect.width)
        {
            position.x = (badgeContainer.rect.width - scrollRect.viewport.rect.width) * 0.5f;
        }
        badgeContainer.anchoredPosition = position;
     * */
    public class WindowManager
    {
        public Transform MainViewStack { get; private set; }

        private List<WindowConfig> viewStack = new List<WindowConfig>();

        private EventService eventService => _eventService ?? (_eventService = Refs.Instance.Get<EventService>());
        private EventService _eventService;

        private Dictionary<string, WindowConfig> windowConfigMap = new Dictionary<string, WindowConfig>();

        private Dictionary<WindowConfig, CanvasController> instances = new Dictionary<WindowConfig, CanvasController>();
        
        public WindowManager(List<WindowConfig> windowConfigs)
        {
            instances.Clear();
            windowConfigMap.Clear();
            for(int i = 0, ilen = windowConfigs.Count; i < ilen; i++)
            {
                windowConfigMap.Add(windowConfigs[i].name, windowConfigs[i]);
            }

            eventService.Subscribe<WindowShow>(OnWindowShow);
            eventService.Subscribe<WindowHide>(OnWindowHide);
            
            Refs.Instance.Bind(this);
        }

        private void OnWindowShow(WindowShow evnt)
        {
            var name = evnt.Name;
            if(windowConfigMap.TryGetValue(name, out var windowConfig))
            {
                if (viewStack.Contains(windowConfig))
                {
                    
                    var controller = instances[windowConfig].GetComponent<CanvasController>();
                    if (controller.IsActive || controller.canvasTransitionState == CanvasAnimatorState.Opening)
                    {
                        return;
                    }
                    controller.ShowView(evnt.Callback);
                }
                else
                {
                    var windowGO = GameObject.Instantiate(windowConfig.Controller, MainViewStack, false);
                    var controller = windowGO.GetComponent<CanvasController>();
                    instances.Add(windowConfig, windowGO);
                    viewStack.Insert(0, windowConfig);
                    controller.Create();
                    controller.ShowView(evnt.Callback);
                }
            }
        }

        private void OnWindowHide(WindowHide evnt)
        {
            var name = evnt.Name;
            if (windowConfigMap.TryGetValue(name, out var windowConfig))
            {
                if (viewStack.Contains(windowConfig))
                {
                    var controller = instances[windowConfig].GetComponent<CanvasController>();
                    if (!controller.IsActive || controller.canvasTransitionState == CanvasAnimatorState.Closing)
                    {
                        return;
                    }
                    if (windowConfig.DestroyOnClose)
                    {
                        var callback = evnt.Callback;
                        controller.HideView(()=> {
                            callback?.Invoke();
                            GameObject.Destroy(controller.gameObject);
                            viewStack.Remove(windowConfig);
                            instances.Remove(windowConfig);
                        });
                        return;
                    }
                    controller.HideView(evnt.Callback);
                }
            }
        }

        public void SetMainViewStack(Transform transform)
        {
            if (MainViewStack != transform)
            {
                MainViewStack = transform;
                var evnt = new ViewStackChanged
                {
                    MainViewStack = transform
                };

                eventService.Dispatch(evnt);
            }
        }
    }
}