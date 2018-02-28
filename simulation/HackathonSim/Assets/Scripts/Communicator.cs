﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Communicator : MonoBehaviour {
	private static readonly string robo_api = "http://9fe64d44.ngrok.io/api/Commands";
	private int counter = 0;

	public void sendMove(float distance)
	{
		RobotCommand command = new RobotCommand();
		command.Id = counter;
		counter++;
		command.Data = String.Format("{0}", distance);
		command.CommandType = "Forward";
		string json = JsonUtility.ToJson(command);
		postJSON(json);
	}
	
	public void sendTurn(float angle)
	{
		RobotCommand command = new RobotCommand();
		command.Id = counter;
		counter++;
		command.Data = String.Format("{0}", angle);
		command.CommandType = "Turn";
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
		Debug.Log(data.error);
	}
}
