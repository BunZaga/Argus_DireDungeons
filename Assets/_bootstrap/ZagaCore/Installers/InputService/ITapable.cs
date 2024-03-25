using UnityEngine;
using ZagaCore.Events.Input;

public interface ITouchDown
{
    void OnTouchDown(TouchEvent touchEvent);
}

public interface ITouchUp
{
    void OnTouchUp(TouchEvent touchEvent);
}

public interface ITapped
{
    void OnTapped(TouchEvent touchEvent);
}

public interface ISwipe
{
    void OnSwipe(int id);
}

public interface IDrag
{
    void OnDrag(TouchEvent touchEvent);
}