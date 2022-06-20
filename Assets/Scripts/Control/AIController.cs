using RPG.Attributes;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] private float chaseDistance = 5f;
        [SerializeField] private float suspicionTime = 3f;
        [SerializeField] private PatrolPath patrolPath;
        [SerializeField] private float waypointTolerance = 1f;
        [SerializeField] private float waypointWaitTime = 3f;
        [Range(0,1)]
        [SerializeField] private float patrolSpeedFraction = 0.2f;

        private GameObject player;

        private Vector3 guardPosition;
        private Quaternion guardRotation;
        private float timeSinceLastSawPlayer = Mathf.Infinity;
        private float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        private int currentWaypointIndex = 0;
        
        private Mover mover;
        private Fighter fighter;
        private Health health;
        private ActionScheduler scheduler;

        void Awake()
        {
            mover = GetComponent<Mover>();
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            scheduler = GetComponent<ActionScheduler>();
            player = GameObject.FindGameObjectWithTag("Player");
        }

        void Start()
        {
            guardPosition = transform.position;
            guardRotation = transform.rotation;
        }

        void Update()
        {
            if (health.IsDead) return;
            if (InAttackRange() && fighter.CanAttack(player))
            {
                AttackBehaviour();
            }
            else if (timeSinceLastSawPlayer < suspicionTime)
            {
                SuspicionBehaviour();
            }
            else
            {
                PatrolBehaviour();
            }

            UpdateTimers();
        }

        void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
        }

        void PatrolBehaviour()
        {
            Vector3 nextPosition = guardPosition;

            if (transform.position == guardPosition)
            {
                transform.rotation= Quaternion.RotateTowards(transform.rotation, guardRotation, 1);
            }

            if (patrolPath != null)
            {
                if (AtWaypoint())
                {
                    timeSinceArrivedAtWaypoint = 0;
                    CycleWaypoint();
                }

                nextPosition = GetCurrentWaypoint();
            }

            if (timeSinceArrivedAtWaypoint > waypointWaitTime)
            {
                mover.StartMoveAction(nextPosition, patrolSpeedFraction);
            }
        }
        bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerance;
        }

        void CycleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }
        
        Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        void SuspicionBehaviour()
        {
            scheduler.CancelCurrentAction();
        }

        void AttackBehaviour()
        {
            timeSinceLastSawPlayer = 0;
            fighter.Attack(player);
        }

        bool InAttackRange()
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            return distanceToPlayer <= chaseDistance;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}