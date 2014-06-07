using UnityEngine;
using System.Collections;


public class GraGUI : MonoBehaviour{
	
	private BaseCharacter baseCharScript;
	public bool isMainMenu; // scena glownego menu, czy nie? jesli nie to ingame menu i hudy etc.
	
	
	// tu ok bo instancja nowa moze byc
	// ale czy na pewno za kazdym razem chcemy nowa instancje obiektu basecharacter? nie sądzę
	public UserSettings us = new UserSettings();
	
	private bool showInGameMenu = false; // czy pokazac menu?
	private float sliderValue = 1f;
	enum MenuStates { MAIN, OPTIONS, SOUND, REDEFINE, NETWORK };
	MenuStates menuState = MenuStates.MAIN;
	string keyChanging = "";
	
	private const string typeName = "UniqueGameName";
	private const string gameName = "RoomName";
	
	private HostData[] hostList = null;
	public static HostData hostToJoin = null;
	public static bool networkTakenCareOf = false;
	
	//mantkowicz zmienne do wyswietlania statsow
	private bool showTeamStatistics = false;
	private bool showPersonalStatistics = false;
	
	private float teamStatisticsBoxWidth = 0.0f;
	private float teamStatisticsBoxHeight = 0.0f;
	private float teamStatisticsOffset = 0.0f;
	private bool  teamStatisticsBoxInitialized = false;
	private float teamStatisticsBoxStep = 0.0f;
	
	private float personalStatisticsBoxWidth = 0.0f;
	private float personalStatisticsBoxHeight = 0.0f;
	private float personalStatisticsOffset = 0.0f;
	private bool  personalStatisticsBoxInitialized = false;
	private float personalStatisticsBoxStep = 0.0f;
	
	private Texture2D skullTexture;
	private Texture2D leftLineTexture;
	
	private string[] steampunksPlayerInfo;
	private string[] futuresPlayerInfo;
	
	public static string serverIp = "";
	public static string mapName = "";
	//endof mantkowicz
	
	// Use this for initialization
	void Start () {
		Application.runInBackground = true;
		MasterServer.ipAddress = "127.0.0.1";
		MasterServer.port = 23466;
		
		skullTexture = Resources.Load<Texture2D> ("skull");
		leftLineTexture = Resources.Load<Texture2D> ("leftLine");
		
		steampunksPlayerInfo = new string[20];
		futuresPlayerInfo = new string[20];
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			// pokazujemy kursorek do menu, albo nie pokazujemy w edytorze unity to chyba
			// i tak zawsze jest
			showInGameMenu = !showInGameMenu;
			Screen.showCursor = showInGameMenu;
			menuState = MenuStates.MAIN;
		}
		
		//mantkowicz - dodanie wyswietlania statystyk tab/alt
		if( Input.GetKey( KeyCode.Tab ) ) 
		{
			if( !teamStatisticsBoxInitialized )
			{
				teamStatisticsBoxWidth = Screen.width * 0.4f;
				teamStatisticsBoxHeight = Screen.height * 0.5f;
				teamStatisticsOffset = -teamStatisticsBoxWidth;
				teamStatisticsBoxStep = teamStatisticsBoxWidth / 10.0f;
				
				teamStatisticsBoxInitialized = true;
			}
			
			showTeamStatistics = true;
		}
		else
		{
			showTeamStatistics = false;
			teamStatisticsBoxInitialized = false;
		}
		
		if( Input.GetKey( KeyCode.LeftAlt ) ) 
		{
			if( !personalStatisticsBoxInitialized )
			{
				personalStatisticsBoxWidth = Screen.width * 0.3f;
				personalStatisticsBoxHeight = Screen.height * 0.2f;
				personalStatisticsOffset = -personalStatisticsBoxHeight;
				personalStatisticsBoxStep = personalStatisticsBoxHeight / 10.0f;
				
				personalStatisticsBoxInitialized = true;
			}
			
			showPersonalStatistics = true;
		}
		else
		{
			showPersonalStatistics = false;
			personalStatisticsBoxInitialized = false;
		}
		//endof mantkowicz
	}
	
	void Awake() {
		us.Load ();
		baseCharScript = GetComponent<BaseCharacter>();
	}
	
	private void HUD() {
		if(!networkView.isMine) return;
		//camera.RenderWithShader(
		// te rzeczy trzeba by pozniej pobrac z jakiegos obiektu player czy cos takiego
		// na razie dla testu stale
		if( !baseCharScript.startTimeIsSet )
		{
			baseCharScript.secondsAtGameStart = (System.DateTime.UtcNow.Ticks - System.DateTime.Parse("01/01/1970 00:00:00").Ticks) / 10000000;
			baseCharScript.startTimeIsSet = true;
		}

		int hp = baseCharScript.currHP;
		int maxHp = baseCharScript.maxHP;
		int ammo = 15;
		int maxAmmo = 20;
		int stamina = (int)baseCharScript.stamina;
		int maxStamina = (int)baseCharScript.maxStamina;
		
		// ustawiamy skórkę gui
		GUI.skin.label.normal.textColor = new Color (255, 255, 255);
		GUI.skin.label.fontSize = 40;
		GUI.skin.label.alignment = TextAnchor.UpperLeft;
		
		// rysujemy gui
		GUI.Label (new Rect (10, Screen.height - 50, 1000, 100), hp + " HP");
		
		// to chcemy wyrownane do prawej ale w sumie troche lewo to dziala
		GUI.skin.label.alignment = TextAnchor.UpperRight;
		GUI.Label (new Rect (170, Screen.height - 50, 1000, 100), "Ammo " + ammo + "/" + maxAmmo);
		
		// celownik
		// trzeba wrzucac teksturki do katalogu Assets/Resources - nie logiczne bo standardowo tego nie ma
		// tylko zwykle assets, ale inaczej resources.load nie zadziala
		// pewnie mozna inaczej ale tak najprosciej
		Texture2D celownik = (Texture2D)Resources.Load ("celownik");
		Rect srodek = new Rect ((Screen.width - celownik.width) / 2, (Screen.height - celownik.height) / 2, 32, 32);
		
		GUI.DrawTexture (srodek, celownik);
		
		Texture2D orb2 = (Texture2D)Resources.Load ("hud_live_0");
		Texture2D orb1 = (Texture2D)Resources.Load ("hud_live_1");
		
		float resizedW = orb1.width / 5;
		float resizedH = orb2.height / 5;
		
		Rect pozycjaOrba = new Rect (5, Screen.height - 250, resizedW, resizedH);
		// rysujemy caly orb pusty
		GUI.DrawTexture (pozycjaOrba, orb2);
		// rysujemy zajety fragment
		drawFragment (orb1, (float)hp / (float)maxHp, 1.0f, resizedW, resizedH, pozycjaOrba.x, pozycjaOrba.y);
		
		
		orb2 = (Texture2D)Resources.Load ("hud_stamina_0");
		orb1 = (Texture2D)Resources.Load ("hud_stamina_1");
		
		pozycjaOrba = new Rect (5, Screen.height - 200, resizedW, resizedH);
		// rysujemy caly orb pusty
		GUI.DrawTexture (pozycjaOrba, orb2);
		// rysujemy zajety fragment
		drawFragment (orb1, (float)stamina / (float)maxStamina, 1.0f, resizedW, resizedH, pozycjaOrba.x, pozycjaOrba.y);
		
		orb2 = (Texture2D)Resources.Load ("hud_ammo_2");
		orb1 = (Texture2D)Resources.Load ("hud_ammo_1");
		
		pozycjaOrba = new Rect (Screen.width - resizedW - 10f, Screen.height - 250, resizedW, resizedH);
		// rysujemy caly orb pusty
		GUI.DrawTexture (pozycjaOrba, orb2);
		// rysujemy zajety fragment
		drawFragment (orb1, (float)ammo / (float)maxAmmo, 1.0f, resizedW, resizedH, pozycjaOrba.x, pozycjaOrba.y);
		
		
		
		// sprawdzamy czy szarosc
		MakeGray mg = GameObject.Find ("PlayerCam").GetComponent<MakeGray> ();
		// na razie wlaczamy szarosc jak jest mniej niz 85 hp, dlatego
		// ze jak sie spadnie z tego klocka co maciek dal to tak jest ;)
		if (mg != null) {
			if (hp >= 25)
				mg.grayEnabled = false;
			else
				mg.grayEnabled = true;
		}
	}
	
	private void drawFragment(Texture2D texturka, float w, float h, float maxW, float maxH, float pozX, float pozY) {
		// skrot do ponizszego od lewej gornej krawedzi
		drawFragment(texturka, 0.0f, 0.0f, w, h, maxW, maxH, pozX, pozY);
	}
	
	private void drawFragment(Texture2D texturka, float x, float y, float w, float h, float maxW, float maxH, float pozX, float pozY) {
		// do rysowania fragmentu textury
		// trick z http://answers.unity3d.com/questions/160560/select-part-of-the-texture-in-guidrawtexture.html
		Vector2 position = new Vector2( pozX, pozY );
		Rect textureCrop = new Rect( x, y, w, h );
		
		GUI.BeginGroup( new Rect( position.x, position.y, maxW * textureCrop.width, maxH * textureCrop.height ) );
		GUI.DrawTexture( new Rect( -maxW * textureCrop.x, -maxH * textureCrop.y, maxW, maxH ), texturka );
		GUI.EndGroup();
	}
	
	
	
	
	
	private void InGameMenu() {
		
		// wymiary calego menu
		int w = 500;
		int h = 370;
		
		// skin
		GUI.skin.box.fontSize = 40;
		GUI.skin.label.fontSize = 35;
		GUI.skin.button.fontSize = 35;
		GUI.skin.label.alignment = TextAnchor.UpperLeft;
		GUI.skin.horizontalSlider.stretchHeight = true;
		
		// wlasciwe rysowanie menu w zaleznosci od stanu
		
		GUI.BeginGroup(new Rect((Screen.width - w)/2,(Screen.height - h)/2, w, h));
		
		switch(menuState) {
		case MenuStates.NETWORK:
			GUI.Box(new Rect(0,0,w,h), "NEW GAME");
			
			if (!Network.isClient && !Network.isServer)
			{
				networkTakenCareOf = false;
				
				if (GUI.Button(new Rect(100, 100, 250, 100), "Start Server")) {
					hostToJoin = null;
					Application.LoadLevel(1);
				}
				
				
				if (GUI.Button(new Rect(100, 250, 250, 100), "Refresh Hosts")) {
					MasterServer.ClearHostList();
					MasterServer.RequestHostList(typeName);
				}
				
				if (MasterServer.PollHostList().Length != 0) {
					hostList = MasterServer.PollHostList();
					//MasterServer.ClearHostList();
				}
				
				if (hostList != null)
				{
					for (int i = 0; i < hostList.Length; i++)
					{
						if (GUI.Button(new Rect(400, 100 + (110 * i), 300, 100), hostList[i].gameName)) {
							hostToJoin = hostList[i];
							Application.LoadLevel(1);
						}
						
					}
				}
			}
			break;
			
		default:
		case MenuStates.MAIN:
			GUI.Box(new Rect(0,0,w,h), "MENU");
			
			if(!isMainMenu) {
				if (GUI.Button (new Rect (15, 50, w * 0.95f, 60), "BACK TO GAME")) {
					showInGameMenu = false;
				}
			}
			else {
				if (GUI.Button (new Rect (15, 50, w * 0.95f, 60), "NEW GAME")) {
					
					// ---------- kod pozyczony z networking.cs 
					if (Network.isServer)
					{
						// zdjęcie serwera z listy hostów
						MasterServer.UnregisterHost();
						// usunięcie nagromadzonych zbuforowanych wywołań RPC
						Network.RemoveRPCsInGroup(0);
					}
					// rozłączenie
					Network.Disconnect();
					
					menuState = MenuStates.NETWORK;
					
					
					// 0 = menu, 1 = nasz level
					//Application.LoadLevel(1);
				}
			}
			
			if (GUI.Button (new Rect (15, 50 + 60 + 60, w * 0.95f, 60), "OPTIONS")) {
				menuState = MenuStates.OPTIONS;
			}
			
			if(isMainMenu) {
				if (GUI.Button (new Rect (15, h - 70, w * 0.95f, 60), "QUIT TO DESKTOP")) {
					Application.Quit();
					// to nie dziala jak sie odpala w edytorze unity (trzeba wyeksportowac i dopiero)
				}
			}
			else {
				if (GUI.Button (new Rect (15, h - 70, w * 0.95f, 60), "QUIT TO MAIN MENU")) {
					Application.LoadLevel(0);
				}
			}
			
			
			break;
			
		case MenuStates.OPTIONS:
			GUI.Box(new Rect(0,0,w,h), "OPTIONS");
			if (GUI.Button (new Rect (15, 50, w * 0.95f, 60), "SOUND OPTIONS")) {
				menuState = MenuStates.SOUND;
			}
			
			if (GUI.Button (new Rect (15, 50 + 60 + 60, w * 0.95f, 60), "KEY BINDINGS")) {
				menuState = MenuStates.REDEFINE;
			}
			
			if (GUI.Button (new Rect (15, h - 70, w * 0.95f, 60), "BACK")) {
				menuState = MenuStates.MAIN;
			}
			break;
			
		case MenuStates.SOUND:
			GUI.Box(new Rect(0,0,w,h), "SOUND");
			GUI.Label (new Rect (15, 50 + 90, w * 0.95f, 60), "VOLUME");
			sliderValue = GUI.HorizontalSlider (new Rect (15, 50 + 90 + 50, w * 0.95f, 60), sliderValue, 0, 1);	
			AudioListener.volume = sliderValue;
			if (GUI.Button (new Rect (15, h - 70, w * 0.95f, 60), "BACK")) {
				menuState = MenuStates.OPTIONS;
			}
			break;
			
		case MenuStates.REDEFINE:
			GUI.skin.label.fontSize = 20;
			GUI.skin.button.fontSize = 20;
			
			GUI.Box(new Rect(0,0,w,h), "KEY BINDINGS");
			
			int offset = 0; // offset przy rysowaniu
			
			ArrayList sortedKeys = new ArrayList();
			
			foreach(string name in us.sortedKeys) { // kiedys bylo us.keys.Keys ale hashtable nie są po kolei, a pasowaloby zeby klawisze do sterowania byly na poczatku
				//Debug.Log(name + " " + us.keys[name]);
				GUI.Label(new Rect(15, 50 + offset, w * 0.95f, 30), name);
				GUI.Label(new Rect(100, 50 + offset, w * 0.95f, 30), us.keys[name].ToString());
				
				if(name == keyChanging) { // zmieniamy klawisz ktory aktualnie wyswietlamy
					if(GUI.Button(new Rect (200, 50 + offset, 280, 30), "PRESS KEY")) {
						keyChanging = "";
					}
				}
				else { // nie zmieniamy
					if(GUI.Button(new Rect (200, 50 + offset, 280, 30), "CHANGE")) {
						keyChanging = name;
					}
				}
				offset += 32;
			}
			
			GUI.skin.button.fontSize = 35;
			
			if (GUI.Button (new Rect (15, h - 70, w * 0.95f, 60), "BACK")) {
				menuState = MenuStates.OPTIONS;
			}
			
			// tutaj czekamy na jakis klawisz
			if(keyChanging != "") { // cos zmieniamy
				Event e = Event.current;
				KeyCode kc = KeyCode.None;
				// to ponizej nie wykryje shifta bo
				// prawdziwy bug w kochanym Unity 
				// http://forum.unity3d.com/threads/30343-current-event-not-detecting-shift-key
				if(e.isKey && e.keyCode != KeyCode.None) {
					Debug.Log("nacisneto klawisz: " + e.keyCode);
					kc = e.keyCode;
				}
				
				// wiec workaround z tego threada ponizej
				
				if(e.shift) {
					if (Input.GetKey(KeyCode.LeftShift)) {
						Debug.Log("lewy szift");
						kc = KeyCode.LeftShift;
					}
					else if (Input.GetKey(KeyCode.RightShift)){
						Debug.Log("prawy szift");
						kc = KeyCode.RightShift;
					}
				}
				
				// jak mamy klawisz to dokonujemy zamiany
				if(kc != KeyCode.None) {
					us.ChangeKey(keyChanging, kc);
					keyChanging = "";
					us.Save();
				}
			}
			
			break;
		}
		
		GUI.EndGroup();
		
	}
	
	void OnGUI() {
		if (isMainMenu) {
			InGameMenu();
		}
		
		else {
			HUD ();
			if (showInGameMenu)
				InGameMenu();
		}
		
		//mantkowicz - wywolywanie funkcji do wyswietlania stats
		if( showTeamStatistics )
		{
			displayTeamStatistics();
		}
		else 
		{
			hideTeamStatistics();
		}
		
		if( showPersonalStatistics )
		{
			displayPersonalStatistics();
		}
		else 
		{
			hidePersonalStatistics();
		}
	}
	
	private void loadSteampunksPlayerInfo()
	{
		for(int i=0; i<20; i+=4)
		{
			steampunksPlayerInfo[i+1] = "+";
			steampunksPlayerInfo[i+2] = (i+2).ToString();
			steampunksPlayerInfo[i+3] = (i+3).ToString();
		}
	}
	
	private void loadFuturesPlayerInfo()
	{
		for(int i=0; i<20; i+=4)
		{
			futuresPlayerInfo[i] = "Fut"+i.ToString();
			futuresPlayerInfo[i+1] = "+";
			futuresPlayerInfo[i+2] = (i+5).ToString();
			futuresPlayerInfo[i+3] = (i+7).ToString();
		}
	}
	
	private void drawTeamStatisticsBox(float offset)
	{
		GUI.skin.label.fontSize = (int) (Screen.height * 0.045f);
		GUI.skin.label.alignment = TextAnchor.MiddleLeft;
		GUI.skin.label.padding.left = 20;
		GUI.Box( new Rect( offset, (teamStatisticsBoxHeight / 4.0f), teamStatisticsBoxWidth, teamStatisticsBoxHeight ), "" );
		
		float yOffset = 15.0f;
		
		//DRAWING TABLE TEMPLATE
		GUI.Label( new Rect( offset, (teamStatisticsBoxHeight / 4.0f) + yOffset, (teamStatisticsBoxWidth/2.0f), (teamStatisticsBoxHeight/10.0f) ), "TEAM1" );
		GUI.Label( new Rect( offset + (teamStatisticsBoxWidth/2.0f), (teamStatisticsBoxHeight / 4.0f) + yOffset, (teamStatisticsBoxWidth/2.0f), (teamStatisticsBoxHeight/10.0f) ), "TEAM2" );
		
		yOffset += (teamStatisticsBoxHeight / 10.0f) + 5.0f;
		
		GUI.skin.label.fontSize = (int) (Screen.height * 0.027f);
		GUI.skin.label.padding.left = 30;
		
		GUI.Label( new Rect( offset, (teamStatisticsBoxHeight / 4.0f) + yOffset, (teamStatisticsBoxWidth/10.0f) * 2.5f, (teamStatisticsBoxHeight/10.0f) ), "name" );
		GUI.Label( new Rect( offset + (teamStatisticsBoxWidth/10.0f) * 5.0f, (teamStatisticsBoxHeight / 4.0f) + yOffset, (teamStatisticsBoxWidth/10.0f) * 2.5f, (teamStatisticsBoxHeight/10.0f) ), "name" );
		
		GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		GUI.skin.label.padding.left = 0;
		
		GUI.Label( new Rect( offset + (teamStatisticsBoxWidth/10.0f) * 2.5f, (teamStatisticsBoxHeight / 4.0f) + yOffset, (teamStatisticsBoxWidth/10.0f) * 0.75f, (teamStatisticsBoxHeight/10.0f) ), skullTexture );
		GUI.Label( new Rect( offset + (teamStatisticsBoxWidth/10.0f) * 3.25f, (teamStatisticsBoxHeight / 4.0f) + yOffset, (teamStatisticsBoxWidth/10.0f) * 0.75f, (teamStatisticsBoxHeight/10.0f) ), "f" );
		GUI.Label( new Rect( offset + (teamStatisticsBoxWidth/10.0f) * 4.0f, (teamStatisticsBoxHeight / 4.0f) + yOffset, (teamStatisticsBoxWidth/10.0f) * 0.75f, (teamStatisticsBoxHeight/10.0f) ), "d" );
		GUI.Label( new Rect( offset + (teamStatisticsBoxWidth/10.0f) * 4.75f, (teamStatisticsBoxHeight / 4.0f) + yOffset, (teamStatisticsBoxWidth/10.0f) * 0.5f, (teamStatisticsBoxHeight/10.0f) ), "|" );
		
		GUI.Label( new Rect( offset + (teamStatisticsBoxWidth/10.0f) * 7.5f, (teamStatisticsBoxHeight / 4.0f) + yOffset, (teamStatisticsBoxWidth/10.0f) * 0.75f, (teamStatisticsBoxHeight/10.0f) ), skullTexture );
		GUI.Label( new Rect( offset + (teamStatisticsBoxWidth/10.0f) * 8.25f, (teamStatisticsBoxHeight / 4.0f) + yOffset, (teamStatisticsBoxWidth/10.0f) * 0.75f, (teamStatisticsBoxHeight/10.0f) ), "f" );
		GUI.Label( new Rect( offset + (teamStatisticsBoxWidth/10.0f) * 9.0f, (teamStatisticsBoxHeight / 4.0f) + yOffset, (teamStatisticsBoxWidth/10.0f) * 0.75f, (teamStatisticsBoxHeight/10.0f) ), "d" );
		//endof DRAWING TABLE TEMPLATE
		
		loadSteampunksPlayerInfo();
		loadFuturesPlayerInfo();
		
		for(int i = 0; i< 20; i += 4)
		{
			yOffset += (teamStatisticsBoxHeight / 10.0f);
			
			GUI.Label( new Rect( offset, (teamStatisticsBoxHeight / 4.0f) + yOffset, (teamStatisticsBoxWidth/10.0f) * 2.5f, (teamStatisticsBoxHeight/10.0f) ), steampunksPlayerInfo[i] );
			GUI.Label( new Rect( offset + (teamStatisticsBoxWidth/10.0f) * 5.0f, (teamStatisticsBoxHeight / 4.0f) + yOffset, (teamStatisticsBoxWidth/10.0f) * 2.5f, (teamStatisticsBoxHeight/10.0f) ), futuresPlayerInfo[i] );
			
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
			GUI.skin.label.padding.left = 0;
			
			GUI.Label( new Rect( offset + (teamStatisticsBoxWidth/10.0f) * 2.5f, (teamStatisticsBoxHeight / 4.0f) + yOffset, (teamStatisticsBoxWidth/10.0f) * 0.75f, (teamStatisticsBoxHeight/10.0f) ), steampunksPlayerInfo[i+1] );
			GUI.Label( new Rect( offset + (teamStatisticsBoxWidth/10.0f) * 3.25f, (teamStatisticsBoxHeight / 4.0f) + yOffset, (teamStatisticsBoxWidth/10.0f) * 0.75f, (teamStatisticsBoxHeight/10.0f) ), steampunksPlayerInfo[i+2] );
			GUI.Label( new Rect( offset + (teamStatisticsBoxWidth/10.0f) * 4.0f, (teamStatisticsBoxHeight / 4.0f) + yOffset, (teamStatisticsBoxWidth/10.0f) * 0.75f, (teamStatisticsBoxHeight/10.0f) ), steampunksPlayerInfo[i+3] );
			
			GUI.Label( new Rect( offset + (teamStatisticsBoxWidth/10.0f) * 4.75f, (teamStatisticsBoxHeight / 4.0f) + yOffset, (teamStatisticsBoxWidth/10.0f) * 0.5f, (teamStatisticsBoxHeight/10.0f) ), "|" );
			
			GUI.Label( new Rect( offset + (teamStatisticsBoxWidth/10.0f) * 7.5f, (teamStatisticsBoxHeight / 4.0f) + yOffset, (teamStatisticsBoxWidth/10.0f) * 0.75f, (teamStatisticsBoxHeight/10.0f) ), futuresPlayerInfo[i+1] );
			GUI.Label( new Rect( offset + (teamStatisticsBoxWidth/10.0f) * 8.25f, (teamStatisticsBoxHeight / 4.0f) + yOffset, (teamStatisticsBoxWidth/10.0f) * 0.75f, (teamStatisticsBoxHeight/10.0f) ), futuresPlayerInfo[i+2] );
			GUI.Label( new Rect( offset + (teamStatisticsBoxWidth/10.0f) * 9.0f, (teamStatisticsBoxHeight / 4.0f) + yOffset, (teamStatisticsBoxWidth/10.0f) * 0.75f, (teamStatisticsBoxHeight/10.0f) ), futuresPlayerInfo[i+3] );
		}
		
		yOffset += (teamStatisticsBoxHeight / 14.0f);
		yOffset += (teamStatisticsBoxHeight / 14.0f);
		
		GUI.skin.label.padding.left = 10;
		GUI.skin.label.padding.right = 10;
		GUI.skin.label.alignment = TextAnchor.MiddleRight;
		GUI.Label( new Rect( offset, (teamStatisticsBoxHeight / 4.0f) + yOffset, (teamStatisticsBoxWidth/10.0f) * 2.0f, (teamStatisticsBoxHeight/10.0f) ), "server IP:" );
		GUI.skin.label.alignment = TextAnchor.MiddleLeft;
		GUI.Label( new Rect( offset + (teamStatisticsBoxWidth/10.0f) * 2.0f, (teamStatisticsBoxHeight / 4.0f) + yOffset, (teamStatisticsBoxWidth/10.0f) * 3.0f, (teamStatisticsBoxHeight/10.0f) ), serverIp );
		
		
		yOffset += (teamStatisticsBoxHeight / 14.0f);
		
		GUI.skin.label.alignment = TextAnchor.MiddleRight;
		GUI.Label( new Rect( offset, (teamStatisticsBoxHeight / 4.0f) + yOffset, (teamStatisticsBoxWidth/10.0f) * 2.0f, (teamStatisticsBoxHeight/10.0f) ), "map:" );
		GUI.skin.label.alignment = TextAnchor.MiddleLeft;
		GUI.Label( new Rect( offset + (teamStatisticsBoxWidth/10.0f) * 2.0f, (teamStatisticsBoxHeight / 4.0f) + yOffset, (teamStatisticsBoxWidth/10.0f) * 2.0f, (teamStatisticsBoxHeight/10.0f) ), mapName );
		GUI.skin.label.alignment = TextAnchor.MiddleRight;
		GUI.Label( new Rect( offset + (teamStatisticsBoxWidth/10.0f) * 4.0f, (teamStatisticsBoxHeight / 4.0f) + yOffset, (teamStatisticsBoxWidth/10.0f) * 3.0f, (teamStatisticsBoxHeight/10.0f) ), "game time:" );
		GUI.skin.label.alignment = TextAnchor.MiddleLeft;
		
		long secondsFromGameStart = (System.DateTime.UtcNow.Ticks - System.DateTime.Parse ("01/01/1970 00:00:00").Ticks) / 10000000;
		secondsFromGameStart -= baseCharScript.secondsAtGameStart;
		
		System.TimeSpan t = System.TimeSpan.FromSeconds( secondsFromGameStart );
		
		string gameTime = string.Format("{0:D2}:{1:D2}:{2:D2}", 
		                                t.Hours, 
		                                t.Minutes, 
		                                t.Seconds, 
		                                t.Milliseconds);
		
		GUI.Label( new Rect( offset + (teamStatisticsBoxWidth/10.0f) * 7.0f, (teamStatisticsBoxHeight / 4.0f) + yOffset, (teamStatisticsBoxWidth/10.0f) * 3.0f, (teamStatisticsBoxHeight/10.0f) ), gameTime );
		
		GUI.skin.label.padding.left = 0;
		GUI.skin.label.padding.right = 0;
	}
	
	private void displayTeamStatistics()
	{
		if( teamStatisticsOffset < 10.0f )
		{
			teamStatisticsOffset += teamStatisticsBoxStep;
		}
		
		drawTeamStatisticsBox(teamStatisticsOffset);
	}
	
	private void hideTeamStatistics()
	{
		if( teamStatisticsOffset > -teamStatisticsBoxWidth )
		{
			teamStatisticsOffset -= teamStatisticsBoxStep;
			
			drawTeamStatisticsBox(teamStatisticsOffset);
		}
	}
	
	private void drawPersonalStatisticsBox(float offset)
	{
		GUI.skin.label.fontSize = (int) (Screen.height * 0.03f);
		GUI.Box ( new Rect( (personalStatisticsBoxWidth / 4.0f), Screen.height + offset, personalStatisticsBoxWidth, personalStatisticsBoxHeight ), "" );
	}
	
	private void displayPersonalStatistics()
	{
		if( personalStatisticsOffset > -personalStatisticsBoxHeight - 20.0f )
		{
			personalStatisticsOffset -= personalStatisticsBoxStep;
		}
		
		drawPersonalStatisticsBox (personalStatisticsOffset);
		
	}
	
	private void hidePersonalStatistics()
	{
		if( personalStatisticsOffset < personalStatisticsBoxHeight )
		{
			personalStatisticsOffset += personalStatisticsBoxStep;
			
			drawPersonalStatisticsBox (personalStatisticsOffset);
		}		
	}
	//endof mantkowicz
	
	public void Test() {
		Debug.Log ("Jestem testem!!!!!!!!");
	}
}
