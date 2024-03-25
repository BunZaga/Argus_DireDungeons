using UnityEngine;

namespace ZagaCore.Events.Input
{
    public class TouchEvent : GameEvent<TouchEvent>
    {
        public TouchPhase TouchPhase;
        public Vector2 TouchPoint;
        public int TouchId;
        
        public override void Recycle()
        {
            TouchPhase = TouchPhase.Canceled;
            TouchPoint = Vector2.zero;
            TouchId = -1;
        }
    }
}