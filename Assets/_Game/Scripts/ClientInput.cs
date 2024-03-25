using Animancer;

[System.Serializable]
public class ClientInput
{
    public float InputX;
    public float InputZ;

    public int ControlState = ActionState.Idle;
}

public static class ActionState
{
    public static readonly int None = 0;
    public static readonly int Idle = 1;
    public static readonly int Moving = 2;
    public static readonly int Action = 4;
    public static readonly int ActionLocked = 8;
    public static readonly int MovingLocked = 16;
}