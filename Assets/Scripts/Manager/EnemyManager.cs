using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    public class EnemyManager : MonoBehaviour
    {
        [SerializeField] private EnemeyDataManifest manifest;
        [SerializeField] private bool isDemo;
        
        [SerializeField, ShowIf("isDemo")] private Vector2 minMaxX;
        [SerializeField, ShowIf("isDemo")] private Vector2 minMaxY;

        [ShowInInspector, ReadOnly]
        List<Enemy> enemies = new List<Enemy>();

        private void Update()
        {
            if(isDemo)
            {
                if(Input.GetKeyDown(KeyCode.S) && Input.GetKey(KeyCode.LeftControl))
                {
                    Vector3 randomPosition = new Vector3(Random.Range(minMaxX.x, minMaxX.y), Random.Range(minMaxY.x, minMaxY.y), 0);
                    int id = Random.Range(0, manifest.list.Count);
                    GameObject newObj = Instantiate(manifest.list[id].prefab);
                    Enemy enemy = newObj.GetComponent<Enemy>();
                    enemy.transform.position = randomPosition;
                    enemy.EnemySetup(manifest.list[id]);
                    enemies.Add(enemy);
                }

                if(enemies.Count > 0)
                {
                    List<Enemy> enemiesDie = new List<Enemy>();
                    foreach (Enemy enemy in enemies)
                    {
                        if (enemy.IsDie)
                        {
                            enemiesDie.Add(enemy);
                            Destroy(enemy.gameObject);
                        }
                    }

                    enemies.RemoveAll(x  => x.IsDie);
                }
            }
        }
    }

}