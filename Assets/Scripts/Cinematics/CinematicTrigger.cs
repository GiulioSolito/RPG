using System;
using RPG.Saving;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour, ISaveable
    {
        private bool cinematicPlayed = false;
        
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && cinematicPlayed == false)
            {
                GetComponent<PlayableDirector>().Play();
                cinematicPlayed = true;
            }
        }

        public object CaptureState()
        {
            return cinematicPlayed;
        }

        public void RestoreState(object state)
        {
            cinematicPlayed = (bool) state;
        }
    }
}