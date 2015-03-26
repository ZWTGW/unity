﻿using UnityEngine;
using System.Collections;

public class GrenadeThrow : MonoBehaviour {
	
	public Camera camera;
	public GameObject granadePref;
	private uint grenadesNumber;
	
	// Use this for initialization
	void Start () {
		grenadesNumber = 10005;
	}
	
	// Update is called once per frame
	void Update () {

		}
	public void Throw (){
			if( grenadesNumber > 0 )
			{
				GameObject granade = Instantiate( granadePref ) as GameObject;
				granade.transform.position = camera.transform.position + camera.transform.forward;
				granade.GetComponent("Rigidbody").GetComponent<Rigidbody>().AddForce( camera.transform.forward*8000 );
				--grenadesNumber;
			}
			else{
				Debug.Log( "no granades left!" );
			}

	}
}
