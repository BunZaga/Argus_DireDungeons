using System;
using UnityEngine;
using ZagaCore.Events.Update;

namespace ZagaCore
{
    public class MainThreadDispatcher : MonoBehaviour
    {
        private EventService eventService;
        
        private void Awake()
        {
            Refs.Instance.Bind(this);
        }

        private void Start()
        {
            eventService = Refs.Instance.Get<EventService>();
        }

        private void Update()
        {
            eventService.Dispatch<EventGameUpdate>();
        }
        
        private void FixedUpdate()
        {
            eventService.Dispatch<EventGameFixedUpdate>();
        }

        private void LateUpdate()
        {
            eventService.Dispatch<EventGameLateUpdate>();
        }

        public void OnNextUpdate(Action action)
        {
            void Callback()
            {
                action.Invoke();
            }

            eventService.OnNextDispatch<EventGameUpdate>(Callback);
        }
    }
}