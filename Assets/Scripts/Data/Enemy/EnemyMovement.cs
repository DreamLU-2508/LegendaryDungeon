using System.Collections;
using System.Collections.Generic;
using DreamLU.AStar;
using UnityEngine;
using VContainer;

namespace DreamLU
{
    public class EnemyMovement : MonoBehaviour
    {
        private Enemy enemy;

        private IHeroPositionProvider _positionProvider;
        private IGameStateProvider _gameStateProvider;
        private IEnemyProvider _enemyProvider;
        private IEnemySpawnProvider _enemySpawnProvider;
        private ICharacterActor _characterActor;
        
        private Vector3 playerPosition;
        private bool chasePlayer;
        private float currentEnemyPathRebuildCooldown;
        private Stack<Vector3> movementSteps = new Stack<Vector3>();
        private Coroutine moveEnemyRoutine;
        private Rigidbody2D _rigidbody2D;
        private List<Vector2Int> surroundingPositionList = new List<Vector2Int>();
        
        private void Awake()
        {
            enemy = GetComponent<Enemy>();
            _positionProvider = CoreLifetimeScope.SharedContainer.Resolve<IHeroPositionProvider>();
            _gameStateProvider = CoreLifetimeScope.SharedContainer.Resolve<IGameStateProvider>();
            _enemySpawnProvider = CoreLifetimeScope.SharedContainer.Resolve<IEnemySpawnProvider>();
            _characterActor = CoreLifetimeScope.SharedContainer.Resolve<ICharacterActor>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            
            playerPosition = _positionProvider.GetTargetPosition();
        }

        private void Update()
        {
            if (enemy.IsDie || _characterActor.IsHeroDead) return;

            MoveEnemy();
        }

        private void MoveEnemy()
        {
            // Movement cooldown timer
            currentEnemyPathRebuildCooldown -= Time.deltaTime;

            // Check distance to player to see if enemy should start chasing
            if (!chasePlayer && Vector3.Distance(transform.position, playerPosition) < enemy.Data.stat.chaseDistance)
            {
                chasePlayer = true;
            }

            // If not close enough to chase player then return
            if (!chasePlayer)
                return;

            // if the movement cooldown timer reached or player has moved more than required distance
            // then rebuild the enemy path and move the enemy
            if (currentEnemyPathRebuildCooldown <= 0f || (Vector3.Distance(playerPosition, _positionProvider.GetTargetPosition()) > Settings.playerMoveDistanceToRebuildPath))
            {
                // Reset path rebuild cooldown timer
                currentEnemyPathRebuildCooldown = Settings.enemyPathRebuildCooldown;

                // Reset player reference position
                playerPosition = _positionProvider.GetTargetPosition();

                // Move the enemy using AStar pathfinding - Trigger rebuild of path to player
                CreatePath();

                // If a path has been found move the enemy
                if (movementSteps != null)
                {
                    if (moveEnemyRoutine != null)   
                    {
                        // Trigger idle event
                        enemy.SetIdle();
                        StopCoroutine(moveEnemyRoutine);
                    }

                    // Move enemy along the path using a coroutine
                    moveEnemyRoutine = StartCoroutine(MoveEnemyRoutine(movementSteps));
                }
            }
        }
        
        private IEnumerator MoveEnemyRoutine(Stack<Vector3> movementSteps)
        {
            while (movementSteps.Count > 0)
            {
                Vector3 nextPosition = movementSteps.Pop();

                // while not very close continue to move - when close move onto the next step
                while (Vector3.Distance(nextPosition, transform.position) > 0.2f)
                {
                    // Trigger movement event
                    enemy.SetAnimationMovement(transform.position, nextPosition);
                    MoveRigidBody(nextPosition, transform.position, enemy.MoveSpeed);

                    yield return new WaitForFixedUpdate();  // moving the enmy using 2D physics so wait until the next fixed update

                }

                yield return new WaitForFixedUpdate();
            }

            // End of path steps - trigger the enemy idle event
            enemy.SetIdle();

        }
        
        private void CreatePath()
        {
            Room currentRoom = _enemySpawnProvider.CurrentRoom;
            
            Grid grid = currentRoom.Grid;

            // Get players position on the grid
            Vector3Int playerGridPosition = GetNearestNonObstaclePlayerPosition(currentRoom);


            // Get enemy position on the grid
            Vector3Int enemyGridPosition = grid.WorldToCell(transform.position);

            // Build a path for the enemy to move on
            movementSteps = AStarUtilities.BuildPath(currentRoom, enemyGridPosition, playerGridPosition);

            // Take off first step on path - this is the grid square the enemy is already on
            if (movementSteps != null)
            {
                movementSteps.Pop();
            }
            else
            {
                // Trigger idle event - no path
                enemy.SetIdle();
            }
        }
            
        /// <summary>
        /// Move the rigidbody component
        /// </summary>
        private void MoveRigidBody(Vector3 movePosition, Vector3 currentPosition, float moveSpeed)
        {
            Vector2 unitVector = Vector3.Normalize(movePosition - currentPosition);

            _rigidbody2D.MovePosition(_rigidbody2D.position + (unitVector * moveSpeed * Time.fixedDeltaTime));
        }
        
        private Vector3Int GetNearestNonObstaclePlayerPosition(Room currentRoom)
        {
            Vector3 playerPosition = _positionProvider.GetTargetPosition();

            Vector3Int playerCellPosition = currentRoom.Grid.WorldToCell(playerPosition);

            Vector2Int adjustedPlayerCellPositon = new Vector2Int(playerCellPosition.x - currentRoom.InstancedRoom.TemplateLowerBounds.x, playerCellPosition.y - currentRoom.InstancedRoom.TemplateLowerBounds.y);
            
            int obstacle = Mathf.Min(currentRoom.AStarMovementPenalty[adjustedPlayerCellPositon.x, adjustedPlayerCellPositon.y], currentRoom.AStarItemObstacles[adjustedPlayerCellPositon.x, adjustedPlayerCellPositon.y]);

            // if the player isn't on a cell square marked as an obstacle then return that position
            if (obstacle != 0)
            {
                return playerCellPosition;
            }
            // find a surounding cell that isn't an obstacle - required because with the 'half collision' tiles
            // and tables the player can be on a grid square that is marked as an obstacle
            else
            {
                // Empty surrounding position list
                surroundingPositionList.Clear();

                // Populate surrounding position list - this will hold the 8 possible vector locations surrounding a (0,0) grid square
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (j == 0 && i == 0) continue;

                        surroundingPositionList.Add(new Vector2Int(i, j));
                    }
                }


                // Loop through all positions
                for (int l = 0; l < 8; l++)
                {
                    // Generate a random index for the list
                    int index = Random.Range(0, surroundingPositionList.Count);

                    // See if there is an obstacle in the selected surrounding position
                    try
                    {
                        obstacle = Mathf.Min(currentRoom.AStarMovementPenalty[adjustedPlayerCellPositon.x + surroundingPositionList[index].x, adjustedPlayerCellPositon.y + surroundingPositionList[index].y], currentRoom.AStarItemObstacles[adjustedPlayerCellPositon.x + surroundingPositionList[index].x, adjustedPlayerCellPositon.y + surroundingPositionList[index].y]);

                        // If no obstacle return the cell position to navigate to
                        if (obstacle != 0)
                        {
                            return new Vector3Int(playerCellPosition.x + surroundingPositionList[index].x, playerCellPosition.y + surroundingPositionList[index].y, 0);
                        }

                    }
                    // Catch errors where the surrounding positon is outside the grid
                    catch
                    {

                    }

                    // Remove the surrounding position with the obstacle so we can try again
                    surroundingPositionList.RemoveAt(index);
                }

                // If no non-obstacle cells found surrounding the player - send the enemy in the direction of an enemy spawn position
                return (Vector3Int)currentRoom.InstancedRoom.SpawnPositionArray[Random.Range(0, currentRoom.InstancedRoom.SpawnPositionArray.Length)];
            }
        }
    }

}