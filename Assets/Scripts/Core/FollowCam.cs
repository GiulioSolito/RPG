using UnityEngine;

namespace RPG.Core
{
    public class FollowCam : MonoBehaviour
    {
        [SerializeField] private Transform target;

        void LateUpdate()
        {
            transform.position = target.position;
        }
    }
}