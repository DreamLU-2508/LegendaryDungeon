using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace DreamLU
{
    public class Door : MonoBehaviour
    {
        [SerializeField] private BoxCollider2D collider2D;

        private Animator animator;
        private BoxCollider2D colliderTrigger;

        private bool isOpened;
        private bool isPreviouslyOpened;
        private Room _room;

        private IEnemySpawnProvider _enemySpawnProvider;

        [ShowInInspector, ReadOnly] public bool CanOpen => isOpened;
        [ShowInInspector, ReadOnly] public bool IsPreviouslyOpened => isPreviouslyOpened;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            colliderTrigger = GetComponent<BoxCollider2D>();
            _enemySpawnProvider = CoreLifetimeScope.SharedContainer.Resolve<IEnemySpawnProvider>();

            _enemySpawnProvider.OnClear += UnlockDoor;
        }

        public void Setup(Room room)
        {
            _room = room;
            _room.OnEnterRoom += CloseDoor;
            collider2D.enabled = true;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.tag == "PlayerWeapon" || collision.tag == "Player")
            {
                OpenDoor();
            }
        }

        private void OpenDoor()
        {
            if (_room.isClear || (!_room.isClear && !_room.InstancedRoom.IsPreviouslyVisited))
            {
                isOpened = true;
                animator.SetBool("open", true);
                collider2D.enabled = false;
                isPreviouslyOpened = true;
            }
        }

        private void CloseDoor(Room room)
        {
            if (room != _room) return;

            if(isOpened && !_room.isClear)
            {
                animator.SetBool("open", false);
                collider2D.enabled = true;
                isOpened = false;
            }
        }

        private void UnlockDoor(Room room)
        {
            if (room != _room) return;

            if (_room.isClear && isPreviouslyOpened)
            {
                isOpened = true;
                animator.SetBool("open", true);
                collider2D.enabled = false;
            }
        }

        private void OnDestroy()
        {
            _room.OnEnterRoom -= CloseDoor;
            _enemySpawnProvider.OnClear -= UnlockDoor;
        }
    }
}
