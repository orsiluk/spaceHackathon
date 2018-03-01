using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MouseController : MonoBehaviour
{

	public MarsRover Rover;
	public LineRenderer PathLine;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Fire1"))
		{
			StartCoroutine(buildPath());
		}
	}



	Vector3 createPathPoint()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		Physics.Raycast(ray, out hit);
		if (hit.transform.gameObject.layer == 9)
		{
			return hit.point;
		}
		else
		{
			throw new Exception();
		}
	}

	IEnumerator buildPath()
	{
		List<Vector3> currentPath = new List<Vector3>();
		while (Input.GetButton("Fire1"))
		{
			try
			{
				Vector3 point = createPathPoint();
				currentPath.Add(point + Vector3.up * 0.1f);
				PathLine.positionCount = currentPath.Count;
				PathLine.SetPositions(currentPath.ToArray());
			}
			catch (Exception e)
			{
				
			}
			yield return new WaitForSeconds(0.2f);
		}
		Rover.setPath(currentPath);
	}
	
}
