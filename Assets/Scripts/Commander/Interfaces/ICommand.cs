using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Commander
{
    public interface ICommand
    {
        void Execute();
        void Undo();
    }
}