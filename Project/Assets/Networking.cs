using UnityEngine;
using System.Collections;

public class Networking : MonoBehaviour {
	public GameObject playerPrefab;
	//public GameObject player2Prefab;

	private const string typeName = "UniqueGameName";
	private const string gameName = "RoomName";

	private HostData[] hostList;

	//********** client ***************
	private void RefreshHostList()
	{
		MasterServer.RequestHostList(typeName);
	}
	
	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if (msEvent == MasterServerEvent.HostListReceived)
			hostList = MasterServer.PollHostList();
	}

	private void JoinServer(HostData hostData)
	{
		Network.Connect(hostData);
	}
	
	void OnConnectedToServer()
	{
		Debug.Log("Server Joined");
		SpawnPlayer();
	}

	void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		Debug.Log(info.ToString());
	}

	void OnFailedToConnect(NetworkConnectionError error)
	{
		Debug.Log(error);
	}
	//************************************

	//************ server ******************
	private void StartServer()
	{
		Network.InitializeServer(4, 25000, !Network.HavePublicAddress());
		MasterServer.RegisterHost(typeName, gameName);
	}

	void OnServerInitialized()
	{
		Debug.Log("Server Initializied");
		SpawnPlayer();
	}

	void OnPlayerConnected(NetworkPlayer player)
	{
		Debug.Log(player.ipAddress+"connected");
	}

	void OnPlayerDisconnected(NetworkPlayer player)
	{
		Debug.Log(player.ipAddress + "disconnected");
		// usunięcie RPC należących do gracza
		Network.RemoveRPCs(player);
		// usunięcie obiektów należących do gracza
		Network.DestroyPlayerObjects(player);
	}
	//************************************


	private void SpawnPlayer()
	{
		int rand = Random.Range (10, 10);
		Network.Instantiate(playerPrefab, new Vector3(70f + rand, 1.5f, 170f), Quaternion.identity, 0);
		
		GraGUI.serverIp = MasterServer.ipAddress;
		GraGUI.mapName = "Antique";
	}


	// zamykanie połączenia (u klienta i na serwerze)
	void closeConnection()
	{
		if (Network.isServer)
		{
			// zdjęcie serwera z listy hostów
			MasterServer.UnregisterHost();
			// usunięcie nagromadzonych zbuforowanych wywołań RPC
			Network.RemoveRPCsInGroup(0);
		}
		// rozłączenie
		Network.Disconnect();
	}
	
	// Use this for initialization
	void Start () {
		Application.runInBackground = true;
		MasterServer.ipAddress = "127.0.0.1";
		MasterServer.port = 23466;
	}

	void OnGUI()
	{
		if(!GraGUI.networkTakenCareOf) {  
			if(GraGUI.hostToJoin == null) {
				StartServer();
			}
			else {
				JoinServer(GraGUI.hostToJoin);
			}
			GraGUI.networkTakenCareOf = true;
		}

/*		if (!Network.isClient && !Network.isServer)
		{
			if (GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))
				StartServer();
			
			if (GUI.Button(new Rect(100, 250, 250, 100), "Refresh Hosts"))
				RefreshHostList();
			
			if (hostList != null)
			{
				for (int i = 0; i < hostList.Length; i++)
				{
					if (GUI.Button(new Rect(400, 100 + (110 * i), 300, 100), hostList[i].gameName))
						JoinServer(hostList[i]);
				}
			}
		}*/
	}
	
	
	// Update is called once per frame
	void Update () {
	
	}
}
