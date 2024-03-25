namespace ZagaCore
{
    public interface IGameEvent
    {
    }

    /// <summary>
    /// GameEvent with no data
    /// </summary>
    public class GameEvent : IGameEvent
    {
    }

    /// <summary>
    /// GameEvent which has data
    /// </summary>
    public class GameEvent<T> : IGameEvent where T : class
    {
        public virtual void Recycle()
        {

        }
    }
}