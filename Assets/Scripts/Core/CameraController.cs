using System;
using UnityEngine;
using Cinemachine;
using RPG.Control;
using GameDevTV.Saving;
using Sirenix.OdinInspector;

namespace RPG.Core
{
    public class CameraController : MonoBehaviour,ISaveable
    {
        [TabGroup("Camera")]
        [SerializeField] private GameObject freeLookCamera;

        [TabGroup("Camera Options")]
        [SerializeField] private float xSpeed = 500f;
        [TabGroup("Camera Options")]
        [SerializeField] private float ySpeed = 50f;
        [TabGroup("Camera Options")]
        [SerializeField] private bool invertX = false;
        [TabGroup("Camera Options")]
        [SerializeField] private bool invertY = true;
        
        private CinemachineFreeLook freeLookComponent;
        private PlayerController playerControllerScript;

        void Awake()
        {
            freeLookComponent = freeLookCamera.GetComponent<CinemachineFreeLook>();
            playerControllerScript = GetComponent<PlayerController>();
        }

        void Start()
        {
            freeLookComponent.m_XAxis.m_InvertInput = invertX;
            freeLookComponent.m_YAxis.m_InvertInput = invertY;
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                // if (playerControllerScript.isDraggingUI) return;

                // use the following line for mouse control of zoom instead of mouse wheel
                // be sure to change Input Axis Name on the Y axis to "Mouse Y"

                //freeLookComponent.m_YAxis.m_MaxSpeed = 10;
                
                freeLookComponent.m_XAxis.m_MaxSpeed = xSpeed;
            }
            if (Input.GetMouseButtonUp(1))
            {
                // use the following line for mouse control of zoom instead of mouse wheel
                // be sure to change Input Axis Name on the Y axis from to "Mouse Y"

                //freeLookComponent.m_YAxis.m_MaxSpeed = 0;
                freeLookComponent.m_XAxis.m_MaxSpeed = 0;
            }

            // wheel zoom //
            // comment out the below if condition if you are using mouse control for zoom
            if (Input.mouseScrollDelta.y != 0)
            {
                freeLookComponent.m_YAxis.m_MaxSpeed = ySpeed;
            }
        }

        [System.Serializable]
        struct CameraSaveData
        {
            public float xSpeed;
            public float ySpeed;
            public bool invertX;
            public bool invertY;
        }
        
        public object CaptureState()
        {
            CameraSaveData data = new CameraSaveData();
            data.xSpeed = xSpeed;
            data.ySpeed = ySpeed;
            data.invertX = invertX;
            data.invertY = invertY;
            return data;
        }

        public void RestoreState(object state)
        {
            CameraSaveData data = (CameraSaveData) state;
            xSpeed = data.xSpeed;
            ySpeed = data.ySpeed;
            invertX = data.invertX;
            invertY = data.invertY;
        }
    }
}