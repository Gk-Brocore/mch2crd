using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Input
{
    public interface IInputProvider
    {
        string Name { get; }
        bool IsActive { get; }
        event Action<Vector2> OnClick;


        void Tick();
    }
}