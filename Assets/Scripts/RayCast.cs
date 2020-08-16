using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCast : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray=Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit=new RaycastHit();
            if(Physics.Raycast(ray,out raycastHit))
            {
                if(raycastHit.collider.gameObject!=null)
                {
                    Debug.Log(raycastHit.collider.gameObject.name);
                }
            }
        }
    }
}
