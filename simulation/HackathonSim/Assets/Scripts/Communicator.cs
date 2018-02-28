using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Communicator : MonoBehaviour {
	private static readonly string robo_api = "some_api";
	private int counter = 0;

	public void sendCommand(float distance, float angle)
	{
		RobotCommand command = new RobotCommand();
		command.Id = counter;
		counter++;
		command.Command = String.Format("{0} {1}", angle, distance);
		string json = JsonUtility.ToJson(command);
		postJSON(json);
	}

	void postJSON(String jsonString)
	{
		WWW www;
		Hashtable postHeader = new Hashtable();
		postHeader.Add("Content-Type", "application/json");

		// convert json string to byte
		var formData = System.Text.Encoding.UTF8.GetBytes(jsonString);

		www = new WWW(robo_api, formData, postHeader);
		StartCoroutine(WaitForReply(www));
	}

	IEnumerator WaitForReply(WWW data)
	{
		yield return data;
		print(data);
	}
}
