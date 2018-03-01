using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MarsRover : MonoBehaviour
{

	public NavMeshAgent agent;
	public ParticleSystem flamethrower;
	public MarsMapper mapper;
	public Communicator communicator;
	private Vector3 cameraOffset;
	private float cameraHeight = 0;
	public Camera mainCamera;

	public Text uiText;
	// Use this for initialization
	void Start ()
	{
		StartCoroutine(updateWorld());
		cameraOffset = mainCamera.transform.position - transform.position;
		cameraHeight = mainCamera.transform.position.y;
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

		Vector3 cameraPosition = transform.position + cameraOffset;
		cameraPosition.y = cameraHeight;
		mainCamera.transform.position = cameraPosition;
		Vector2 latlng = getLatLong();
		uiText.text = String.Format("{0},{1}", latlng.x, latlng.y);
	}

	public void MoveTo(Vector3 target)
	{
		agent.destination = target;
	}

	public void setPath(List<Vector3> path)
	{
		StartCoroutine(FollowPath(path));
	}

	IEnumerator updateWorld()
	{
		while (true)
		{
			yield return new WaitForSeconds(1);
			mapper.updateTiles(transform.position);
		}
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

	public Vector2 getLatLong()
	{
		return MarsMapper.MapToLatLng(transform.position.x/10, transform.position.z/10);
	}
}
