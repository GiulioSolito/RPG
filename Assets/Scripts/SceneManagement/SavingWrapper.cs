using System;
using System.Collections;
using GameDevTV.Saving;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        [SerializeField] private KeyCode saveKey = KeyCode.S;
        [SerializeField] private KeyCode loadKey = KeyCode.L;
        [SerializeField] private KeyCode deleteKey = KeyCode.Delete;
        [SerializeField] private float fadeInTime = 0.2f;
        
        private const string DefaultSaveFile = "Save";

        private SavingSystem savingSystem;
        private Fader fader;

        void Awake()
        {
            savingSystem = GetComponent<SavingSystem>();
            fader = FindObjectOfType<Fader>();

            StartCoroutine(LoadLastScene());
        }

        IEnumerator LoadLastScene()
        {
            yield return savingSystem.LoadLastScene(DefaultSaveFile);
            fader.FadeOutImmediate();
            yield return fader.FadeIn(fadeInTime);
        }

        void Update()
        {
            if (Input.GetKeyDown(saveKey))
            {
                Save();
            }
            
            if (Input.GetKeyDown(loadKey))
            {
                Load();
            }

            if (Input.GetKeyDown(deleteKey))
            {
                Delete();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        public void Save()
        {
            savingSystem.Save(DefaultSaveFile);
        }

        public void Load()
        {
            savingSystem.Load(DefaultSaveFile);
        }

        void Delete()
        {
            savingSystem.Delete(DefaultSaveFile);
        }
    }
}