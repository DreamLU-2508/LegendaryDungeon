using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    [CreateAssetMenu(fileName = "AMMO_", menuName = "Database/Ammo/Ammo Data")]
    public class AmmoData : ScriptableObject
    {
        public AmmoID ammoID;
        public string ammoName;

        public bool isPlayerAmmo;

        public Sprite ammoSprite;


        [Header("Infor Ammo")]
        public int damage = 1;
        public Vector2 minMaxSpeed;
        public float range = 20f;
        public float rotationSpeed = 1f;
        public Vector2 minMaxSpread;
        public Vector2 minMaxSpawnAmount;
        public int manaConsumed;

        [Header("prefab")]
        public GameObject prefab;
    }

}