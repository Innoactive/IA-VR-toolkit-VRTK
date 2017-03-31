namespace VRTK
{
    public interface IRadialMenu
    {
        event HapticPulseEventHandler FireHapticPulse;

        void ShowMenu();

        void HideMenu(bool force);

        void ClickButton(float angle);

        void UnClickButton(float angle);

        void HoverButton(float angle);

        void StopTouching();

        bool IsShown { get; set; }

        bool HideOnRelease { get; set; }

        bool ExecuteOnUnclick { get; set; }
    }
}
