using UnityEngine;
using System.Collections;


public class GraGUI : MonoBehaviour{

	private BaseCharacter baseCharScript;



	// tu ok bo instancja nowa moze byc
	// ale czy na pewno za kazdym razem chcemy nowa instancje obiektu basecharacter? nie sądzę
	public UserSettings us = new UserSettings();

	private bool showInGameMenu = false; // czy pokazac menu?
	private float sliderValue = 1f;
	enum MenuStates { MAIN, OPTIONS, SOUND, REDEFINE };
	MenuStates menuState = MenuStates.MAIN;
	string keyChanging = "";

	// Use this for initialization
	void Start () {

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

	}

	void Awake() {
		us.Load ();
		baseCharScript = GetComponent<BaseCharacter>();
	}

	private void HUD() {
		//camera.RenderWithShader(
		// te rzeczy trzeba by pozniej pobrac z jakiegos obiektu player czy cos takiego
		// na razie dla testu stale
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
		if (hp >= 25) mg.enabled = false; 
		else mg.enabled = true;
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
		default:
		case MenuStates.MAIN:
			GUI.Box(new Rect(0,0,w,h), "MENU");
			if (GUI.Button (new Rect (15, 50, w * 0.95f, 60), "BACK TO GAME")) {
				showInGameMenu = false;
			}

			if (GUI.Button (new Rect (15, 50 + 60 + 60, w * 0.95f, 60), "OPTIONS")) {
				menuState = MenuStates.OPTIONS;
			}

			if (GUI.Button (new Rect (15, h - 70, w * 0.95f, 60), "QUIT TO DESKTOP")) {
				Application.Quit();
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

			int i = 0; // offset przy rysowaniu

			ArrayList sortedKeys = new ArrayList();

			foreach(string name in us.sortedKeys) { // kiedys bylo us.keys.Keys ale hashtable nie są po kolei, a pasowaloby zeby klawisze do sterowania byly na poczatku
				//Debug.Log(name + " " + us.keys[name]);
				GUI.Label(new Rect(15, 50 + i, w * 0.95f, 30), name);
				GUI.Label(new Rect(100, 50 + i, w * 0.95f, 30), us.keys[name].ToString());

				if(name == keyChanging) { // zmieniamy klawisz ktory aktualnie wyswietlamy
					if(GUI.Button(new Rect (200, 50 + i, 280, 30), "PRESS KEY")) {
						keyChanging = "";
					}
				}
				else { // nie zmieniamy
					if(GUI.Button(new Rect (200, 50 + i, 280, 30), "CHANGE")) {
						keyChanging = name;
					}
				}
				i += 32;
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
		HUD();
		if (showInGameMenu) InGameMenu ();
	}

	public void Test() {
		Debug.Log ("Jestem testem!!!!!!!!");
	}
}
