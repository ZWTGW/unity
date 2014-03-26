using UnityEngine;
using System.Collections;


public class GraGUI : MonoBehaviour {

	private BaseCharacter baseCharScript;
	private bool showInGameMenu = false; // czy pokazac menu?
	private float sliderValue = 1f;

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
		}
	}

	void Awake() {
		baseCharScript = GetComponent<BaseCharacter>();
	}

	private void HUD() {
		// te rzeczy trzeba by pozniej pobrac z jakiegos obiektu player czy cos takiego
		// na razie dla testu stale
		int hp = baseCharScript.currHP;
		int maxHp = baseCharScript.maxHP;
		int ammo = 15;
		int maxAmmo = 20;
		
		// ustawiamy skórkę gui
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
		
		// taki orb z hp ala diablo czy coś (tylko gorszy)
		Texture2D orb1 = (Texture2D)Resources.Load ("hpOrb1");
		Texture2D orb2 = (Texture2D)Resources.Load ("hpOrb2");
		Rect pozycjaOrba = new Rect (5, Screen.height - 180, orb1.width, orb1.height);
		// rysujemy caly orb zajety
		GUI.DrawTexture (pozycjaOrba, orb1);
		
		// do rysowania fragmentu textury
		// trick z http://answers.unity3d.com/questions/160560/select-part-of-the-texture-in-guidrawtexture.html
		Rect textureCrop = new Rect( 0.0f, 0.0f, 1f, 1f - (float)hp / (float)maxHp );
		Vector2 position = new Vector2( pozycjaOrba.x, pozycjaOrba.y );
		
		// fragment orba wypelnionego czy cos
		GUI.BeginGroup( new Rect( position.x, position.y, orb2.width * textureCrop.width, orb2.height * textureCrop.height ) );
		GUI.DrawTexture( new Rect( -orb2.width * textureCrop.x, -orb2.height * textureCrop.y, orb2.width, orb2.height ), orb2 );
		GUI.EndGroup();
	}

	private void InGameMenu() {
		// wymiary calego menu
		int w = 500;
		int h = 370;

		GUI.BeginGroup(new Rect((Screen.width - w)/2,(Screen.height - h)/2, w, h));
		GUI.skin.box.fontSize = 40;
		GUI.skin.label.fontSize = 35;
		GUI.skin.button.fontSize = 35;
		GUI.skin.label.alignment = TextAnchor.UpperLeft;
		GUI.skin.horizontalSlider.stretchHeight = true;
		GUI.Box(new Rect(0,0,w,h), "MENU");
		if (GUI.Button (new Rect (15, 50, w * 0.95f, 60), "BACK TO GAME")) {
			showInGameMenu = false;
		}
		if (GUI.Button (new Rect (15, h - 70, w * 0.95f, 60), "QUIT TO DESKTOP")) {
			Application.Quit();
		}

		GUI.Label (new Rect (15, 50 + 90, w * 0.95f, 60), "VOLUME");
		sliderValue = GUI.HorizontalSlider (new Rect (15, 50 + 90 + 50, w * 0.95f, 60), sliderValue, 0, 1);
		GUI.EndGroup();

		AudioListener.volume = sliderValue;

	}

	void OnGUI() {
		HUD();
		if (showInGameMenu) InGameMenu ();
	}
	
}
