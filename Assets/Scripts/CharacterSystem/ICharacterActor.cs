using UnityEngine;

namespace DreamLU
{
    public interface ICharacterActor
    {
        public Transform CharacterTransform { get; }

        public Animator CharacterAnimator { get; }

        public bool IsMovementLocked { get; set; }
    }
}