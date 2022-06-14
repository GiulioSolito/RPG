using RPG.Combat;
using RPG.Movement;
using UnityEngine;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        private Mover mover;
        private Fighter fighter;
        
        void Awake()
        {
            mover = GetComponent<Mover>();
            fighter = GetComponent<Fighter>();
        }

        void Update()
        {
            if (InteractWithCombat()) return;
            if(InteractWithMovement()) return;
        }

        bool InteractWithMovement()
        {
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);

            if (hasHit)
            {
                if (Input.GetMouseButton(0))
                {
                    mover.StartMoveAction(hit.point);
                }
                return true;
            }
            return false;
        }
        
        bool InteractWithCombat()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());

            foreach (RaycastHit hit in hits)
            {
                CombatTarget target = hit.collider.GetComponent<CombatTarget>();
                
                if (fighter.CanAttack(target) == false)
                {
                    continue;
                }

                if (Input.GetMouseButtonDown(0))
                {
                    Fighter fighter = GetComponent<Fighter>();

                    if (fighter != null)
                    {
                        fighter.Attack(target);
                    }
                }
                return true;
            }
            return false;
        }
        
        static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}