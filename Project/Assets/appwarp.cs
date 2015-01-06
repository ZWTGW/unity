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
	public static bool isMovementKeyPressed = false;
	public static int messageFrame = 0;
	public static string chatMessage = "";
	public static string message = "wiadomosc";

	public static Dictionary<string, GameObject> gObjects = new Dictionary<string, GameObject>();
	public static Dictionary<string, Vector3> positions = new Dictionary<string, Vector3>();
	public static Dictionary<string, NetworkPlayer> players = new Dictionary<string, NetworkPlayer>();

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

		message = username;

		WarpClient.GetInstance().Connect(username);

	}
	
	public float interval = 0.1f;
	float timer = 0;

	public static void addPlayer()
	{
		GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);

		Weapon w = new Weapon ();
		obj.AddComponent<GrenadeThrow> ();
		obj.transform.position = new Vector3(732f,1.5f,500f);
	}

	public static void addPlayer(string uname)
	{
		players.Add(uname, new NetworkPlayer(uname));
	}
	
	public static void movePlayer(float x, float y, float z, string uname)
	{
		players[uname].Position = new Vector3(x,y,z);
	}

	public static void rotatePlayer(float rx,float ry,float rz,float rw, string uname)
	{
		players[uname].Rotation = new Quaternion(rx, ry, rz, rw);
	}

	public static void setPlayerMovementState(string uname, bool isPlayerMovementKeyPressed){
		players[uname].IsMovementKeyPressed = isPlayerMovementKeyPressed;
	}

	public static void shootPlayer(float s)
	{
		if (s == 1) 
		{
			//obj.GetComponent<GrenadeThrow>().Throw();
		}
		else
		{
			//obj.GetComponent<GrenadeThrow>().Throw();
		}
	}

	public static void sendChatMessage(string s)
	{
		if (isMessage == false) {
						chatMessage = s;
						isMessage = true;
				}
	}

	private void manageInput(){
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit();
		}
		if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
			isMovementKeyPressed = true;
		else
			isMovementKeyPressed = false;
	}

	private string toJson(){
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		string shootInfo = shoot ? "1" : "0";
		string movementKeyInfo = isMovementKeyPressed ? "1" : "0";
		sb.Append("{\"x\":\""+transform.position.x+"\",");
		sb.Append("\"y\":\""+transform.position.y+"\",");
		sb.Append("\"z\":\""+transform.position.z+"\",");
		sb.Append("\"rx\":\""+transform.rotation.x+"\",");
		sb.Append("\"ry\":\""+transform.rotation.y+"\",");
		sb.Append("\"rz\":\""+transform.rotation.z+"\",");
		sb.Append("\"rw\":\""+transform.rotation.w+"\",");
		sb.Append("\"s\":\""+shootInfo+"\",");
		sb.Append("\"mk\":\""+movementKeyInfo+"\",");
		sb.Append("\"m\":\""+message+"\"}");
		return sb.ToString();
	}
	
	void Update () {
		timer -= Time.deltaTime;
		if(timer < 0)
		{
			listen.sendMsg(toJson());
			message = "";
			timer = interval;
		}
		
		manageInput();
		WarpClient.GetInstance().Update();

		foreach(string key in players.Keys){
			players[key].update();
		}

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
