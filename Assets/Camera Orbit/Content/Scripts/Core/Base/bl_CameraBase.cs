using UnityEngine;

public class bl_CameraBase : MonoBehaviour {

    private Transform m_Transform = null;
    public Transform Transform
    {
        get
        {
            if (m_Transform == null)
            {
                m_Transform = GetComponent<Transform>();
            }
            return m_Transform;
        }
    }

    public float MouseScrollWheel
    {
        get
        {
            return Input.GetAxis("Mouse ScrollWheel");
        }
    }

    public float AxisY
    {
        get
        {
            return Input.GetAxis("Mouse Y");
        }
    }

    public float AxisX
    {
        get
        {
            return Input.GetAxis("Mouse X");
        }
    }

    public float KeyAxisY
    {
        get
        {
            return Input.GetAxis("Vertical");
        }
    }

    public float KeyAxisX
    {
        get
        {
            return Input.GetAxis("Horizontal");
        }
    }

    private Camera m_Camera = null;
    public Camera GetCamera
    {
        get
        {
            if(m_Camera == null)
            {
                m_Camera = GetComponent<Camera>();
            }
            if(m_Camera == null)
            {
                m_Camera = GetComponentInChildren<Camera>();
            }
            return m_Camera;
        }
    }
}