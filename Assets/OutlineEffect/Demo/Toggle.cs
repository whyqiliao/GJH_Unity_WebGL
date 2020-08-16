using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cakeslice
{
    public class Toggle : MonoBehaviour
    {
        void Start()
        {
            GetComponent<Outline>().enabled =false;
        }
        void OnMouseEnter()
        {
            GetComponent<Outline>().enabled =true;
        }
        void OnMouseExit()
        {
            GetComponent<Outline>().enabled =false;
        }
    }
}