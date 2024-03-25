using System.Collections.Generic;
using UnityEngine;

namespace ZagaCore
{
    public enum PlatformTypes
    {
        Unknown,
        IOS,
        Android,
        Switch,
        Steam
    }

    public class PlatformUtils
    {
        public PlatformTypes Platform =>
#if UNITY_IOS
            PlatformTypes.IOS;
#elif UNITY_ANDROID
            PlatformTypes.Android;
#elif UNITY_SWITCH
            PlatformTypes.Switch;
#elif UNITY_STEAM
            PlatformTypes.Steam;
#else
            PlatformTypes.Unknown;
#endif

        public bool IsMobile => (Platform == PlatformTypes.Android || Platform == PlatformTypes.IOS);

        public PlatformUtils()
        {
            Refs.Instance.Bind(this);
        }
    }
}