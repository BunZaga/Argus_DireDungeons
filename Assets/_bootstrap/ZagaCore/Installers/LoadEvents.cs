namespace ZagaCore.Events.Loading
{
    public class TitleScreenLoaded : GameEvent { }
    public class AllServicesLoaded : GameEvent { }

    public class ServiceInitializedProgress : GameEvent<ServiceInitializedProgress>
    {
        public float Progress = 0.0f;
    }

    public class AddToDebugOutput : GameEvent<AddToDebugOutput>
    {
        public string Message;
    }
}
