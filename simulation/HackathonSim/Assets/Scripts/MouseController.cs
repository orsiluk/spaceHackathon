using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity;
public class MouseController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Fire1"))
		{
			mouseClick();
		}
	}



	void mouseClick()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit))
			GameObject.CreatePrimitive(PrimitiveType.Sphere).transform.position = hit.point;
	}
}
