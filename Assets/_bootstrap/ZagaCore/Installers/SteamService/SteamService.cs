#if UNITY_STEAM

using System.Collections;
using Steamworks;
using UnityEngine;
using ZagaCore;

namespace FunZaga.Plugins.Steam
{
    public class SteamService : MonoBehaviour
    {
        public HAuthTicket HTicket = HAuthTicket.Invalid;
        
        private Refs refs => _refs ?? (_refs = Refs.Instance);
        private Refs _refs;
        private EventService eventService => _eventService ?? (_eventService = Refs.Instance.Get<EventService>());
        private EventService _eventService;

        private void Awake()
        {
            StartCoroutine(WaitForSteam());
        }

        private IEnumerator WaitForSteam()
        {
            while(!SteamManager.Initialized)
            {
                yield return null;
            }
            Refs.Instance.Bind(this);
        }
    }
}
#endif