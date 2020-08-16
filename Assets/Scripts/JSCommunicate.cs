using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class JSCommunicate : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void ClosePage();
    public void HandleClosePage()
    {
        ClosePage();
    }
}
