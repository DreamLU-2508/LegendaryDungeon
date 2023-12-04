using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    public interface IGameStateProvider
    {
        public GameConfig GameConfig { get; }

        public void ProceedToEndLevel();
    }
}
