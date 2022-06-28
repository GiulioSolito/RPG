using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [TabGroup("Behaviour Timers")]
        [TabGroup("Behaviour Timers")]
        [SerializeField] private float chaseDistance = 5f;
        [TabGroup("Behaviour Timers")]
        [SerializeField] private float suspicionTime = 3f;
        [TabGroup("Behaviour Timers")]
        [SerializeField] private float aggroCooldownTime = 5f;

        [TabGroup("Line of Sight")] 
        [SerializeField] private float angle = 30f;
        [TabGroup("Line of Sight")]
        [SerializeField] private float height = 1f;
        [TabGroup("Line of Sight")] 
        [SerializeField] private Color meshColor = Color.red;
        [TabGroup("Line of Sight")] 
        [SerializeField] private int scanFrequency = 30;
        [TabGroup("Line of Sight")] 
        [SerializeField] private LayerMask layers;
        [TabGroup("Line of Sight")] 
        [SerializeField] private LayerMask occulusionLayers;
        [TabGroup("Line of Sight")] 
        [SerializeField] private List<GameObject> objects = new List<GameObject>();
        
        private Collider[] colliders = new Collider[50];
        private Mesh mesh;
        private int count;
        private float scanInterval;
        private float scanTimer;
        
        [TabGroup("Wapoints")]
        [SerializeField] private PatrolPath patrolPath;
        [TabGroup("Wapoints")]
        [SerializeField] private float waypointTolerance = 1f;
        [TabGroup("Wapoints")]
        [SerializeField] private float waypointWaitTime = 3f;
        [TabGroup("Wapoints")][Range(0,1)]
        [SerializeField] private float patrolSpeedFraction = 0.2f;
        
        [TabGroup("Shout")]
        [SerializeField] private float shoutDistance = 5f;

        private GameObject player;

        private Vector3 guardPosition;
        private Quaternion guardRotation;
        
        private float timeSinceLastSawPlayer = Mathf.Infinity;
        private float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        private float timeSinceAggravated = Mathf.Infinity;
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

            scanInterval = 1f / scanFrequency;
        }

        void Update()
        {
            if (health.IsDead) return;
            if (IsAggravated() && fighter.CanAttack(player))
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

        void Aggravate()
        {
            timeSinceAggravated = 0;
        }
        
        void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
            timeSinceAggravated += Time.deltaTime;

            scanTimer -= Time.deltaTime;

            if (scanTimer < 0)
            {
                scanTimer += scanInterval;
                Scan();
            }
        }

        void Scan()
        {
            count = Physics.OverlapSphereNonAlloc(transform.position, chaseDistance, colliders, layers,
                QueryTriggerInteraction.Collide);
            
            objects.Clear();

            for (int i = 0; i < count; i++)
            {
                GameObject obj = colliders[i].gameObject;

                if (IsInSight(obj))
                {
                    objects.Add(obj);
                }
            }
        }

        bool IsInSight(GameObject obj)
        {
            Vector3 origin = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            Vector3 dest = obj.transform.position;
            Vector3 direction = dest - origin;

            direction.y = 0;
            float deltaAngle = Vector3.Angle(direction, transform.forward);

            if (deltaAngle > angle)
            {
                return false;
            }

            origin.y += height / 2;
            dest.y = origin.y;

            if (Physics.Linecast(origin,dest,occulusionLayers))
            {
                return false;
            }
            
            return true;
        }

        void PatrolBehaviour()
        {
            print("patrol");
            Vector3 nextPosition = guardPosition;

            if (Vector3.Distance(transform.position, guardPosition) < Mathf.Epsilon)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, guardRotation, 1);
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
            AggravateNearbyEnemies();
        }

        void AggravateNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);

            foreach (RaycastHit hit in hits)
            {
                AIController controller = hit.collider.GetComponent<AIController>();

                if (controller == this) continue;
                if (controller == null) continue;
                
                // controller.Aggravate();
            }
        }

        bool IsAggravated()
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            return distanceToPlayer <= chaseDistance && IsInSight(player) || timeSinceAggravated < aggroCooldownTime;
        }

        Mesh CreateWedgeMesh()
        {
            mesh = new Mesh();

            int segments = 10;
            int numTriangles = (segments * 4) + 2 + 2;
            int numVertices = numTriangles * 3;

            Vector3[] vertices = new Vector3[numVertices];
            int[] triangles = new int[numVertices];

            Vector3 bottomCenter = Vector3.zero;
            Vector3 bottomLeft = Quaternion.Euler(0, -angle, 0) * Vector3.forward * chaseDistance;
            Vector3 bottomRight = Quaternion.Euler(0, angle, 0) * Vector3.forward * chaseDistance;

            Vector3 topCenter = bottomCenter + Vector3.up * height;
            Vector3 topRight = bottomRight + Vector3.up * height;
            Vector3 topLeft = bottomLeft + Vector3.up * height;

            int vert = 0;
            
            //left side
            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomLeft;
            vertices[vert++] = topLeft;
            
            vertices[vert++] = topLeft;
            vertices[vert++] = topCenter;
            vertices[vert++] = bottomCenter;
            
            //right side
            vertices[vert++] = bottomCenter;
            vertices[vert++] = topCenter;
            vertices[vert++] = topRight;
            
            vertices[vert++] = topRight;
            vertices[vert++] = bottomRight;
            vertices[vert++] = bottomCenter;

            float currentAngle = -angle;
            float deltaAngle = (angle * 2) / segments;

            for (int i = 0; i < segments; i++)
            {
                bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * chaseDistance;
                bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * chaseDistance;

                topRight = bottomRight + Vector3.up * height;
                topLeft = bottomLeft + Vector3.up * height;
                
                //far side
                vertices[vert++] = bottomLeft;
                vertices[vert++] = bottomRight;
                vertices[vert++] = topRight;

                vertices[vert++] = topRight;
                vertices[vert++] = topLeft;
                vertices[vert++] = bottomLeft;
            
                //top
                vertices[vert++] = topCenter;
                vertices[vert++] = topLeft;
                vertices[vert++] = topRight;
            
                //bottom
                vertices[vert++] = bottomCenter;
                vertices[vert++] = bottomRight;
                vertices[vert++] = bottomLeft;
                
                currentAngle += deltaAngle;
            }

            for (int i = 0; i < numVertices; i++)
            {
                triangles[i] = i;
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            
            return mesh;
        }

        void OnValidate()
        {
            mesh = CreateWedgeMesh();
            scanInterval = 1f / scanFrequency;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);

            if (mesh)
            {
                Gizmos.color = meshColor;
                Gizmos.DrawMesh(mesh, transform.position, transform.rotation);
            }

            Gizmos.color = Color.green;

            foreach (GameObject obj in objects)
            {
                Gizmos.DrawSphere(obj.transform.position, 0.2f);
            }
        }
    }
}