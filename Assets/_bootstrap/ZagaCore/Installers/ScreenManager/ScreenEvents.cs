
namespace ZagaCore.Events.Screen
{
    public class ScreenOrientationChanged : GameEvent<ScreenOrientationChanged>
    {
        public UnityEngine.ScreenOrientation NewScreenOrientation;
        public override void Recycle()
        {
            NewScreenOrientation = UnityEngine.ScreenOrientation.AutoRotation;
        }
    }

    public class ScreenResolutionChanged : GameEvent<ScreenResolutionChanged>
    {
        public int NewScreenHeight;
        public int NewScreenWidth;

        public override void Recycle()
        {
            NewScreenHeight = 0;
            NewScreenWidth = 0;
        }
    }
}