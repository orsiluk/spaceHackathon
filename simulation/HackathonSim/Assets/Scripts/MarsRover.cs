using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class MarsRover : MonoBehaviour
{

	public NavMeshAgent agent;
	public ParticleSystem flamethrower;

	public Communicator communicator;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKey("space"))
		{
			flamethrower.gameObject.active = true;
		}
		else
		{
			flamethrower.gameObject.active = false;
		}
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
			sendMovement(point);
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
	
	void sendMovement(Vector3 point)
	{
		float distance = Vector3.Distance(transform.position, point);
		Vector3 direction = point - transform.position;
		float turnAngle = Vector3.Angle(direction, transform.forward);
		communicator.sendTurn( turnAngle);
		communicator.sendMove(distance);
	}
}
