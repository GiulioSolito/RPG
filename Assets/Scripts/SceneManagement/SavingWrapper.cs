using System;
using System.Collections;
using RPG.Saving;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        [SerializeField] private float fadeInTime = 0.2f;
        
        private const string DefaultSaveFile = "Save";

        private SavingSystem savingSystem;
        private Fader fader;

        void Awake()
        {
            savingSystem = GetComponent<SavingSystem>();
            fader = FindObjectOfType<Fader>();

            // StartCoroutine(LoadLastScene());
        }

        IEnumerator LoadLastScene()
        {
            yield return savingSystem.LoadLastScene(DefaultSaveFile);
            fader.FadeOutImmediate();
            yield return fader.FadeIn(fadeInTime);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }
            
            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }

            if (Input.GetKeyDown(KeyCode.Delete))
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