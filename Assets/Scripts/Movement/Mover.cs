using RPG.Core;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction
    {
        [SerializeField] private Transform target; 
    
        private Ray ray;

        private NavMeshAgent agent;
        private Animator anim;
        private ActionScheduler scheduler;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            anim = GetComponent<Animator>();
            scheduler = GetComponent<ActionScheduler>();
        }

        void Update()
        {
            UpdateAnimator();
        }

        public void StartMoveAction(Vector3 destination)
        {
            scheduler.StartAction(this);
            MoveTo(destination);
        }

        public void MoveTo(Vector3 destination)
        {
            agent.destination = destination;
            agent.isStopped = false;
        }
        
        void UpdateAnimator()
        {
            Vector3 velocity = agent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;

            anim.SetFloat("Speed", speed);
        }       
        
        public void Cancel()
        {
            agent.isStopped = true;
        } 
    }
}