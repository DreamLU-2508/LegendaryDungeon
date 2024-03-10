using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace DreamLU
{
    public class CharacterController : MonoBehaviour
    {
        [SerializeField] private Transform weaponShootPosition;
        [SerializeField] private Transform weaponSecondShootPosition;
        [SerializeField] private CharacterAimController aimController;

        private Character character;
        private Camera _camera;
        private IPauseGame _pauseGame;
        private ICharacterActor _characterActor;

        private void Awake()
        {
            character = GetComponent<Character>();
            _camera = Camera.main;
            _pauseGame = CoreLifetimeScope.SharedContainer.Resolve<IPauseGame>();
            _characterActor = CoreLifetimeScope.SharedContainer.Resolve<ICharacterActor>();
        }

        private void Update()
        {
            if(_pauseGame.IsPausedGame) return;
            
            if(_characterActor.IsActionLocked || _characterActor.IsMovementLocked) return;
            
            HandleMovement();

            HandleAim(weaponShootPosition);

            if(character.UseSkillDoubleGun)
            {
                HandleAim(weaponSecondShootPosition);
            }
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

        private void HandleAim(Transform weaponShootPosition)
        {
            Vector3 mouseWorldPos = CharacterUtilities.GetMouseWorldPosition(_camera);
            Vector3 weaponDirection = mouseWorldPos - weaponShootPosition.position;
            Vector3 playerDirection = mouseWorldPos - this.transform.position;

            float weaponAngle = CharacterUtilities.GetAngleFromVector(weaponDirection);
            float playerAngle = CharacterUtilities.GetAngleFromVector(playerDirection);

            AimDirection playerAimDir = CharacterUtilities.GetAimDirection(playerAngle);

            character.SetAimAnimation(playerAimDir);
            aimController.Aim1(playerAimDir, weaponAngle);
            if (character.UseSkillDoubleGun)
            {
                aimController.Aim2(playerAimDir, weaponAngle);
            }

            ShootWeapon(weaponDirection, weaponAngle, playerAngle, playerAimDir);
        }

        private void ShootWeapon(Vector3 weaponDirection, float weaponAngle, float playerAngle, AimDirection playerAimDir)
        {
            if (Input.GetMouseButton(0))
            {
                character.ActiveWeaponPlayer(playerAngle, weaponAngle, weaponDirection);
            }
        }
    }
}