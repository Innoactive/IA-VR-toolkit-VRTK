using System;
using UnityEngine;
using UnityEngine.Events;

namespace VRTK
{
    [Serializable]
    public class MenuButton
    {
        public Sprite ButtonIcon;
        public UnityEvent OnClick = new UnityEvent();
        public UnityEvent OnHold = new UnityEvent();
        public UnityEvent OnHoverEnter = new UnityEvent();
        public UnityEvent OnHoverExit = new UnityEvent();
    }
}