﻿using UnityEngine;
using System.Collections;

/*
 * A very simple Third Person Camera
 */
public class NACameraThirdPerson : NACamera 
{
	//public float 	distance = 0f;
	public float 	smoothing = 0f;
	public Camera 	camera;
	public Vector3 	offset 	= new Vector3(0,0,-5);
	public bool 	local 	= true; 	//local or world positionning
	Vector3 		pos;
	Quaternion 		rot;

	
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (camera != null)
		{
			pos = camera.transform.position;
			rot = camera.transform.rotation;
		}
	}

	void LateUpdate()
	{
		//on fait un orbit au lieu de faire un lookat
		if (camera != null)
		{
			float k = smoothing;
			//Vector3 targetpos = transform.position + transform.forward * -distance;
			Vector3 targetpos = transform.position;
			if (local)
			{
				Quaternion q = Quaternion.FromToRotation(Vector3.forward,transform.forward);
				targetpos += (q*offset);
			}
			else
			{
				targetpos += offset;
			}
			camera.transform.position = pos * k + targetpos * (1-k);

			//Quaternion targetrotation = transform.rotation;
			//float angle = Quaternion.Angle(targetrotation, rot);
			//camera.transform.rotation = Quaternion.Lerp(rot, targetrotation, (1-k));
			camera.transform.LookAt(transform.position);
		}
	}
}
