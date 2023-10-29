using UnityEngine;

namespace DreamLU
{
    public interface IAmmo
    {
        public void OnCreateAmmo(AmmoData data, float aimAngle, float weaponAimAngle, Vector3 weaponAimDir,bool overrideAmmoMovement = false);

        public void OnCreateAmmo(AmmoData data, Vector3 target);

        public GameObject GetGameObject();
    }
}
