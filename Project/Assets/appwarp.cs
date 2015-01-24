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

public class appwarp : MonoBehaviour 
{
	public static string apiKey = "1aeaf55857e4e6236795c704f96b34d600252349d1a77b5962e777e3b0c0176a";
	public static string secretKey = "084fcd2b1262908c6c13e6960da6837628b04e83c89aababadba1b120d4d8053";
	public static string roomid = "1070595519";
	public static string username;

	public static float timeCounter = 0.0f;

	public static float interval = 0.1f;
	public static float timer = 0.1f;

	//chcemy troche ograniczyc tempo wysylania wiadomosci coby nie bylo spamu :)
	public static float msgInterval = 1f;
	public static float msgTimer = 1f;

	public static string messageToSend = "";
	public static Dictionary<float, string> messagesToDisplay = new Dictionary<float, string> ();

	Listener listen = new Listener();
		
	public static Dictionary<string, NetworkPlayer> players = new Dictionary<string, NetworkPlayer>();

	//flags
	public static bool isMovementKeyPressed = false;
	public static bool notifyShooting = false;
	public static bool catchFlag = false;
	public static bool dropFlag = false;

	//main functions
	void Start () 
	{
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
	}

	void resetFlags()
	{
		notifyShooting = false;
		catchFlag = false;
	    dropFlag = false;
	}

	void Update () 
	{
		//naliczanie czasu
		timeCounter += Time.deltaTime;

		//wysylanie wiadomosci co interval
		timer -= Time.deltaTime;
		if(timer < 0)
		{
			listen.sendMsg( toJson( getParameters() ) );
			resetFlags();
			timer = interval;
		}

		//wysylanie wiadomosci uzytkownika co msgInterval
		msgTimer -= Time.deltaTime;
		if(msgTimer < 0 && messageToSend.Length > 0) //jesli jest jakas wiadomosc
		{
			listen.sendMsg( toJson( messageToSend ) );
			msgTimer = msgInterval;

			messageToSend = "";
		}

		//---------------------------------------------------------------------------|
		
		manageInput();

		WarpClient.GetInstance().Update();
		
		foreach(string key in players.Keys)
		{
			players[key].update();
		}
	}
	
	void OnGUI()
	{
		//update messages
		foreach(KeyValuePair<float, string> msg in messagesToDisplay)
		{
			if( timeCounter - msg.Key > 3.0f )
			{
				messagesToDisplay.Remove( msg.Key );
			}
		}

		//show messages
		if( messagesToDisplay.Count > 0 )
		{
			//get 5 newest ones
			string[] topMsg = new string[5]{"", "", "", "", ""};

			List<float> keyList = new List<float>( messagesToDisplay.Keys );
			keyList.Sort();

			while( keyList.Count > 5 ) 
			{
				keyList.RemoveAt(0);
			}

			int temp_counter = 0;
			foreach(KeyValuePair<float, string> msg in messagesToDisplay)
			{
				topMsg[temp_counter] = msg.Value;

				temp_counter++;
			}

			//and show them

			GUIStyle descriptionStyle = new GUIStyle();
			descriptionStyle.wordWrap = true;

			string outMessage = "";

			for(int i = 0; i < 5; i++)
			{
				outMessage += topMsg[i] + "\n";
			}

			GUI.Button (new Rect (10,70, 400, 300), outMessage, descriptionStyle);
		}
	}

	void OnApplicationQuit()
	{
		WarpClient.GetInstance().Disconnect();
	}

	//!!tu uzupelniamy dane ktorych potem uzywamy w funkcji manageNotification() ponizej!
	private float[] getParameters()
	{
		return new float[]
		{
			transform.position.x,
			transform.position.y,
			transform.position.z,

			transform.rotation.x,
			transform.rotation.y,
			transform.rotation.z,
			transform.rotation.w,

			isMovementKeyPressed ? 1.0f : 0.0f,

			notifyShooting ? 1.0f : 0.0f,

			catchFlag ? 1.0f : 0.0f,
			dropFlag ? 1.0f : 0.0f
		};
	}
	
	//managing functions
	//uwaga! w celu uzyskania wartosci float danych z tabelki, wykonujemy na nich funkcje p()
	public static void manageNotification(string username, string parameters)
	{
		string[] p = deserializeParameters(parameters);

		//ignorujemy notyfikacje od siebie
		if( username != appwarp.username )
		{
			movePlayer( f(p[0]), f(p[1]), f(p[2]), username);

			rotatePlayer( f(p[3]), f(p[4]), f(p[5]), f(p[6]), username);

			bool keyPressed = ( f(p[7]) == 1.0f ) ? true : false;
			setPlayerMovementState(keyPressed, username);

			bool doStartShooting = ( f(p[8]) == 1.0f ) ? true : false;
			startShooting( doStartShooting, username );
		}

		//nie ignorujemy notyfikacji od siebie
	}

	public static float f(string o)
	{
		return float.Parse (o);
	}

	public static void addPlayer(string uname)
	{
		players.Add(uname, new NetworkPlayer(uname));
	}

	private void manageInput()
	{
		if (Input.GetKeyDown(KeyCode.Escape)) 
		{
			Application.Quit();
		}

		if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
		{
			isMovementKeyPressed = true;
		}
		else
		{
			isMovementKeyPressed = false;
		}

		//na potrzeby testow - jak bedzie mozliwosc wpisywania msg z ekranu to od razu do wywalenia!
		if(Input.GetKey(KeyCode.H)) {messageToSend = "WIADOMOSC SPOD KLAWISZA h";}
		else if(Input.GetKey(KeyCode.J)){ messageToSend = "WIADOMOSC SPOD KLAWISZA j";}
		else if(Input.GetKey(KeyCode.K)){ messageToSend = "WIADOMOSC SPOD KLAWISZA k";}
	}

	//action functions
	public static void movePlayer(float x, float y, float z, string uname)
	{
		players[uname].Position = new Vector3(x,y,z);
	}

	public static void rotatePlayer(float x,float y,float z,float w, string uname)
	{
		players[uname].Rotation = new Quaternion(x, y, z, w);
	}

	public static void setPlayerMovementState(bool isPlayerMovementKeyPressed, string uname)
	{
		players[uname].IsMovementKeyPressed = isPlayerMovementKeyPressed;
	}
	
	public static void startShooting(bool doStartShooting, string uname)
	{
		players[uname].Shooting = doStartShooting;
	}

	public static void showMessage(string message, string uname)
	{
		string m = uname + ": " + message;

		messagesToDisplay.Add (timeCounter, m);
	}

	//serializing functions
	private string serializeParameters(float[] parameters)
	{
		string serializedParameters = "";

		for (int i = 0; i < parameters.Length; i++) 
		{
			serializedParameters += parameters[i].ToString();

			if( i != parameters.Length - 1 ) serializedParameters += ';';
		}

		return serializedParameters;
	}

	private static string[] deserializeParameters(string serializedParameters)
	{
		string [] splittedSerializedParameters = serializedParameters.Split(';');

		/*object[] parameters = new object[splittedSerializedParameters.Length];

		for (int i = 0; i < splittedSerializedParameters.Length; i++) 
		{
			parameters[i] = splittedSerializedParameters[i];
		}*/
		
		return splittedSerializedParameters;
	}

	private string toJson(float[] parameters)
	{
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		
		sb.Append("{\"parameters\":\"" + serializeParameters(parameters) + "\"}");

		//Debug.Log ("?????????????????????????????????????????????????????????????????????????????" + sb.ToString() );

		return sb.ToString();
	}

	private string toJson(string message)
	{
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		
		sb.Append("{\"message\":\"" + message + "\"}");
				
		return sb.ToString();
	}
}
