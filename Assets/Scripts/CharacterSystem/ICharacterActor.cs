using UnityEngine;

namespace DreamLU
{
    public interface ICharacterActor
    {
        public Transform CharacterTransform { get; }

        public Animator CharacterAnimator { get; }

        public bool IsMovementLocked { get; set; }

        public void AddDamage(int damage);

        public void AddCard(CardData cardData);
        
        public bool IsActionLocked { get; set; }
    }
}