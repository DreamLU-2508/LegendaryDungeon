using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    public interface IGameStateProvider
    {
        public GameConfig GameConfig { get; }

        public void ProceedToEndLevel();

        public void GotoMainMenu();

        public void OnEnterRoom(Room roomData);

        public Vector3 GetMouseWorldPosition();
    }
}
