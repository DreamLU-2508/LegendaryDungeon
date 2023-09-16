using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace DreamLU
{
    public class CharacterController : MonoBehaviour
    {
        [SerializeField] private Transform weaponShootPosition;
        [SerializeField] private CharacterAimController aimController;

        private Character character;
        private Camera _camera;

        private void Awake()
        {
            character = GetComponent<Character>();
            _camera = Camera.main;
        }

        private void Update()
        {
            HandleMovement();

            HandleAim();
        }

        private void HandleMovement()
        {
            character.SetIdle();
        }

        private void HandleAim()
        {
            Vector3 mouseWorldPos = CharacterUtilities.GetMouseWorldPosition(_camera);
            Vector3 weaponDirection = mouseWorldPos - weaponShootPosition.position;
            Vector3 playerDirection = mouseWorldPos - this.transform.position;

            float weaponAngle = CharacterUtilities.GetAngleFromVector(weaponDirection);
            float playerAngle = CharacterUtilities.GetAngleFromVector(playerDirection);

            AimDirection playerAimDir = CharacterUtilities.GetAimDirection(playerAngle);

            character.SetAimAnimation(playerAimDir);
            aimController.Aim(playerAimDir, weaponAngle);
        }
    }

}