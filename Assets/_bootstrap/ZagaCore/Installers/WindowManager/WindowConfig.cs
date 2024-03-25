using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FunZaga.WindowService
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = nameof(WindowConfig), menuName = "Services/WindowManager/" + nameof(WindowConfig))]
    public class WindowConfig : ScriptableObject
    {
        public CanvasController Controller;
        public int DrawOrder;
        public bool PreCache;
        public bool DestroyOnClose;
    }
}