using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MarsRover : MonoBehaviour
{

	public NavMeshAgent agent;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void MoveTo(Vector3 target)
	{
		agent.destination = target;
	}

	public void setPath(List<Vector3> path)
	{
		StartCoroutine(FollowPath(path));
	}
	
	IEnumerator FollowPath(List<Vector3> path)
	{
		agent.isStopped = false;
		foreach (var point in path)
		{
			Debug.Log("Moving to next position");
			MoveTo(point);
			while (Vector3.Distance(transform.position, point) > 2)
			{
				yield return new WaitForSeconds(0.1f);
			}
		}
		Debug.Log("Finished path");
		agent.isStopped = true;
	}
}
