//do zrobienia
using UnityEngine;
using System.Collections;
using System;

public class BaseCharacter : MonoBehaviour {

	private string _name;
	public int maxHP=100;
	public int currHP=100;
	public float stamina=100;
	public float maxStamina=100;
	public int kills = 0;

	public int deaths = 0;
	private bool respawned = true;

	public string playerName = "just me";

	public long secondsAtGameStart = 0L;
	public bool startTimeIsSet = false;

	public string team = "steams";

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if(currHP <= 0 && respawned )// ???
		{
			deaths++;
			respawned = false;
		}
	
	}
}
