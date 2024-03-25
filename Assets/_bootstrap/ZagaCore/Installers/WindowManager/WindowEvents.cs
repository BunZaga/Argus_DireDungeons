using System;
using UnityEngine;
using ZagaCore;

namespace FunZaga.Events.Window
{
    public class ViewStackChanged : GameEvent<ViewStackChanged>
    {
        public Transform MainViewStack;

        public override void Recycle()
        {
            MainViewStack = null;
        }
    }

    public class WindowShow : GameEvent<WindowShow>
    {
        public string Name;
        public Action Callback;

        public override void Recycle()
        {
            Name = string.Empty;
            Callback = null;
        }
    }

    public class WindowHide : GameEvent<WindowHide>
    {
        public string Name;
        public Action Callback;

        public override void Recycle()
        {
            Name = string.Empty;
            Callback = null;
        }
    }
}