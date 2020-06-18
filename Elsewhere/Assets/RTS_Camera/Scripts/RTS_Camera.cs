using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;
using System;

namespace RTS_Cam
{
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("RTS Camera")]
    public class RTS_Camera : MonoBehaviour
    {

        #region Foldouts

#if UNITY_EDITOR

        public int lastTab = 0;

        public bool movementSettingsFoldout;
        public bool zoomingSettingsFoldout;
        public bool rotationSettingsFoldout;
        public bool heightSettingsFoldout;
        public bool mapLimitSettingsFoldout;
        public bool targetingSettingsFoldout;
        public bool inputSettingsFoldout;

#endif

        #endregion

        private Transform m_Transform; //camera tranform
        public bool useFixedUpdate = false; //use FixedUpdate() or Update()

        #region Movement

        public float keyboardMovementSpeed; //speed with keyboard movement
        public float screenEdgeMovementSpeed; //spee with screen edge movement
        public float followingSpeed; //speed when following a target
        public float rotationSpeed;
        public float panningSpeed;
        public float mouseRotationSpeed;

        #endregion

        #region Height

        public bool autoHeight = true;
        public LayerMask groundMask = -1; //layermask of ground or other objects that affect height

        public float maxHeight; //maximal height
        public float minHeight; //minimnal height
        public float heightDampening = 5f; 
        public float keyboardZoomingSensitivity = 2f;
        public float scrollWheelZoomingSensitivity = 25f;

        private float zoomPos = 0; //value in range (0, 1) used as t in Matf.Lerp

        #endregion

        #region MapLimits

        public bool limitMap;
        public float limitX = 50f; //x limit of map
        public float limitY = 50f; //y limit of map
        public Vector2 limitOffSet;

        #endregion

        #region Camera Orthographic

        public Camera mainCamera;
        public float m_orthographicSize;

        #endregion

        #region Targeting

        public Transform targetFollow; //target to follow
        public Vector3 targetOffset;

        /// <summary>
        /// are we following target
        /// </summary>
        public bool FollowingTarget
        {
            get
            {
                return targetFollow != null;
            }
        }

        #endregion

        #region Input

        public bool useScreenEdgeInput = true;
        public float screenEdgePercentage = 0.25f;

        public bool useKeyboardInput = true;
        public string horizontalAxis = "Horizontal";
        public string verticalAxis = "Vertical";

        public bool usePanning = true;
        public KeyCode panningKey = KeyCode.Mouse2;

        public bool useKeyboardZooming = true;
        public KeyCode zoomInKey = KeyCode.E;
        public KeyCode zoomOutKey = KeyCode.Q;

        public bool useScrollwheelZooming = true;
        public string zoomingAxis = "Mouse ScrollWheel";

        public bool useKeyboardRotation = true;
        public KeyCode rotateRightKey = KeyCode.X;
        public KeyCode rotateLeftKey = KeyCode.Z;

        public bool useMouseRotation = true;
        public KeyCode mouseRotationKey = KeyCode.Mouse1;

        private Vector2 KeyboardInput
        {
            get { return useKeyboardInput ? new Vector2(Input.GetAxis(horizontalAxis), Input.GetAxis(verticalAxis)) : Vector2.zero; }
        }

        private Vector2 MouseInput
        {
            get { return Input.mousePosition; }
        }

        private float ScrollWheel
        {
            get { return Input.GetAxis(zoomingAxis); }
        }

        private Vector2 MouseAxis
        {
            get { return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")); }
        }

        private int ZoomDirection
        {
            get
            {
                bool zoomIn = Input.GetKey(zoomInKey);
                bool zoomOut = Input.GetKey(zoomOutKey);
                if (zoomIn && zoomOut)
                    return 0;
                else if (!zoomIn && zoomOut)
                    return 1;
                else if (zoomIn && !zoomOut)
                    return -1;
                else 
                    return 0;
            }
        }

        private int RotationDirection
        {
            get
            {
                bool rotateRight = Input.GetKey(rotateRightKey);
                bool rotateLeft = Input.GetKey(rotateLeftKey);
                if(rotateLeft && rotateRight)
                    return 0;
                else if(rotateLeft && !rotateRight)
                    return -1;
                else if(!rotateLeft && rotateRight)
                    return 1;
                else 
                    return 0;
            }
        }

        #endregion

        #region Unity_Methods

        private void Start()
        {
            m_Transform = transform;
            mainCamera = this.GetComponent<Camera>();
            m_orthographicSize = mainCamera.orthographicSize;
        }

        private void Update()
        {
            if (!useFixedUpdate)
                CameraUpdate();
            if (Input.GetKeyDown("x"))
            {
                Debug.Log("Orthographic size = " + m_orthographicSize);
            }
        }

        private void FixedUpdate()
        {
            if (useFixedUpdate)
                CameraUpdate();
        }

        #endregion

        #region RTSCamera_Methods

        /// <summary>
        /// update camera movement and rotation
        /// </summary>
        private void CameraUpdate()
        {
            if (FollowingTarget)
                FollowTarget();
            else
                Move();

            //HeightCalculation();
            ZoomCamera();
            Rotation();
            LimitPosition();
        }

        /// <summary>
        /// move camera with keyboard or with screen edge
        /// </summary>
        private void Move()
        {
            if (useKeyboardInput)
            {
                Vector3 desiredMove = new Vector3(KeyboardInput.x, KeyboardInput.y,0);

                desiredMove *= keyboardMovementSpeed;
                desiredMove *= Time.deltaTime;
                desiredMove = Quaternion.Euler(new Vector3(0f, 0f, transform.eulerAngles.z)) * desiredMove;
                desiredMove = m_Transform.InverseTransformDirection(desiredMove);

                m_Transform.Translate(desiredMove, Space.Self);
            }

            if (useScreenEdgeInput)
            {
                Vector3 desiredMove = new Vector3();

                Rect leftRect = new Rect(0, 0, Screen.width*screenEdgePercentage, Screen.height);
                Rect rightRect = new Rect(Screen.width * (1 - screenEdgePercentage), 0, Screen.width * screenEdgePercentage, Screen.height);
                Rect upRect = new Rect(0, Screen.height * (1 - screenEdgePercentage), Screen.width, Screen.height * screenEdgePercentage);
                Rect downRect = new Rect(0, 0, Screen.width, Screen.height * screenEdgePercentage);

                desiredMove.x = leftRect.Contains(MouseInput) ? -1 : rightRect.Contains(MouseInput) ? 1 : 0;
                desiredMove.y = upRect.Contains(MouseInput) ? 1 : downRect.Contains(MouseInput) ? -1 : 0;

                desiredMove *= screenEdgeMovementSpeed;
                desiredMove *= Time.deltaTime;
                desiredMove = Quaternion.Euler(new Vector3(0f, 0f, transform.eulerAngles.z)) * desiredMove;
                desiredMove = m_Transform.InverseTransformDirection(desiredMove);

                m_Transform.Translate(desiredMove, Space.Self);
            }       
        
            if(usePanning && Input.GetKey(panningKey) && MouseAxis != Vector2.zero)
            {
                Vector3 desiredMove = new Vector3(-MouseAxis.x, -MouseAxis.y, 0);

                desiredMove *= panningSpeed;
                desiredMove *= Time.deltaTime;
                desiredMove = Quaternion.Euler(new Vector3(0f, 0f, transform.eulerAngles.z)) * desiredMove;
                desiredMove = m_Transform.InverseTransformDirection(desiredMove);

                m_Transform.Translate(desiredMove, Space.Self);
            }
        }

        /// <summary>
        /// calcualte height
        /// </summary>
        private void HeightCalculation()
        {
            float distanceToGround = DistanceToGround();
            if (useScrollwheelZooming)
                zoomPos += -ScrollWheel * Time.deltaTime * scrollWheelZoomingSensitivity;
            if (useKeyboardZooming)
                zoomPos += ZoomDirection * Time.deltaTime * keyboardZoomingSensitivity;

            zoomPos = Mathf.Clamp01(zoomPos);

            float targetHeight = Mathf.Lerp(-minHeight, -maxHeight, zoomPos);
            float difference = 0; 

            if(distanceToGround != targetHeight)
                difference = targetHeight - distanceToGround;

            m_Transform.position = Vector3.Lerp(m_Transform.position, 
                new Vector3(m_Transform.position.x, m_Transform.position.y, targetHeight + difference), Time.deltaTime * heightDampening);
        }

        private void ZoomCamera()
        {
            if (useScrollwheelZooming)
                zoomPos += -ScrollWheel * Time.deltaTime * scrollWheelZoomingSensitivity;
            if (useKeyboardZooming)
                zoomPos += ZoomDirection * Time.deltaTime * keyboardZoomingSensitivity;
            zoomPos = Mathf.Clamp01(zoomPos);
            
            float targetOrthoSize = Mathf.Lerp(minHeight, maxHeight, zoomPos);

            if (m_orthographicSize != targetOrthoSize)
            {
                mainCamera.orthographicSize = lerp(m_orthographicSize, targetOrthoSize, Mathf.Clamp01(Time.deltaTime * heightDampening));
            }
            m_orthographicSize = mainCamera.orthographicSize;
        }

        private static float lerp(float a, float b, float f)
        {
            return (a * (1.0f - f)) + (b * f);
        }

        /// <summary>
        /// rotate camera
        /// </summary>
        private void Rotation()
        {
            if(useKeyboardRotation)
                transform.Rotate(Vector3.up, RotationDirection * Time.deltaTime * rotationSpeed, Space.World);

            if (useMouseRotation && Input.GetKey(mouseRotationKey))
                m_Transform.Rotate(Vector3.up, -MouseAxis.x * Time.deltaTime * mouseRotationSpeed, Space.World);
        }

        /// <summary>
        /// follow targetif target != null
        /// </summary>
        private void FollowTarget()
        {
            Vector3 targetPos = new Vector3(targetFollow.position.x, m_Transform.position.y, targetFollow.position.z) + targetOffset;
            m_Transform.position = Vector3.MoveTowards(m_Transform.position, targetPos, Time.deltaTime * followingSpeed);
        }

        /// <summary>
        /// limit camera position
        /// </summary>
        private void LimitPosition()
        {
            if (!limitMap)
                return;

            float orthoXLimit = limitX - m_orthographicSize / Screen.height * Screen.width; ;
            float orthoYLimit = limitY - m_orthographicSize;
            m_Transform.position = new Vector3(Mathf.Clamp(m_Transform.position.x, -orthoXLimit + limitOffSet.x, orthoXLimit + limitOffSet.x),
                Mathf.Clamp(m_Transform.position.y, -orthoYLimit + limitOffSet.y, orthoYLimit + limitOffSet.y), m_Transform.position.z);
        }

        /// <summary>
        /// set the target
        /// </summary>
        /// <param name="target"></param>
        public void SetTarget(Transform target)
        {
            targetFollow = target;
        }

        /// <summary>
        /// reset the target (target is set to null)
        /// </summary>
        public void ResetTarget()
        {
            targetFollow = null;
        }

        /// <summary>
        /// calculate distance to ground
        /// </summary>
        /// <returns></returns>
        private float DistanceToGround()
        {
            Ray ray = new Ray(m_Transform.position, Vector3.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, groundMask.value))
                return (hit.point - m_Transform.position).magnitude;

            return 0f;
        }

        #endregion
    }
}