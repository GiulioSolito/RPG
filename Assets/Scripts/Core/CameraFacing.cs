using UnityEngine;
using Cinemachine;

namespace RPG.Core
{
    public class CameraFacing : MonoBehaviour
    {
        [SerializeField] private CinemachineFreeLook playerFramingCamera;

        void Start()
        {
            playerFramingCamera = GameObject.FindGameObjectWithTag("PlayerCam").GetComponent<CinemachineFreeLook>();
        }

        void LateUpdate()
        {
            transform.LookAt(2 * transform.position - playerFramingCamera.transform.position);
        }
    }
}