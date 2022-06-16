using System;
using UnityEngine;

namespace RPG.Core
{
    public class PersistentObjectSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject persistentObjectPrefab;

        private static bool hasSpawned = false;

        void Awake()
        {
            if (hasSpawned) return;
            
            SpawnPersistentObject();
            hasSpawned = true;
        }

        void SpawnPersistentObject()
        {
            GameObject persistentObject = Instantiate(persistentObjectPrefab);
            DontDestroyOnLoad(persistentObject);
        }
    }
}