using FunZaga.Events.Window;
using UnityEngine;
using ZagaCore;

public class GameUI : MonoBehaviour
{
    private void Start()
    {
        if (Refs.Instance.Get<PlatformUtils>().IsMobile)
        {
            Refs.Instance.Get<EventService>().Dispatch(new WindowShow
            {
                Name = "DungeonUI"
            });
        }
        
        Refs.Instance.Get<EventService>().Dispatch(new CapsuleControl.FakeDungeonLoaded{});
    }
}
