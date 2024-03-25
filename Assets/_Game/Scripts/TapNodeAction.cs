using UnityEngine;

public interface ITapNodeData
{
    
}

public class TapNodeData: ITapNodeData
{
}

public interface ITapNodeAction
{
    public void OnTapped(ITapNodeData data);
}

public abstract class TapNodeAction: ScriptableObject, ITapNodeAction
{
    public virtual void OnTapped(ITapNodeData data)
    {
    }
}