using System.Collections.Generic;
using Assets.Scripts.Menu;

namespace Assets.Scripts
{
    public interface IInputController
    {
        void Init(MenuInputController menuInputController);
//        string GetType();
        bool GetTapJump();
        bool ButtonActive(string name);
        float GetAxis(string name);
        bool AxisActive(string name);
        bool AxisPositive(string name);
        bool AxisNegative(string name);
        void VibrateController(float leftMotor, float rightMotor);
        void StopVibration();
//        List<byte> ControllerButtonPressState();
        List<byte> ControllerButtonHoldState();
        /* Button indices
         * 0: A
         * 1: B
         * 2: X
         * 3: Y
         * 4: L1
         * 5: R1
         * 6: L2
         * 7: R2
        */
        sbyte[] ControllerAnalogState();
        /* Analog indices
         * 0: Horizontal analog (-1, -.4, 0, .4, 1)
         * 1: Vertical analog (-1, -.4, 0, .4, 1)
         */
    }
}