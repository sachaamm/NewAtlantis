﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LogEntry
{
	public string str = "";
	public int category = 0;
}
public class LogManager : MonoBehaviour 
{
	public static bool bGUI = false;
	public static Texture2D texWhite = null;
	public static List<LogEntry> logs = new List<LogEntry>();

	public static float timer = 0f;
	// Use this for initialization
	void Start () 
	{
		texWhite = Resources.Load ("white") as Texture2D;
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		timer+=Time.deltaTime;
		if (Input.GetKeyDown(KeyCode.L))
		{
			bGUI = !bGUI;
		}
	
	}

	void OnGUI()
	{
		if (bGUI)
		{
			GUI.color = new Color(0,0,0,0.5f);
			GUI.DrawTexture(new Rect(0,100,Screen.width, Screen.height-200), texWhite);
			GUI.color = Color.white;
			int count = (Screen.height-250)/12;
			int start = logs.Count-count;
			int end = logs.Count-1;
			start = Mathf.Max (start, 0);

			float y = 100;
			if (GUI.Button (new Rect(0,y,100,30), "clear"))
			{
				logs.Clear();
			}
			y = 150;
			if (logs.Count == 0)
				return;
			for (int i=start;i<=end;++i)
			{
				LogEntry e = logs[i];
				if (e.category == 0)
					GUI.color = Color.white;
				else if (e.category == 1)
					GUI.color = Color.yellow;
				else if (e.category == 2)
					GUI.color = Color.red;
				GUI.Label(new Rect(0, y, Screen.width, 30), e.str);
				y += 12;
			}
		}
		else
		{
			//version minimisée, on affiche juste le dernier message si dans le timer
			if (logs.Count > 0 && timer < 2f)
			{
				int end = logs.Count-1;
				LogEntry e = logs[end];
				if (e.category == 0)
					GUI.color = new Color(0,0,0,0.5f);
				else if (e.category == 1)
					GUI.color = new Color(1,1,0,0.5f);
				else if (e.category == 2)
					GUI.color = Color.red;

				GUI.DrawTexture(new Rect(0,Screen.height-30,Screen.width, 30), texWhite);
				GUI.color = Color.white;
				GUI.Label(new Rect(0,Screen.height-30,Screen.width, 30), e.str);
			}
		}
	}


	public static void Log(string str)
	{
		//Debug.Log (str);
		LogEntry e = new LogEntry();
		e.category = 0;
		e.str = str;
		logs.Add (e);
		timer = 0f;
	}

	public static void LogWarning(string str)
	{
		Debug.LogWarning (str);
		LogEntry e = new LogEntry();
		e.category = 1;
		e.str = str;
        logs.Add (e);
		timer = 0f;
    }
	public static void LogError(string str)
	{
		Debug.LogError (str);
		LogEntry e = new LogEntry();
		e.category = 2;
		e.str = str;
        logs.Add (e);
		timer = 0f;
    }
}
