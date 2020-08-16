using UnityEngine;
using System.Collections;

public class bl_OrbitTouch : MonoBehaviour
{

     /// <summary>
    /// you need this for mobile only, so if you use it for mobile set as false
    /// </summary>
    [Header("Movement")]
    public bl_CameraOrbit m_CameraOrbit;
    public Vector2 MovementMultiplier = new Vector2(1, 1);
    [Header("Pinch Zoom")]
    public bool CancelRotateOnPinch = true;
    [Range(0.01f, 2)]
    public float m_PinchZoomSpeed = 0.5f;

    private Vector2 direction;
    private Vector2 smoothDirection;
    private bool touched;
    private int pointerID;
    private bool Pinched = false;

    void Awake()
    {
        direction = Vector2.zero;
        touched = false;
    }

    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        ControlInput();
    }

    void ControlInput()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch t = Input.touches[i];
            if (t.phase == TouchPhase.Began)
            {
                OnPointerDown(t);
            }
            else if (t.phase == TouchPhase.Moved)
            {
                OnDrag(t);
            }
            else if (t.phase == TouchPhase.Ended)
            {
                OnPointerUp(t);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    public void OnPointerDown(Touch data)
    {
        if (m_CameraOrbit == null)
        {
            Debug.LogWarning("Please assign a camera orbit target");
            return;
        }
        if (!touched)
        {
            touched = true;
            pointerID = data.fingerId;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    public void OnDrag(Touch data)
    {
        if (m_CameraOrbit == null)
        {
            Debug.LogWarning("Please assign a camera orbit target");
            return;
        }

        PinchZoom();

        if (Pinched)
            return;

        if (data.fingerId == pointerID)
        {
            direction = data.deltaPosition.normalized;

            m_CameraOrbit.Horizontal = (direction.x * MovementMultiplier.x);
            m_CameraOrbit.Vertical = (-direction.y * MovementMultiplier.y);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void ReanudeControl()
    {
        m_CameraOrbit.Interact = true;
        m_CameraOrbit.CanRotate = true;
        Pinched = false;
    }

    /// <summary>
    /// 
    /// </summary>
    void PinchZoom()
    {
        // If there are two touches on the device...
        if (Input.touchCount == 2 && Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Moved)
        {
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // Otherwise change the field of view based on the change in distance between the touches.
            m_CameraOrbit.SetStaticZoom(-1*deltaMagnitudeDiff * (m_PinchZoomSpeed / Mathf.PI));
            if (CancelRotateOnPinch)
            {
                CancelInvoke("ReanudeControl");
                m_CameraOrbit.Interact = false;
                m_CameraOrbit.CanRotate = false;
                Invoke("ReanudeControl", 0.2f);
                Pinched = true;
            }
        }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    public void OnPointerUp(Touch data)
    {
        if (m_CameraOrbit == null)
        {
            Debug.LogWarning("Please assign a camera orbit target");
            return;
        }
        if (data.fingerId == pointerID)
        {
            direction = Vector2.zero;
            touched = false;
        }
    }
}