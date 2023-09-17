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
            float hor = Input.GetAxisRaw("Horizontal");
            float ver = Input.GetAxisRaw("Vertical");

            Vector2 dir = new Vector2(hor, ver);

            if(hor != 0 && ver != 0)
            {
                dir *= 0.7f;
            }

            if(dir != Vector2.zero)
            {
                character.SetMovement(dir);
                return;
            }

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