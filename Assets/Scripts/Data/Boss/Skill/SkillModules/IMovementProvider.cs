using UnityEngine;

namespace DreamLU
{ 
    public interface IMovementProvider
    {
        public void SetCharge(bool stopMove);

        public void SetAnimationMovement(Vector3 enemyPosition, Vector3 targetPosition);
    }
}
