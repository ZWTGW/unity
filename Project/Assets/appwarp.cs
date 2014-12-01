using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using com.shephertz.app42.gaming.multiplayer.client;
using com.shephertz.app42.gaming.multiplayer.client.events;
using com.shephertz.app42.gaming.multiplayer.client.listener;
using com.shephertz.app42.gaming.multiplayer.client.command;
using com.shephertz.app42.gaming.multiplayer.client.message;
using com.shephertz.app42.gaming.multiplayer.client.transformer;

//using UnityEditor;

using AssemblyCSharp;

public class appwarp : MonoBehaviour {

	//please update with values you get after signing up
	public static string apiKey = "1aeaf55857e4e6236795c704f96b34d600252349d1a77b5962e777e3b0c0176a";
	public static string secretKey = "084fcd2b1262908c6c13e6960da6837628b04e83c89aababadba1b120d4d8053";
	public static string roomid = "1070595519";
	public static string username;
	Listener listen = new Listener();
	public static Vector3 newPos = new Vector3(0,0,0);
	public static Quaternion newRot = new Quaternion(0,0,0,0);
	public static bool shoot = false;
	public static bool isMessage = false;
	public static int messageFrame = 0;
	public static string chatMessage = "";
	public static string message = "wiadomosc";
	void Start () {;
		WarpClient.initialize(apiKey,secretKey);
		WarpClient.GetInstance().AddConnectionRequestListener(listen);
		WarpClient.GetInstance().AddChatRequestListener(listen);
		WarpClient.GetInstance().AddUpdateRequestListener(listen);
		WarpClient.GetInstance().AddLobbyRequestListener(listen);
		WarpClient.GetInstance().AddNotificationListener(listen);
		WarpClient.GetInstance().AddRoomRequestListener(listen);
		WarpClient.GetInstance().AddZoneRequestListener(listen);
		WarpClient.GetInstance ().AddTurnBasedRoomRequestListener (listen);
		// join with a unique name (current time stamp)
		username = System.DateTime.UtcNow.Ticks.ToString();
		WarpClient.GetInstance().Connect(username);
		
		//EditorApplication.playmodeStateChanged += OnEditorStateChanged;
		addPlayer();
	}
	
	public float interval = 0.1f;
	float timer = 0;

	
	public static GameObject obj;
	
	public static void addPlayer()
	{
		obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
		Weapon w = new Weapon ();
		obj.AddComponent<GrenadeThrow> ();
		obj.transform.position = new Vector3(732f,1.5f,500f);
	}
	
	public static void movePlayer(float x, float y, float z)
	{
		newPos = new Vector3(x,y,z);
	}

	public static void rotatePlayer(float rx,float ry,float rz,float rw)
	{
		newRot = new Quaternion(rx, ry, rz, rw);
	}

	public static void shootPlayer(float s)
	{
		if (s == 1) 
		{
			obj.GetComponent<GrenadeThrow>().Throw();
		}
		else
		{
			obj.GetComponent<GrenadeThrow>().Throw();
		}
	}

	public static void sendChatMessage(string s)
	{
		if (isMessage == false) {
						chatMessage = s;
						isMessage = true;
				}
	}
	
	void Update () {
		timer -= Time.deltaTime;
		if(timer < 0)
		{
			string json = "";

			if(shoot)
			{
				json = "{\"x\":\""+transform.position.x+"\",\"y\":\""+transform.position.y+"\",\"z\":\""+transform.position.z+"\",\"rx\":\""+transform.rotation.x+"\",\"ry\":\""+transform.rotation.y+"\",\"rz\":\""+transform.rotation.z+"\",\"rw\":\""+transform.rotation.w+"\",\"s\":\""+1+"\",\"m\":\""+message+"\"}";
			}
			else
			{
				json = "{\"x\":\""+transform.position.x+"\",\"y\":\""+transform.position.y+"\",\"z\":\""+transform.position.z+"\",\"rx\":\""+transform.rotation.x+"\",\"ry\":\""+transform.rotation.y+"\",\"rz\":\""+transform.rotation.z+"\",\"rw\":\""+transform.rotation.w+"\",\"s\":\""+0+"\",\"m\":\""+message+"\"}";
			}

			message = "";

			listen.sendMsg(json);
			
			timer = interval;
		}
		
		if (Input.GetKeyDown(KeyCode.Escape)) {
        	Application.Quit();
    	}
		WarpClient.GetInstance().Update();
		obj.transform.position = Vector3.Lerp(obj.transform.position, newPos, Time.deltaTime);
		obj.transform.rotation = newRot;
	}
	
	void OnGUI()
	{
		GUI.contentColor = Color.black;
		GUI.Label(new Rect(10,10,500,200), listen.getDebug());

		GUIStyle gs = new GUIStyle ();
		gs.fontSize = 10;
		//gs.font.material.color = new Color (255, 255, 255);
		if (messageFrame == 200) 
		{
			isMessage = false;
			messageFrame = 0;
			chatMessage = "";
		}

		if (isMessage) 
		{
			messageFrame++;
			GUI.Label (new Rect (10, 10, 600, 50), chatMessage, gs);
		}
	}
	
	/*void OnEditorStateChanged()
	{
    	if(EditorApplication.isPlaying == false) 
		{
			WarpClient.GetInstance().Disconnect();
    	}
	}*/
	
	void OnApplicationQuit()
	{
		WarpClient.GetInstance().Disconnect();
	}
	
}
