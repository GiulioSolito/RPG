using System;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Control
{
    public class ItemCollector : MonoBehaviour, IAction
    {
        [SerializeField] private float pickupDistance = 1f;

        private Mover mover;
        private ActionScheduler scheduler;
        private Pickup pickup;

        void Awake()
        {
            mover = GetComponent<Mover>();
            scheduler = GetComponent<ActionScheduler>();
        }

        void Update()
        {
            if (pickup == null) return;
            mover.MoveTo(pickup.transform.position, 1f);

            if (Vector3.Distance(transform.position, pickup.transform.position) <= pickupDistance)
            {
                mover.Cancel();
                pickup.PickupItem();
            }
        }

        public void StartPickupCollector(Pickup pickup)
        {
            scheduler.StartAction(this);
            this.pickup = pickup;
        }

        public void StopPickup()
        {
            pickup = null;
        }

        public void Cancel()
        {
            StopPickup();
            mover.Cancel();
        }
    }
}