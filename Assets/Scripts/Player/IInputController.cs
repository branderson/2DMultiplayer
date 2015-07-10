using Assets.Scripts.Menu;

namespace Assets.Scripts
{
    public interface IInputController
    {
        void Init(MenuInputController menuInputController);
        bool ButtonActive(string name);
        bool AxisActive(string name);
        bool AxisPositive(string name);
        bool AxisNegative(string name);
        void VibrateController(float leftMotor, float rightMotor);
        void StopVibration();
    }
}