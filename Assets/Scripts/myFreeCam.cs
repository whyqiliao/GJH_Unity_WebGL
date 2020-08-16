using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class myFreeCam : MonoBehaviour
{
	[Header("镜头加速乘数")]
    [Tooltip("按下左shift后镜头加快")]
	public float fastMoveFactor = 2;
	[Header("镜头变慢乘数")]
    [Tooltip("按下左ctrl后镜头变慢")]
	public float slowMoveFactor = 0.25f;
	[Header("镜头移动速度")]
    [Tooltip("镜头的默认移动速度")]
	public float speed = 20f;
	float lookSensitivity = 90;
	float rotationX;
	float rotationY;
	CharacterController Ctrlor;
	private float SlowSpeed;
	private float FastSpeed;
	void Start () 
	{
		//Cursor.lockState = CursorLockMode.Confined;
		Ctrlor=gameObject.AddComponent<CharacterController>();
		Ctrlor.stepOffset=0.1f;
		Ctrlor.radius=0.3f;
		Ctrlor.height=0.5f;
		FastSpeed=fastMoveFactor*speed;
		SlowSpeed=slowMoveFactor*speed;
	}
	// Update is called once per frame
	void Update () 
	{
#region Movement
		//mouseinput
		if(!Input.GetMouseButtonDown(1) && Input.GetMouseButton(1))
		{
			rotationX += Input.GetAxis("Mouse X") * lookSensitivity * Time.deltaTime;
			rotationY += Input.GetAxis("Mouse Y") * lookSensitivity * Time.deltaTime;
			rotationY = Mathf.Clamp (rotationY, -90, 90);
		}
		transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
		transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);

		//fast
		if(Input.GetKey(KeyCode.LeftShift)){
			speed=FastSpeed;
		}
		//slow
		if(Input.GetKey(KeyCode.LeftControl)){
			speed=SlowSpeed;
		}
		//w
		if (Input.GetKey (KeyCode.W)) {
                Vector3 forward = transform.TransformDirection(Vector3.forward);
                Ctrlor.Move(forward*speed*Time.deltaTime);

        }
		//a
		if(Input.GetKey (KeyCode.A)) {
                Vector3 left = transform.TransformDirection(Vector3.left);
                Ctrlor.Move(left* speed * Time.deltaTime);
        }
		//s
		if(Input.GetKey (KeyCode.S)) {
                Vector3 back = transform.TransformDirection(Vector3.back);
                Ctrlor.Move(back* speed * Time.deltaTime);
        }
		//d
		if(Input.GetKey (KeyCode.D)) {
                Vector3 right = transform.TransformDirection(Vector3.right);
                Ctrlor.Move(right* speed * Time.deltaTime);
        }
		//up
		if(Input.GetKey (KeyCode.E)) {
                Vector3 up = transform.TransformDirection(Vector3.up);
                Ctrlor.Move(up* speed * Time.deltaTime);
        }
		//down
		if(Input.GetKey (KeyCode.Q)) {
                Vector3 down = transform.TransformDirection(Vector3.down);
                Ctrlor.Move(down* speed * Time.deltaTime);
		}
	}
#endregion
}
