//
// Fingers Gestures
// (c) 2015 Digital Ruby, LLC
// http://www.digitalruby.com
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DigitalRubyShared
{
    /// <summary>
    /// Allows two finger pan, scale and rotate on a game object
    /// </summary>
    [AddComponentMenu("Fingers Gestures/Component/Fingers Pan Rotate Scale", 4)]
    public class FingersPanRotateScaleComponentScript : MonoBehaviour
    {
        /// <summary>
        /// Double tap reset mode
        /// </summary>
        public enum _DoubleTapResetMode
        {
            /// <summary>
            /// No reset on double tap
            /// </summary>
            Off = 0,

            /// <summary>
            /// Reset scale and rotation on double tap
            /// </summary>
            ResetScaleRotation = 1,

            /// <summary>
            /// Reset scale, rotation and position on double tap
            /// </summary>
            ResetScaleRotationPosition = 2
        }

        /// <summary>The cameras to use to convert screen coordinates to world coordinates. Defaults to Camera.main.</summary>
        [Header("Setup")]
        [Tooltip("The cameras to use to convert screen coordinates to world coordinates. Defaults to Camera.main.")]
        public Camera[] Cameras;

        /// <summary>Whether to bring the object to the front when a gesture executes on it</summary>
        [Tooltip("Whether to bring the object to the front when a gesture executes on it")]
        public bool BringToFront = true;

        /// <summary>Whether the gestures in this script can execute simultaneously with all other gestures.</summary>
        [Tooltip("Whether the gestures in this script can execute simultaneously with all other gestures.")]
        public bool AllowExecutionWithAllGestures;

        /// <summary>The mode to execute in, can be require over game object or allow on any game object.</summary>
        [Tooltip("The mode to execute in, can be require over game object or allow on any game object.")]
        public GestureRecognizerComponentScriptBase.GestureObjectMode Mode = GestureRecognizerComponentScriptBase.GestureObjectMode.RequireIntersectWithGameObject;

        /// <summary>The minimum and maximum scale as a percentage of the original scale of the object. 0,0 for no limits. -1,-1 for no scaling.</summary>
        [Tooltip("The minimum and maximum scale as a percentage of the original scale of the object. 0,0 for no limits. -1,-1 for no scaling.")]
        public Vector2 MinMaxScale;

        /// <summary>The threshold in units touches must move apart or together to begin scaling.</summary>
        [Tooltip("The threshold in units touches must move apart or together to begin scaling.")]
        [Range(0.0f, 1.0f)]
        public float ScaleThresholdUnits = 0.15f;

        /// <summary>Whether to add a double tap to reset the transform of the game object this script is on. This must be set in the inspector and not changed.</summary>
        [Header("Enable / Disable Gestures")]
        [Tooltip("Whether to add a double tap to reset the transform of the game object this script is on. This must be set in the inspector and not changed.")]
        public _DoubleTapResetMode DoubleTapResetMode;

        /// <summary>Whether to allow panning. Can be set during editor or runtime.</summary>
        [Tooltip("Whether to allow panning. Can be set during editor or runtime.")]
        public bool AllowPan = true;

        /// <summary>Whether to allow scaling. Can be set during editor or runtime.</summary>
        [Tooltip("Whether to allow scaling. Can be set during editor or runtime.")]
        public bool AllowScale = true;

        /// <summary>Whether to allow rotating. Can be set during editor or runtime.</summary>
        [Tooltip("Whether to allow rotating. Can be set during editor or runtime.")]
        public bool AllowRotate = true;

        /// <summary>Gesture state updated event</summary>
        [Tooltip("Gesture state updated event")]
        public GestureRecognizerComponentStateUpdatedEvent StateUpdated;

        /// <summary>
        /// Allow moving the target
        /// </summary>
        public PanGestureRecognizer PanGesture { get; private set; }

        /// <summary>
        /// Allow scaling the target
        /// </summary>
        public ScaleGestureRecognizer ScaleGesture { get; private set; }

        /// <summary>
        /// Allow rotating the target
        /// </summary>
        public RotateGestureRecognizer RotateGesture { get; private set; }

        /// <summary>
        /// The double tap gesture or null if DoubleTapToReset was false when this script started up
        /// </summary>
        public TapGestureRecognizer DoubleTapGesture { get; private set; }

        private Rigidbody2D rigidBody2D;
        private Rigidbody rigidBody;
        private SpriteRenderer spriteRenderer;
        private CanvasRenderer canvasRenderer;
        private Transform _transform;
        private int startSortOrder;
        private float panZ;
        private Vector3 panOffset;
        private Vector3? startScale;
        bool isMove = false;

        float tempYPos;

        float tempZPos;
        Vector3 temp = Vector3.zero;

        private struct SavedState
        {
            public Vector3 Scale;
            public Quaternion Rotation;
            public Vector3 Position;
        }
        private readonly Dictionary<Transform, SavedState> savedStates = new Dictionary<Transform, SavedState>();

        private void Start()
        {
            if (!isMinScaleDefaultUsed)
            {
                if (SceneManager.GetActiveScene().name == "ARModulePlanDetectionScene")
                {
                    minscaleValue = (this.gameObject.transform.localScale.x / 30);
                    maxScaleValue = 1.8f;
                }
                else
                {
                    minscaleValue = (this.gameObject.transform.localScale.x / 2);
                }
            }
            //Debug.LogError("minscaleValue:" + minscaleValue + "  :maxScaleValue:" + maxScaleValue);
        }

        public void SetMinScaleOfAvatar()
        {
            if (!isMinScaleDefaultUsed)
            {
                if (SceneManager.GetActiveScene().name == "ARModulePlanDetectionScene")
                {
                    minscaleValue = (this.gameObject.transform.localScale.x / 30);
                    maxScaleValue = 1.8f;
                }
                else
                {
                    minscaleValue = (this.gameObject.transform.localScale.x / 2);
                }
            }
        }

        private void PanGestureUpdated(DigitalRubyShared.GestureRecognizer panGesture)
        {
            if (!AllowPan)
            {
                panGesture.Reset();
                return;
            }

            Camera camera;
            GameObject obj = FingersScript.StartOrResetGesture(panGesture, BringToFront, Cameras, gameObject, spriteRenderer, Mode, out camera);
            if (camera == null)
            {
                panGesture.Reset();
                return;
            }

            StateUpdated.Invoke(PanGesture);
            if (panGesture.State == GestureRecognizerState.Began)
            {
                //tempYPos = transform.position.y;
                //tempZPos = transform.position.z;
                SetStartState(panGesture, obj, false);
            }
            else if (panGesture.State == GestureRecognizerState.Executing && _transform != null)
            {
                if (PanGesture.ReceivedAdditionalTouches)
                {
                    panZ = camera.WorldToScreenPoint(_transform.position).z;
                    if (canvasRenderer == null)
                    {
                        panOffset = _transform.position - camera.ScreenToWorldPoint(new Vector3(panGesture.FocusX, panGesture.FocusY, panZ));
                    }
                    else
                    {
                        Vector2 screenToCanvasPoint = canvasRenderer.GetComponentInParent<Canvas>().ScreenToCanvasPoint(new Vector2(panGesture.FocusX, panGesture.FocusY));
                        panOffset = new Vector3(screenToCanvasPoint.x - _transform.position.x, screenToCanvasPoint.y - _transform.position.y, 0.0f);
                    }
                }

                Vector3 gestureScreenPoint = new Vector3(panGesture.FocusX, panGesture.FocusY, panZ);
                Vector3 gestureWorldPoint = camera.ScreenToWorldPoint(gestureScreenPoint) + panOffset;

                //Debug.Log("gestureScreenPoint : " + gestureScreenPoint);
                //Debug.Log("gestureWorldPoint : " + gestureWorldPoint);
                if (rigidBody != null)
                {
                    if (SceneManager.GetActiveScene().name == "ARModulePlanDetectionScene" && !LiveVideoRoomManager.Instance.IsVideoScreenImageScreenAvtive)
                    {
                        //temp = new Vector3(gestureWorldPoint.x, gestureWorldPoint.y, tempZPos - (tempYPos - gestureWorldPoint.y));
                        //  temp = new Vector3(gestureWorldPoint.x, gestureWorldPoint.y, 0)+(camera.transform.forward*( tempZPos - (tempYPos - gestureWorldPoint.y)));

                        //transform.LookAt(camera.transform);
                        //tempYPos = transform.position.y;
                        temp = new Vector3(gestureWorldPoint.x, gestureWorldPoint.y, gestureWorldPoint.z) + (camera.transform.forward * gestureWorldPoint.y);
                        //Debug.LogError("TempPos:" + temp + ":this pos:" + this.transform.position + ":Camera Pos:" + camera.transform.forward + ":gestureWordPos z:" + gestureWorldPoint.z + ":gestureWordPos Y:" + gestureWorldPoint.y + ":gestureScreenPoint.y:"+ gestureScreenPoint.y);
                        //rigidBody.MovePosition(temp);

                        rigidBody.MovePosition(gestureWorldPoint);

                        //Debug.LogError("CameraPos:" + camera.transform.position + "   :Camera rotation:" + camera.transform.rotation);
                    }
                    else
                    {
                        Vector3 stageDimensions = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

                        var p1 = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
                        var p2 = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, Camera.main.nearClipPlane));
                        var p3 = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, Camera.main.nearClipPlane));
                        var p4 = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.nearClipPlane));

                        Vector2 dimensions = CalculateScreenSizeInWorldCoords();

                        // If you want the min max values to update if the resolution changes 
                        // set them in update else set them in Start
                        float camDistance = Vector3.Distance(transform.position, Camera.main.transform.position);
                        Vector2 bottomCorner = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, camDistance));
                        Vector2 topCorner = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, camDistance));

                        float minX = bottomCorner.x;
                        float maxX = topCorner.x;
                        float minY = bottomCorner.y;
                        float maxY = topCorner.y;

                        //Debug.LogError("Width =" + dimensions.x + " ;Height is =" + dimensions.y + "       :PlayerPosition:" + this.transform.position + "  :maxX:"+maxX+"  :minx:"+minX+"  :maxy:"+maxY+"  :miny:"+minY);

                        //Debug.LogError("vector3:" + stageDimensions + "     :P1:" + p1 + "      :P2:" + p2 + "      :p3:" + p3 + "      :p4:" + p4);
                        //if(this.transform.position)
                        //rigidBody.MovePosition(gestureWorldPoint);
                        Vector3 ccc;
                        if (SceneManager.GetActiveScene().name == "ARModuleActionScene" || SceneManager.GetActiveScene().name == "ARModuleRealityScene")
                        {
                            ccc = new Vector3(Mathf.Clamp(gestureWorldPoint.x, -0.35f, 0.35f), Mathf.Clamp(gestureWorldPoint.y, -2.7f, -0.4f), gestureWorldPoint.z);
                        }
                        else
                        {
                            ccc = new Vector3(Mathf.Clamp(gestureWorldPoint.x, -0.3f, 0.3f), Mathf.Clamp(gestureWorldPoint.y, -1f, -0.4f), gestureWorldPoint.z);
                        }
                        rigidBody.MovePosition(ccc);

                        //rigidBody.MovePosition(new Vector3(Mathf.Clamp(gestureWorldPoint.x, (minX + 0.1f), (maxX - 0.1f)), Mathf.Clamp(gestureScreenPoint.y, (minY - 0.4f), (maxY - 1.2f)),gestureScreenPoint.z));
                        //rigidBody.transform.position = Vector3.MoveTowards(this.transform.position, new Vector3(Mathf.Clamp(gestureWorldPoint.x, (minX + 0.1f), (maxX - 0.1f)), Mathf.Clamp(gestureScreenPoint.y, (minY - 0.4f), (maxY - 1.2f)), gestureScreenPoint.z), Time.deltaTime);
                        
                        //this.transform.position = new Vector3(Mathf.Clamp(this.transform.position.x, (minX + 0.1f), (maxX - 0.1f)), Mathf.Clamp(this.transform.position.y, (minY - 0.2f), (maxY - 0.8f)), this.transform.position.z);
                        
                        //float x = Mathf.Clamp(this.transform.position.x, (minX + 0.1f), (maxX - 0.1f));
                        //float y = Mathf.Clamp(this.transform.position.y, (minY - 0.4f), (maxY - 1.2f));                    
                    }
                    // rigidBody.MovePosition(gestureWorldPoint);
                    if (rigidBody.gameObject.tag == "Player" && !isMove)
                    {
                        //Debug.LogError("Player : " + gameObject.name);
                        if (rigidBody != null && rigidBody.gameObject.GetComponent<AvatarBorderSelectionManager>() != null)
                        {
                            //Debug.LogError("Player : " + gameObject.name);
                            rigidBody.gameObject.GetComponent<AvatarBorderSelectionManager>().AvatarSelectionBorder(true);
                        }
                        isMove = true;
                    }
                }
                else if (rigidBody2D != null)
                {
                    rigidBody.MovePosition(gestureWorldPoint);
                }
                else if (canvasRenderer != null)
                {
                    _transform.position = gestureScreenPoint - panOffset;
                }
                else
                {
                    _transform.position = gestureWorldPoint;
                }
            }
            else if (panGesture.State == GestureRecognizerState.Ended)
            {
                if (spriteRenderer != null && BringToFront)
                {
                    spriteRenderer.sortingOrder = startSortOrder;
                }

                if (rigidBody != null && rigidBody.gameObject.GetComponent<AvatarBorderSelectionManager>() != null)
                {
                    rigidBody.gameObject.GetComponent<AvatarBorderSelectionManager>().AvatarSelectionBorder(false);
                }

                isMove = false;
                ClearStartState();
            }
        }

        Vector2 CalculateScreenSizeInWorldCoords()
        {
            var cam = Camera.main;
            var p1 = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
            var p2 = cam.ViewportToWorldPoint(new Vector3(1, 0, cam.nearClipPlane));
            var p3 = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));

            float width = (p2 - p1).magnitude;
            float height = (p3 - p2).magnitude;

            Vector2 dimensions = new Vector2(width, height);

            return dimensions;
        }

        public bool isMinScaleDefaultUsed = false;
        public float minscaleValue, maxScaleValue;
        private void ScaleGestureUpdated(DigitalRubyShared.GestureRecognizer scaleGesture)
        {
            //Debug.LogError("isAllow:" + AllowScale + " :MinMaxScale X:" + MinMaxScale.x + "  :MinMaxScale Y:" + MinMaxScale.y + ":Start:" + startScale + "   :Can Rotate:" + ARFaceModuleManager.Instance.m_CanRotateCharacter);
            if (!AllowScale)
            {
                scaleGesture.Reset();
                return;
            }
            else if (MinMaxScale.x < 0.0f || MinMaxScale.y < 0.0f || (startScale == null && !ARFaceModuleManager.Instance.m_CanRotateCharacter))
            {
                //Debug.LogError("startScale" + startScale);
                // no scaling
                return;
            }

            Camera camera;
            GameObject obj = FingersScript.StartOrResetGesture(scaleGesture, BringToFront, Cameras, gameObject, spriteRenderer, Mode, out camera);
            if (camera == null)
            {
                scaleGesture.Reset();
                return;
            }

            StateUpdated.Invoke(PanGesture);
            //Debug.LogError("Began" + scaleGesture.State);
            if (scaleGesture.State == GestureRecognizerState.Began)
            {
                SetStartState(scaleGesture, obj, false);
            }
            else if (scaleGesture.State == GestureRecognizerState.Executing && _transform != null)
            {
                Vector3 scale;
                // assume uniform scale
                /* if (SceneManager.GetActiveScene().name == "ARModuleActionScene")
                 {
                     if ((rigidBody.gameObject.name == "ARModuleAvatar"))
                     {
                         scale = new Vector3(
                                 Mathf.Clamp((_transform.localScale.x * ScaleGesture.ScaleMultiplier), minscaleValue, maxScaleValue),
                                 Mathf.Clamp((_transform.localScale.y * ScaleGesture.ScaleMultiplier), minscaleValue, maxScaleValue),
                                 Mathf.Clamp((_transform.localScale.z * ScaleGesture.ScaleMultiplier), minscaleValue, maxScaleValue)
                                             );
                     }
                     else
                     {
                         scale = new Vector3(
                                (_transform.localScale.x * ScaleGesture.ScaleMultiplier),
                                (_transform.localScale.y * ScaleGesture.ScaleMultiplier),
                                (_transform.localScale.z * ScaleGesture.ScaleMultiplier)
                                            );
                     }
                 }
                 else
                 {
                     scale = new Vector3(
                                (_transform.localScale.x * ScaleGesture.ScaleMultiplier),
                                (_transform.localScale.y * ScaleGesture.ScaleMultiplier),
                                (_transform.localScale.z * ScaleGesture.ScaleMultiplier)
                                            );
                 }*/
                //Debug.LogError("maxScaleValue " + minscaleValue);
                scale = new Vector3(
                               Mathf.Clamp((_transform.localScale.x * ScaleGesture.ScaleMultiplier), rigidBody.GetComponent<FingersPanRotateScaleComponentScript>().minscaleValue, maxScaleValue),
                               Mathf.Clamp((_transform.localScale.y * ScaleGesture.ScaleMultiplier), rigidBody.GetComponent<FingersPanRotateScaleComponentScript>().minscaleValue, maxScaleValue),
                               Mathf.Clamp((_transform.localScale.z * ScaleGesture.ScaleMultiplier), rigidBody.GetComponent<FingersPanRotateScaleComponentScript>().minscaleValue, maxScaleValue)
                                           );
                /*scale = new Vector3
               (
                 Mathf.Clamp((_transform.localScale.x * ScaleGesture.ScaleMultiplier), minscaleValue, maxScaleValue),
                   Mathf.Clamp((_transform.localScale.y * ScaleGesture.ScaleMultiplier), minscaleValue, maxScaleValue),
                   Mathf.Clamp((_transform.localScale.z * ScaleGesture.ScaleMultiplier), minscaleValue, maxScaleValue)
               );*/
                if (MinMaxScale.x > 0.0f || MinMaxScale.y > 0.0f)
                {
                    float minValue = Mathf.Min(MinMaxScale.x, MinMaxScale.y);
                    float maxValue = Mathf.Max(MinMaxScale.x, MinMaxScale.y);
                    scale.x = Mathf.Clamp(scale.x, startScale.Value.x * minValue, startScale.Value.x * maxValue);
                    scale.y = Mathf.Clamp(scale.y, startScale.Value.y * minValue, startScale.Value.y * maxValue);
                    scale.z = Mathf.Clamp(scale.z, startScale.Value.z * minValue, startScale.Value.z * maxValue);
                }
                if (rigidBody.gameObject.tag == "Player" && !isMove)
                {
                    if (rigidBody != null && rigidBody.gameObject.GetComponent<AvatarBorderSelectionManager>() != null)
                    {
                        rigidBody.gameObject.GetComponent<AvatarBorderSelectionManager>().AvatarSelectionBorder(true);
                    }
                    isMove = true;
                }

                // don't mess with z scale for 2D
                scale.z = (rigidBody2D == null && spriteRenderer == null && canvasRenderer == null ? scale.z : _transform.localScale.z);
                _transform.localScale = scale;
                //Debug.LogError("Scale:" + scale);
            }
            else if (scaleGesture.State == GestureRecognizerState.Ended)
            {
                if (rigidBody != null && rigidBody.gameObject.GetComponent<AvatarBorderSelectionManager>() != null)
                {
                    rigidBody.gameObject.GetComponent<AvatarBorderSelectionManager>().AvatarSelectionBorder(false);
                }

                ClearStartState();
                isMove = false;
            }
        }

        private void RotateGestureUpdated(DigitalRubyShared.GestureRecognizer rotateGesture)
        {
            if (!AllowRotate)
            {
                rotateGesture.Reset();
                return;
            }

            Camera camera;
            GameObject obj = FingersScript.StartOrResetGesture(rotateGesture, BringToFront, Cameras, gameObject, spriteRenderer, Mode, out camera);
            if (camera == null)
            {
                rotateGesture.Reset();
                return;
            }

            StateUpdated.Invoke(rotateGesture);
            if (rotateGesture.State == GestureRecognizerState.Began)
            {
                SetStartState(rotateGesture, obj, false);
            }
            else if (rotateGesture.State == GestureRecognizerState.Executing && _transform != null)
            {
                if (rigidBody != null)
                {
                    float angle = RotateGesture.RotationDegreesDelta;
                    Quaternion rotation = Quaternion.AngleAxis(angle, -Vector3.up);
                    rigidBody.MoveRotation(rigidBody.rotation * rotation);
                    if (rigidBody.gameObject.tag == "Player" && !isMove)
                    {
                        if (rigidBody != null && rigidBody.gameObject.GetComponent<AvatarBorderSelectionManager>() != null)
                        {
                            rigidBody.gameObject.GetComponent<AvatarBorderSelectionManager>().AvatarSelectionBorder(true);
                        }
                        isMove = true;
                    }
                }
                else if (rigidBody2D != null)
                {
                    rigidBody2D.MoveRotation(rigidBody2D.rotation + RotateGesture.RotationDegreesDelta * 3f);
                }
                else if (canvasRenderer != null)
                {
                    _transform.Rotate(Vector3.forward, RotateGesture.RotationDegreesDelta, Space.Self);
                }
                else
                {
                    _transform.Rotate(camera.transform.forward, RotateGesture.RotationDegreesDelta, Space.Self);
                }
            }
            else if (rotateGesture.State == GestureRecognizerState.Ended)
            {
                if (rigidBody != null && rigidBody.gameObject.GetComponent<AvatarBorderSelectionManager>() != null)
                {
                    rigidBody.gameObject.GetComponent<AvatarBorderSelectionManager>().AvatarSelectionBorder(false);
                }

                isMove = false;
                ClearStartState();
            }
        }

        private void DoubleTapGestureUpdated(DigitalRubyShared.GestureRecognizer r)
        {
            if (DoubleTapResetMode == _DoubleTapResetMode.Off)
            {
                r.Reset();
                return;
            }
            else if (r.State == GestureRecognizerState.Ended)
            {
                Camera camera = FingersScript.GetCameraForGesture(r, Cameras);
                GameObject obj = FingersScript.GestureIntersectsObject(r, camera, gameObject, Mode);
                SavedState state;
                if (obj != null && savedStates.TryGetValue(obj.transform, out state))
                {
                    obj.transform.rotation = state.Rotation;
                    obj.transform.localScale = state.Scale;
                    if (DoubleTapResetMode == _DoubleTapResetMode.ResetScaleRotationPosition)
                    {
                        obj.transform.position = state.Position;
                    }
                    savedStates.Remove(obj.transform);
                }
            }
        }

        private void ClearStartState()
        {
            if (Mode != GestureRecognizerComponentScriptBase.GestureObjectMode.AllowOnAnyGameObjectViaRaycast)
            {
                return;
            }
            else if (PanGesture.State != GestureRecognizerState.Executing &&
                RotateGesture.State != GestureRecognizerState.Executing &&
                ScaleGesture.State != GestureRecognizerState.Executing)
            {
                rigidBody2D = null;
                rigidBody = null;
                spriteRenderer = null;
                canvasRenderer = null;
                _transform = null;
            }
        }

        private bool SetStartState(DigitalRubyShared.GestureRecognizer gesture, GameObject obj, bool force)
        {
            if (!force && Mode != GestureRecognizerComponentScriptBase.GestureObjectMode.AllowOnAnyGameObjectViaRaycast)
            {
                return false;
            }
            else if (obj == null)
            {
                ClearStartState();
                return false;
            }
            else if (_transform == null)
            {
                rigidBody2D = obj.GetComponent<Rigidbody2D>();
                rigidBody = obj.GetComponent<Rigidbody>();
                spriteRenderer = obj.GetComponent<SpriteRenderer>();
                canvasRenderer = obj.GetComponent<CanvasRenderer>();
                if (spriteRenderer != null)
                {
                    startSortOrder = spriteRenderer.sortingOrder;
                }
                _transform = (rigidBody == null ? (rigidBody2D == null ? obj.transform : rigidBody2D.transform) : rigidBody.transform);
                if (DoubleTapResetMode != _DoubleTapResetMode.Off && !savedStates.ContainsKey(_transform))
                {
                    savedStates[_transform] = new SavedState { Rotation = _transform.rotation, Scale = _transform.localScale, Position = _transform.position };
                }
                if (startScale == null)
                {
                    startScale = _transform.localScale;
                }
            }
            else if (_transform != obj.transform)
            {
                if (gesture != null)
                {
                    gesture.Reset();
                }
                return false;
            }
            return true;
        }

        private void OnEnable()
        {
            if ((Cameras == null || Cameras.Length == 0) && Camera.main != null)
            {
                Cameras = new Camera[] { Camera.main };
            }
            PanGesture = new PanGestureRecognizer { MaximumNumberOfTouchesToTrack = 2, ThresholdUnits = 0.01f };
            PanGesture.StateUpdated += PanGestureUpdated;
            ScaleGesture = new ScaleGestureRecognizer();
            ScaleGesture.ThresholdUnits = ScaleThresholdUnits;
            ScaleGesture.StateUpdated += ScaleGestureUpdated;
            RotateGesture = new RotateGestureRecognizer();
            RotateGesture.StateUpdated += RotateGestureUpdated;
            if (Mode != GestureRecognizerComponentScriptBase.GestureObjectMode.AllowOnAnyGameObjectViaRaycast)
            {
                SetStartState(null, gameObject, true);
            }
            if (DoubleTapResetMode != _DoubleTapResetMode.Off)
            {
                DoubleTapGesture = new TapGestureRecognizer { NumberOfTapsRequired = 2 };
                DoubleTapGesture.StateUpdated += DoubleTapGestureUpdated;
            }
            if (AllowExecutionWithAllGestures)
            {
                PanGesture.AllowSimultaneousExecutionWithAllGestures();
                RotateGesture.AllowSimultaneousExecutionWithAllGestures();
                ScaleGesture.AllowSimultaneousExecutionWithAllGestures();
                if (DoubleTapGesture != null)
                {
                    DoubleTapGesture.AllowSimultaneousExecutionWithAllGestures();
                }
            }
            else
            {
                PanGesture.AllowSimultaneousExecution(ScaleGesture);
                PanGesture.AllowSimultaneousExecution(RotateGesture);
                ScaleGesture.AllowSimultaneousExecution(RotateGesture);
                if (DoubleTapGesture != null)
                {
                    DoubleTapGesture.AllowSimultaneousExecution(ScaleGesture);
                    DoubleTapGesture.AllowSimultaneousExecution(RotateGesture);
                    DoubleTapGesture.AllowSimultaneousExecution(PanGesture);
                }
            }
            if (Mode == GestureRecognizerComponentScriptBase.GestureObjectMode.RequireIntersectWithGameObject)
            {
                RotateGesture.PlatformSpecificView = gameObject;
                PanGesture.PlatformSpecificView = gameObject;
                ScaleGesture.PlatformSpecificView = gameObject;
                if (DoubleTapGesture != null)
                {
                    DoubleTapGesture.PlatformSpecificView = gameObject;
                }
            }
            FingersScript.Instance.AddGesture(PanGesture);
            FingersScript.Instance.AddGesture(ScaleGesture);
            FingersScript.Instance.AddGesture(RotateGesture);
            FingersScript.Instance.AddGesture(DoubleTapGesture);
        }

        private void OnDisable()
        {
            if (FingersScript.HasInstance)
            {
                FingersScript.Instance.RemoveGesture(PanGesture);
                FingersScript.Instance.RemoveGesture(ScaleGesture);
                FingersScript.Instance.RemoveGesture(RotateGesture);
                FingersScript.Instance.RemoveGesture(DoubleTapGesture);
                DestroyImmediate(FingersScript.Instance.gameObject);//new added ARFaceModuleManager.Instance.m_CanRotateCharacter
            }
        }
    }
}