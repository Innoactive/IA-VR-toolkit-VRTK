using System;
using UnityEngine;
using UnityEngine.Events;

namespace VRTK
{
    [Serializable]
    public class MenuButton : IMenuButton
    {
        public Sprite buttonIcon;
        public UnityEvent onClick = new UnityEvent();
        public UnityEvent onHold = new UnityEvent();
        public UnityEvent onHoverEnter = new UnityEvent();
        public UnityEvent onHoverExit = new UnityEvent();

        public Sprite ButtonIcon
        {
            get { return buttonIcon; }
            set { buttonIcon = value; }
        }

        public UnityEvent OnClick
        {
            get { return onClick; }
            set { onClick = value; }
        }
        public UnityEvent OnHold
        {
            get { return onHold; }
            set { onHold = value; }
        }
        public UnityEvent OnHoverEnter
        {
            get { return onHoverEnter; }
            set { onHoverEnter = value; }
        }
        public UnityEvent OnHoverExit
        {
            get { return onHoverExit; }
            set { onHoverExit = value; }
        }
    }
}