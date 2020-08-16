using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Serialization;

[RequireComponent(typeof(Image))]
public class bl_OrbitTouchPad : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    /// <summary>
    /// you need this for mobile only, so if you use it for mobile set as false
    /// </summary>
    [Header("Movement")]
    [SerializeField]
    private bool OverrideEditor = true;
    [FormerlySerializedAs("m_CameraOrbit")]
    public bl_CameraOrbit cameraOrbit;
    [SerializeField]
    private Vector2 MovementMultiplier = new Vector2(1, 1);
    [Header("Pinch Zoom")]
    public bool CancelRotateOnPinch = true;
    [SerializeField, Range(0.01f, 2)]
    private float m_PinchZoomSpeed = 0.5f;

    private Vector2 origin;
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
    /// <param name="data"></param>
    public void OnPointerDown(PointerEventData data)
    {
        if (cameraOrbit == null)
        {
            Debug.LogWarning("Please assign a camera orbit target");
            return;
        }
        if (!touched)
        {
            touched = true;
            pointerID = data.pointerId;
            origin = data.position;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    public void OnDrag(PointerEventData data)
    {
        if (cameraOrbit == null)
        {
            Debug.LogWarning("Please assign a camera orbit target");
            return;
        }

        PinchZoom(data);

        if (Pinched)
            return;

        if (!OverrideEditor)//set false when you want use for mobile
        {
            if (data.pointerId == pointerID)
            {
                Vector2 currentPosition = data.position;
                Vector2 directionRaw = currentPosition - origin;
                direction = directionRaw.normalized;

                cameraOrbit.Horizontal = (direction.x * MovementMultiplier.x);
                cameraOrbit.Vertical = (-direction.y * MovementMultiplier.y);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void ReanudeControl()
    {
        cameraOrbit.Interact = true;
        cameraOrbit.CanRotate = true;
        Pinched = false;
    }

    /// <summary>
    /// 
    /// </summary>
    void PinchZoom(PointerEventData data)
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
            cameraOrbit.SetStaticZoom(deltaMagnitudeDiff * m_PinchZoomSpeed);
            if (CancelRotateOnPinch)
            {
                CancelInvoke("ReanudeControl");
                cameraOrbit.Interact = false;
                cameraOrbit.CanRotate = false;
                Invoke("ReanudeControl", 0.2f);
                Pinched = true;
            }
        }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    public void OnPointerUp(PointerEventData data)
    {
        if (cameraOrbit == null)
        {
            Debug.LogWarning("Please assign a camera orbit target");
            return;
        }
        if (data.pointerId == pointerID)
        {
            direction = Vector2.zero;
            touched = false;
        }
    }
}