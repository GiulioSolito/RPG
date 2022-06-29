using System;
using RPG.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    { 
        [SerializeField] private float speed = 1f;
        [SerializeField] private bool isHoming = true;
        [SerializeField] private GameObject hitEffect;
        [SerializeField] private float maxLifetime = 10f;

        [SerializeField] private GameObject[] destroyOnHit;
        [SerializeField] private float lifeAfterImpact = 2f;
        [SerializeField] private UnityEvent onHit;
        [SerializeField] private Transform projectileOwner;

        private Health target;
        private GameObject instigator;
        private float damage;

        void Start()
        {
            transform.LookAt(GetAimLocation());
        }

        void Update()
        {
            if (target == null) return;

            if (isHoming && target.IsDead == false)
            {
                transform.LookAt(GetAimLocation());
            }
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        public Transform SetOwner(Transform owner)
        {
            projectileOwner = owner;
            return owner;
        }
        
        public void SetTarget(Health target, GameObject instigator, float damage)
        {
            this.target = target;
            this.damage = damage;
            this.instigator = instigator;
            
            Destroy(gameObject, maxLifetime);
        }

        Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();

            if (targetCapsule == null) return target.transform.position;
            
            return target.transform.position + Vector3.up * targetCapsule.height / 2;
        }

        void OnTriggerEnter(Collider other)
        {
            Health health = other.GetComponent<Health>();

            if (other.transform == projectileOwner) return;

            if (health != null)
            {
                if (health.IsDead) return;

                if (health == target)
                {
                    target.TakeDamage(instigator, damage);
                }
            }

            speed = 0;
            onHit.Invoke();
            
            if (hitEffect != null && health == target)
            {
                Instantiate(hitEffect, GetAimLocation(), transform.rotation);
            }
            else if (hitEffect != null && health != target)
            {
                Instantiate(hitEffect, transform.position, transform.rotation);
            }

            foreach (GameObject toDestroy in destroyOnHit)
            {
                Destroy(toDestroy);
            }
            
            Destroy(gameObject, lifeAfterImpact);
        }
    }
}