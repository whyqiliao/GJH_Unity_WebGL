using UnityEngine;
using System.Runtime.InteropServices;
public class PlatfromRecognizer : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern bool IsMobile();
    bool ismobile;

    [Header("Reference")]
    public bl_CameraOrbit m_CameraOrbit;

    void Start() {
        ismobile=IsMobile();
        m_CameraOrbit.SetIsMobile(ismobile);
    }
}
