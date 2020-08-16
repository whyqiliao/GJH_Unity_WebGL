using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Lovatto.OrbitCamera;

[ExecuteInEditMode]
public class bl_CameraOrbit : bl_CameraBase
{
    [HideInInspector]
    public bool m_Interact = true;

    [Header("Target")]
    public Transform Target;
    public Vector3 TargetOffset;
    [Header("Settings")]
    public bool isForMobile = false;
    public bool executeInEditMode = true;
    public float Distance = 5f;
    [Range(0.01f, 5)]public float SwichtSpeed = 2;
    public Vector2 DistanceClamp = new Vector2(1.5f, 5);
    public bool ClampVertical = false;
    public bool ClampHorizontal = false;
    public Vector2 YLimitClamp = new Vector2(-20, 80);
    public Vector2 XLimitClamp = new Vector2(360, 360); //Clamp the horizontal angle from the start position (max left = x, max right = y) >= 360 = not limit
    public Vector2 SpeedAxis = new Vector2(100, 100);
    public bool LockCursorOnRotate = true;
    [Header("Input")]
    public bool RequiredInput = true;
    public CameraMouseInputType RotateInputKey = CameraMouseInputType.LeftAndRight;
    [Range(0.001f, 0.07f)]
    public float InputMultiplier = 0.02f;
    [Range(0.1f, 15)]
    public float InputLerp = 7;
    public bool useKeys = false;
    [Header("Movement")]
    public CameraMovementType MovementType = CameraMovementType.Normal;
    public bool useZoomOutEffect = false;
    [Range(-90, 90)]
    public float TouchZoomAmount = -5;
    [Range(0.1f, 20)]
    public float LerpSpeed = 7;
    [Range(1f, 100)]
    public float OutInputSpeed = 20;
    [Header("Fog")]
    [Range(5, 179)]
    public bool useStartZoomEffect = false;
    public float StartFov = 179;
    [Range(0.1f, 15)]
    public float FovLerp = 7;
    [Range(0.0f, 7)]
    public float DelayStartFoV = 1.2f;
    [Range(1, 10)]
    public float ScrollSensitivity = 5;
    [Range(1, 25)]
    public float ZoomSpeed = 7;
    [Header("Auto Rotation")]
    public bool AutoRotate = true;
    public CameraAutoRotationType AutoRotationType = CameraAutoRotationType.Dinamicaly;
    [Range(0, 20)]
    public float AutoRotSpeed = 5;
    [Header("Collision")]
    public bool DetectCollision = true;
    public bool TeleporOnHit = true;
    [Range(0.01f, 4)]
    public float CollisionRadius = 2;
    public LayerMask DetectCollisionLayers;
    [Header("Fade")]
    public bool FadeOnStart = true;
    [Range(0.01f, 5)]public float FadeSpeed = 2;
    public Color FadeColor = Color.black;

    private Texture2D FadeTexture = null;
    private float y;
    private float x;
    private Ray Ray;
    private bool LastHaveInput = false;
    private float distance = 0;
    private float currentFog = 60;
    private float defaultFog;
    float horizontal;
    float vertical;
    private float defaultAutoSpeed;
    private float lastHorizontal;
    private bool canFogControl = false;
    private bool haveHit = false;
    private float LastDistance;
    private bool m_CanRotate = true;
    private Vector3 ZoomVector;
    private Quaternion CurrentRotation;
    private Vector3 CurrentPosition;
    private float FadeAlpha = 1;
    private bool isSwitchingTarget = false;
    private bool isDetectingHit = false;
    private float initXRotation;
    private Camera cameraComponent;
    public Camera CameraComponent { get { if (cameraComponent == null) { cameraComponent = GetComponent<Camera>(); } return cameraComponent; } }

    public void SetIsMobile(bool isMobile)
    {
        isForMobile=isMobile;
        Debug.Log(isForMobile);
    }
    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        if (!Application.isPlaying) return;
        SetUp();
    }

    /// <summary>
    /// 
    /// </summary>
    void SetUp()
    {
        //SetUp default position for camera
        //For avoid the effect of 'teleportation' in the first movement
        distance = Vector3.Distance(transform.position, TargetPosition);
        Distance = distance;
        Vector3 eulerAngles = Transform.eulerAngles;
        x = eulerAngles.y;
        y = eulerAngles.x;
        initXRotation = eulerAngles.y;
        horizontal = x;
        vertical = y;


        FadeTexture = Texture2D.whiteTexture;
        currentFog = GetCamera.fieldOfView;
        defaultFog = currentFog;
        if (useStartZoomEffect)
        {
            GetCamera.fieldOfView = StartFov;
            defaultAutoSpeed = AutoRotSpeed;
            StartCoroutine(IEDelayFog());
        }
        if (RotateInputKey == CameraMouseInputType.MobileTouch && FindObjectOfType<bl_OrbitTouchPad>() == null)
        {
            Debug.LogWarning("For use  mobile touched be sure to put the 'OrbitTouchArea in the canvas of scene");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void LateUpdate()
    {
        if (!Application.isPlaying)
        {
            if (!executeInEditMode) return;

#if UNITY_EDITOR
            OnEditorUpdate();
#endif
            return;
        }

        if (Target == null)
        {
            Debug.LogWarning("Target is not assigned to orbit camera!", this);
            return;
        }
        if (isSwitchingTarget)
            return;

        if (CanRotate)
        {
            //Calculate the distance of camera
            ZoomControll(false);

            //Control rotation of camera
            OrbitControll();

            //Auto rotate the camera when key is not pressed.
            if (AutoRotate && !isInputKeyRotate && Application.isPlaying) { AutoRotation(); }
        }
        else
        {
            //Calculate the distance of camera
            ZoomControll(true);
        }

        //When can't interact with inputs not need continue here.
        if (!m_Interact || !Application.isPlaying)
            return;

        //Control fog effect in camera.
        FogControl();

        //Control all input for apply to the rotation.
        InputControl();
    }

    /// <summary>
    /// 
    /// </summary>
    void InputControl()
    {
        if (LockCursorOnRotate && !useKeys)
        {
            if (!isForMobile)
            {
                if (!isInputKeyRotate && LastHaveInput)
                {
                    if (LockCursorOnRotate && Interact) { bl_CameraUtils.LockCursor(false); }
                    LastHaveInput = false;
                    if (lastHorizontal >= 0) { AutoRotSpeed = OutInputSpeed; } else { AutoRotSpeed = -OutInputSpeed; }
                }
                if (isInputKeyRotate && !LastHaveInput)
                {
                    if (LockCursorOnRotate && Interact) { bl_CameraUtils.LockCursor(true); }
                    LastHaveInput = true;
                }
            }  
        }

        if (isInputUpKeyRotate && useZoomOutEffect)
        {
            currentFog -= TouchZoomAmount;
        }
    }

    /// <summary>
    /// Rotate auto when any key is pressed.
    /// </summary>
    void AutoRotation()
    {
        switch (AutoRotationType)
        {
            case CameraAutoRotationType.Dinamicaly:
                AutoRotSpeed = (lastHorizontal > 0) ? Mathf.Lerp(AutoRotSpeed, defaultAutoSpeed, Time.deltaTime / 2) : Mathf.Lerp(AutoRotSpeed, -defaultAutoSpeed, Time.deltaTime / 2);
                break;
            case CameraAutoRotationType.Left:
                AutoRotSpeed = Mathf.Lerp(AutoRotSpeed, defaultAutoSpeed, Time.deltaTime / 2);
                break;
            case CameraAutoRotationType.Right:
                AutoRotSpeed = Mathf.Lerp(AutoRotSpeed, -defaultAutoSpeed, Time.deltaTime / 2);
                break;

        }
        horizontal += Time.deltaTime * AutoRotSpeed;
    }

    /// <summary>
    /// 
    /// </summary>
    void FogControl()
    {
        if (!canFogControl)
            return;

        //Control the 'puw' effect of fog camera.
        currentFog = Mathf.SmoothStep(currentFog, defaultFog, Time.deltaTime * FovLerp);
        //smooth transition with lerp
        GetCamera.fieldOfView = Mathf.Lerp(GetCamera.fieldOfView, currentFog, Time.deltaTime * FovLerp);
    }


    /// <summary>
    /// 
    /// </summary>
    void OrbitControll()
    {
        if (m_Interact)
        {
            if (!isForMobile)
            {
                if (RequiredInput && !useKeys && isInputKeyRotate || !RequiredInput)
                {
                    horizontal += ((SpeedAxis.x) * InputMultiplier) * AxisX;
                    vertical -= (SpeedAxis.y * InputMultiplier) * AxisY;
                    lastHorizontal = AxisX;
                }
                else if (useKeys)
                {
                    horizontal -= ((KeyAxisX * SpeedAxis.x) ) * InputMultiplier;
                    vertical += (KeyAxisY * SpeedAxis.y) * InputMultiplier;
                    lastHorizontal = KeyAxisX;
                }
            }
        }

        if (ClampVertical)
        {
            //clamp 'vertical' angle
            vertical = bl_CameraUtils.ClampAngle(vertical, YLimitClamp.x, YLimitClamp.y);
        }
        if (ClampHorizontal)
        {
            if (XLimitClamp.x < 360 && XLimitClamp.y < 360)
            {
                horizontal = bl_CameraUtils.ClampAngle(horizontal, (initXRotation - XLimitClamp.y), (XLimitClamp.x + initXRotation));
            }
        }
        //smooth movement of responsiveness input.
        x = Mathf.Lerp(x, horizontal, Time.deltaTime * InputLerp);
        y = Mathf.Lerp(y, vertical, Time.deltaTime * InputLerp);
        if (ClampVertical)
        {
            //clamp 'y' angle
            y = bl_CameraUtils.ClampAngle(y, YLimitClamp.x, YLimitClamp.y);
        }

        //convert vector to quaternion for apply to rotation
        CurrentRotation = Quaternion.Euler(y, x, 0f);

        //calculate the position and clamp on a circle
        CurrentPosition = (CurrentRotation * ZoomVector) + TargetPosition;

        //switch in the movement select
        switch (MovementType)
        {
            case CameraMovementType.Dynamic:
                Transform.position = Vector3.Lerp(Transform.position, CurrentPosition, (LerpSpeed) * Time.deltaTime);
                Transform.rotation = Quaternion.Lerp(Transform.rotation, CurrentRotation, (LerpSpeed * 2) * Time.deltaTime);
                break;
            case CameraMovementType.Normal:
                Transform.rotation = CurrentRotation;
                Transform.position = CurrentPosition;
                break;
            case CameraMovementType.Towars:
                Transform.rotation = Quaternion.RotateTowards(Transform.rotation, CurrentRotation, (LerpSpeed));
                Transform.position = Vector3.MoveTowards(Transform.position, CurrentPosition, (LerpSpeed));
                break;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void ZoomControll(bool autoApply)
    {
        bool isHit = false;
        float delta = Time.deltaTime;
        //clamp distance and check this.
        distance = Mathf.Clamp(distance - (MouseScrollWheel * ScrollSensitivity), DistanceClamp.x, DistanceClamp.y);
        //Collision detector with a simple raycast
        if (DetectCollision)
        {
            //Calculate direction from target
            Vector3 forward = Transform.position - TargetPosition;
            //create a ray from transform to target
            Ray = new Ray(TargetPosition, forward.normalized);
            RaycastHit hit;
            //if ray detect a an obstacle in between the point of origin and the target
            if (Physics.SphereCast(Ray.origin, CollisionRadius, Ray.direction, out hit, distance, DetectCollisionLayers))
            {
                if (!haveHit) { LastDistance = distance; haveHit = true; }
                distance = Mathf.Clamp(hit.distance, DistanceClamp.x, DistanceClamp.y);
                if (TeleporOnHit) { Distance = distance; }
                isHit = true;
            }
            else
            {
                if (!isDetectingHit) { StartCoroutine(DetectHit()); }
            }
            distance = (distance < 1) ? 1 : distance;// distance is recommendable never is least than 1
            if (!haveHit || !TeleporOnHit)
            {
                float s = (isHit) ? Mathf.PI : 1;
                Distance = Mathf.SmoothStep(Distance, distance, delta * (ZoomSpeed * s));
            }
        }
        else
        {
            distance = (distance < 1) ? 1 : distance;// distance is recommendable never is least than 1
            Distance = Mathf.SmoothStep(Distance, distance, delta * ZoomSpeed);
        }

        //apply distance to vector depth z
        ZoomVector = new Vector3(0f, 0f, -this.Distance);

        if (autoApply)
        {
            //calculate the position and clamp on a circle
            CurrentPosition = ((CurrentRotation * ZoomVector)) + TargetPosition;

            //switch in the movement select
            switch (MovementType)
            {
                case CameraMovementType.Dynamic:
                    Transform.position = Vector3.Lerp(Transform.position, CurrentPosition, (LerpSpeed) * delta);
                    Transform.rotation = Quaternion.Lerp(Transform.rotation, CurrentRotation, (LerpSpeed * 2) * delta);
                    break;
                case CameraMovementType.Normal:
                    Transform.rotation = CurrentRotation;
                    Transform.position = CurrentPosition;
                    break;
                case CameraMovementType.Towars:
                    Transform.rotation = Quaternion.RotateTowards(Transform.rotation, CurrentRotation, (LerpSpeed));
                    Transform.position = Vector3.MoveTowards(Transform.position, CurrentPosition, (LerpSpeed));
                    break;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private bool isInputKeyRotate
    {
        get
        {
            switch (RotateInputKey)
            {
                case CameraMouseInputType.All:
                    return (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse1) || Input.GetKey(KeyCode.Mouse2) || Input.GetMouseButton(0));
                case CameraMouseInputType.LeftAndRight:
                    return (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse1));
                case CameraMouseInputType.LeftMouse:
                    return (Input.GetKey(KeyCode.Mouse0));
                case CameraMouseInputType.RightMouse:
                    return (Input.GetKey(KeyCode.Mouse1));
                case CameraMouseInputType.MouseScroll:
                    return (Input.GetKey(KeyCode.Mouse2));
                case CameraMouseInputType.MobileTouch:
                    return (Input.GetMouseButton(0) || Input.GetMouseButton(1));
                default:
                    return (Input.GetKey(KeyCode.Mouse0));
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void OnGUI()
    {
        if (Application.isPlaying)
        {
            if (isSwitchingTarget)
            {
                GUI.color = new Color(FadeColor.r, FadeColor.g, FadeColor.b, FadeAlpha);
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), FadeTexture, ScaleMode.StretchToFill);
                return;
            }

            if (FadeOnStart && FadeAlpha > 0)
            {
                FadeAlpha -= Time.deltaTime * FadeSpeed;
                GUI.color = new Color(FadeColor.r, FadeColor.g, FadeColor.b, FadeAlpha);
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), FadeTexture, ScaleMode.StretchToFill);
            }
        }

#if UNITY_EDITOR
        EditorGUI();
#endif
    }

#if UNITY_EDITOR
    /// <summary>
    /// 
    /// </summary>
    void EditorGUI()
    {
        if (!Application.isPlaying && Target != null && executeInEditMode)
        {
            Vector3 targetPoint = TargetPosition;
            Plane plane = new Plane(Transform.forward, Transform.position);
            float size = 4;
            //If the object is behind the camera, then don't draw it
            if (plane.GetSide(targetPoint) == false)
            {
                return;
            }

            //Calculate the 2D position of the position where the icon should be drawn
            Vector3 viewportPoint = CameraComponent.WorldToViewportPoint(targetPoint);

            //The viewportPoint coordinates are between 0 and 1, so we have to convert them into screen space here
            Vector2 drawPosition = new Vector2(viewportPoint.x * Screen.width, Screen.height * (1 - viewportPoint.y));

            float clampBorder = 12;

            //Clamp the position to the edge of the screen in case the icon would be drawn outside the screen
            drawPosition.x = Mathf.Clamp(drawPosition.x, clampBorder, Screen.width - clampBorder);
            drawPosition.y = Mathf.Clamp(drawPosition.y, clampBorder, Screen.height - clampBorder);

            GUI.color = Color.yellow;
            GUI.DrawTexture(new Rect(drawPosition.x - size * 0.5f, drawPosition.y - size * 0.5f, size, size), Texture2D.whiteTexture, ScaleMode.StretchToFill);
            GUI.color = Color.white;
        }
    }
#endif

    /// <summary>
    /// 
    /// </summary>
    private bool isInputUpKeyRotate
    {
        get
        {
            switch (RotateInputKey)
            {
                case CameraMouseInputType.All:
                    return (Input.GetKeyUp(KeyCode.Mouse0) || Input.GetKeyUp(KeyCode.Mouse1) || Input.GetKeyUp(KeyCode.Mouse2) || Input.GetMouseButtonUp(0));
                case CameraMouseInputType.LeftAndRight:
                    return (Input.GetKeyUp(KeyCode.Mouse0) || Input.GetKeyUp(KeyCode.Mouse1));
                case CameraMouseInputType.LeftMouse:
                    return (Input.GetKeyUp(KeyCode.Mouse0) || Input.GetMouseButtonUp(0));
                case CameraMouseInputType.RightMouse:
                    return (Input.GetKeyUp(KeyCode.Mouse1));
                case CameraMouseInputType.MouseScroll:
                    return (Input.GetKeyUp(KeyCode.Mouse2));
                case CameraMouseInputType.MobileTouch:
                    return (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1));
                default:
                    return (Input.GetKey(KeyCode.Mouse0) || Input.GetMouseButton(0));
            }
        }
    }

    /// <summary>
    /// Call this function for change the target to orbit
    /// the change will be by a smooth fade effect
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        StopCoroutine("TranslateTarget");
        StartCoroutine("TranslateTarget", newTarget);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="side">axis position</param>
    public void SetViewPoint(int side)
    {
        AutoRotate = false;
        if (side == 0)//top
        {
            vertical = 90;
            horizontal = 0;
        }
        else if(side == 1)//front
        {
            vertical = 0;
            horizontal = 0;
        }
        else if (side == 2)//right
        {
            vertical = 0;
            horizontal = -90;
        }
        else if (side == 3)//left
        {
            vertical = 0;
            horizontal = 90;
        }
        else if (side == 4)//back
        {
            vertical = 0;
            horizontal = 180;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    IEnumerator TranslateTarget(Transform newTarget)
    {
        isSwitchingTarget = true;
        while (FadeAlpha < 1)
        {
            Transform.position = Vector3.Lerp(Transform.position, Transform.position + new Vector3(0, 2, -2), Time.deltaTime );
            FadeAlpha += Time.smoothDeltaTime * SwichtSpeed;
            yield return null;
        }
        Target = newTarget;
        isSwitchingTarget = false;
        while (FadeAlpha > 0)
        {
            FadeAlpha -= Time.smoothDeltaTime * SwichtSpeed;
            yield return null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator DetectHit()
    {
        isDetectingHit = true;
        yield return new WaitForSeconds(0.4f);
        if (haveHit) { distance = LastDistance; haveHit = false; }
        isDetectingHit = false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator IEDelayFog()
    {
        yield return new WaitForSeconds(DelayStartFoV);
        canFogControl = true;
    }

    public Vector3 TargetPosition
    {
        get { if(Target != null) { return Target.position + TargetOffset; } else { return Vector3.zero; } }
    }

    /// <summary>
    /// 
    /// </summary>
    public float Horizontal
    {
        get
        {
            return horizontal;
        }
        set
        {
            horizontal += value;
            lastHorizontal = horizontal;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public float Vertical
    {
        get
        {
            return vertical;
        }
        set
        {
            vertical += value;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool Interact
    {
        get
        {
            return m_Interact;
        }
        set
        {
            m_Interact = value;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool CanRotate
    {
        get
        {
            return m_CanRotate;
        }
        set
        {
            m_CanRotate = value;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public float AutoRotationSpeed
    {
        get
        {
            return defaultAutoSpeed;
        }
        set
        {
            defaultAutoSpeed = value;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void SetZoom(float value)
    {
        distance += (-(value * 0.5f) * ScrollSensitivity);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void SetStaticZoom(float value)
    {
        distance += value;
    }

#if UNITY_EDITOR
    /// <summary>
    /// 
    /// </summary>
    void OnEditorUpdate()
    {
        if (Target == null) return;
        Vector3 relativePos = TargetPosition - Transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);

        vertical = rotation.eulerAngles.y;
        horizontal = rotation.eulerAngles.x;

        distance = Vector3.Distance(transform.position, TargetPosition);
        distance = Mathf.Clamp(distance, DistanceClamp.x, DistanceClamp.y);
        Distance = distance;
        ZoomVector = new Vector3(0f, 0f, -distance);

        if (XLimitClamp.x < 360 && XLimitClamp.y < 360)
        {
            vertical = bl_CameraUtils.ClampAngle(vertical, XLimitClamp.y, XLimitClamp.x);
        }
        horizontal = bl_CameraUtils.ClampAngle(horizontal, YLimitClamp.x, YLimitClamp.y);

        CurrentRotation = Quaternion.Euler(horizontal, vertical, rotation.eulerAngles.z);
        CurrentPosition = (CurrentRotation * ZoomVector) + TargetPosition;
        Transform.position = CurrentPosition;
        Transform.rotation = CurrentRotation;
    }
    /// <summary>
    /// 
    /// </summary>
    void OnDrawGizmosSelected()
    {
        // Gizmos.color = new Color32(0, 221, 221, 255);
        Gizmos.color = Color.green;
        if (Target != null)
        {
            Gizmos.DrawLine(transform.position, TargetPosition);
            Gizmos.matrix = Matrix4x4.TRS(TargetPosition, transform.rotation, new Vector3(1f, 0, 1f));
            Gizmos.DrawWireSphere(TargetPosition, Distance);
            Gizmos.matrix = Matrix4x4.identity;
        }
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, new Vector3(1f, 1, 1f));
        Gizmos.DrawCube(Vector3.zero, new Vector3(0.2f, 0.2f, 1));
        Gizmos.DrawCube(Vector3.zero, Vector3.one * 0.5f);
        Gizmos.matrix = Matrix4x4.identity;
    }

    public Dictionary<string, bool> editorValues = new Dictionary<string, bool>() { { "input", true }, { "move", true }, { "zoom", true }, { "collision", true }, { "fade", true } };

#endif
}