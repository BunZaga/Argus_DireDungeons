namespace ZagaCore.Events.Camera
{
    public class EventStartUIInput : GameEvent
    {
        
    }
    public class EventStopUIInput : GameEvent
    {
        
    }
    public class EventChangeUICamera : GameEvent<EventChangeUICamera>
    {
        public UnityEngine.Camera NewUICamera;

        public override void Recycle()
        {
            NewUICamera = null;
        }
    }
    
    public class EventUICameraChanged : GameEvent<EventUICameraChanged>
    {
        public UnityEngine.Camera NewUICamera;

        public override void Recycle()
        {
            NewUICamera = null;
        }
    }

    public class EventChange3DCamera : GameEvent<EventChange3DCamera>
    {
        public UnityEngine.Camera New3DCamera;

        public override void Recycle()
        {
            New3DCamera = null;
        }
    }
    
    public class Event3DCameraChanged : GameEvent<Event3DCameraChanged>
    {
        public UnityEngine.Camera New3DCamera;

        public override void Recycle()
        {
            New3DCamera = null;
        }
    }
}