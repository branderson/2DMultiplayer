using XInputDotNetPure;

namespace Assets.Scripts.Player
{
    public class PlayerConfig
    {
        public bool TapJump = true;
        public bool Vibration = true;
        public bool DPad = false;
        public int Slot = 0;
        public int ControllerIndex = -1;
        public PlayerIndex XIndex = PlayerIndex.One;
        public bool UseXInput = false;
        public bool Keyboard = false;
        public bool Computer = false;
        public bool Active = false;
    }
}