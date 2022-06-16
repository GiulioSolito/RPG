using System;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour
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
    }
}