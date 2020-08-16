using UnityEngine;
using UnityEngine.EventSystems;

public class bl_OrbitButton : MonoBehaviour,IPointerDownHandler,IPointerEnterHandler,IPointerExitHandler,IPointerUpHandler
{

    public bl_CameraOrbit CameraOrbit;
    public Axys m_Axi = Axys.Horizontal;
    [Range(-15,15)] public float RotationAmount = 5;
    public bool Maintain = false;

    private bool isMaitain = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (CameraOrbit == null)
        {
            Debug.LogWarning("Please assign a camera orbit target");
            return;
        }
        if (m_Axi == Axys.Horizontal)
        {
            CameraOrbit.Horizontal = RotationAmount;
        }
        else if (m_Axi == Axys.Vertical)
        {
            CameraOrbit.Vertical = RotationAmount;
        }
        isMaitain = true;
    }

    void Update()
    {
        if (Maintain)
        {
            if (isMaitain)
            {
                if (m_Axi == Axys.Horizontal)
                {
                    CameraOrbit.Horizontal = (RotationAmount / 5);
                }
                else if (m_Axi == Axys.Vertical)
                {
                    CameraOrbit.Vertical = (RotationAmount / 5);
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (CameraOrbit == null)
        {
            Debug.LogWarning("Please assign a camera orbit target");
            return;
        }
        CameraOrbit.Interact = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (CameraOrbit == null)
        {
            Debug.LogWarning("Please assign a camera orbit target");
            return;
        }
        CameraOrbit.Interact = true;
        isMaitain = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (CameraOrbit == null)
        {
            Debug.LogWarning("Please assign a camera orbit target");
            return;
        }
        CameraOrbit.Interact = true;
        isMaitain = false;
    }

    [System.Serializable]
    public enum Axys
    {
        Horizontal,
        Vertical,
    }
}