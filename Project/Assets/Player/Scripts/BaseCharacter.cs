﻿//do zrobienia
using UnityEngine;
using System.Collections;
using System;

public class BaseCharacter : MonoBehaviour {

	private string _name;
	public int maxHP=100;
	public int currHP=100;
	public float stamina=100;
	public float maxStamina=100;
	public string playerName = "just me";

	public long secondsAtGameStart = 0L;
	public bool startTimeIsSet = false;

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
