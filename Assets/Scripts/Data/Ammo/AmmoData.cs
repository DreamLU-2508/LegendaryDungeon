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
        public Vector2 minMaxSpeed;
        public float range = 20f;
        public Vector2 minMaxSpread;
        public int manaConsumed;

        [Header("prefab")]
        public GameObject prefab;
        public GameObject vfxPrefabSmoke;
        public GameObject vfxPrefabHit;
    }

}