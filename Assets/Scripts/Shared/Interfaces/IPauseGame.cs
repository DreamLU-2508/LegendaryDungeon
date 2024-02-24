using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    public interface IPauseGame
    {
        public bool IsPausedGame { get; }
        
        public void PauseGame();
        public void UnpauseGame();
        
        public event System.Action OnPauseGame;

        public event System.Action OnUnpauseGame;
    }
}
