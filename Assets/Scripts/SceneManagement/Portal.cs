using System.Collections;
using RPG.Control;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        private enum DestinationIdentifier { A, B, C, D, E }
        
        [SerializeField] private int sceneToLoad = -1;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private DestinationIdentifier destination;
        [SerializeField] private float fadeOutTime = 1f;
        [SerializeField] private float fadeInTime = 2f;
        [SerializeField] private float fadeWaitTime = 0.5f;

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                StartCoroutine(Transition());
            }
        }

        IEnumerator Transition()
        {
            if (sceneToLoad < 0)
            {
                Debug.LogError("Scene to load not set.");
                yield break;
            }
            
            DontDestroyOnLoad(gameObject);

            Fader fader = FindObjectOfType<Fader>();
            SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();
            PlayerController controller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            controller.enabled = false;
            
            yield return fader.FadeOut(fadeOutTime);

            wrapper.Save();
            
            yield return SceneManager.LoadSceneAsync(sceneToLoad);
            PlayerController newController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            newController.enabled = false;
            
            wrapper.Load();
            
            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);
            
            wrapper.Save();
            
            yield return new WaitForSeconds(fadeWaitTime);
            fader.FadeIn(fadeInTime);

            newController.enabled = true;
            
            Destroy(gameObject);
        }
        
        Portal GetOtherPortal()
        {
            foreach (Portal portal in FindObjectsOfType<Portal>())
            {
                if (portal == this)  continue;
                if (portal.destination != destination) continue;

                return portal;
            }
            return null;
        }
        
        void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPoint.position);
            player.transform.rotation = otherPortal.spawnPoint.rotation;
        }
    }
}