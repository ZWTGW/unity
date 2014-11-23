using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Konsola : MonoBehaviour {

	struct LogItem {
		public string message;
		public string stackTrace;
		public LogType type;
	}

	List<LogItem> logs = new List<LogItem>();
	bool consoleShown = false;

	string polecenie = "";

	// Use this for initialization
	void Start () {
		// testy - to powinno sie wyswietlic w naszej konsoli, ale tez w wbudowanej konsoli unity
		print ("------- GAME STARTED -------\n");
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.BackQuote)) { // tylda
			consoleShown = !consoleShown;
			polecenie = "";
		} 
	}

	void handleCommand(string polecenie) {
		GameObject playa;

		if (polecenie.StartsWith ("/")) polecenie = polecenie.Substring (1);
		string[] s = polecenie.Split (' ');
		if (s.Length < 1) return; // nie mamy nic
		switch (s [0]) {
			case "cls":
			case "clear":	
				logs.Clear();
				break;
			case "help":
				print("/move X Y Z - move player by X Y Z");
				print("/pos - get player position \n");
				print("/help - this \n");
				print("/cls - clear screen \n");
				print("Avaliable commands: \n");

				break;

			case "move":
				if(s.Length != 4) {
					print("Invalid parameters for move :(");
					break;
				}
				float x = 0;
				float y = 0;
				float z = 0;
				try {
					x = float.Parse(s[1]);
					y = float.Parse(s[2]);
					z = float.Parse(s[3]);
				}
				catch(System.FormatException e) {
					print("Invalid parameters for move :(");
					return;
				}	

				playa = GameObject.Find ("Player(Clone)");	
				playa.transform.Translate(x, y, z);
				break;

			case "pos":
				playa = GameObject.Find ("Player(Clone)");
				print("Z: " + playa.transform.position.z);
				print("Y: " + playa.transform.position.y);
				print("X: " + playa.transform.position.x);
			break;

			default:
				print("Invalid command '" + s[0] + "'. Try 'help'.");
				break;
		}
	}

	void OnEnable ()
	{
		// teraz wszystko z wbudowanej konsolki unity bedzie szlo tez do funkcji log ktora jest ponizej
		Application.RegisterLogCallback(Log);
	}
	void OnDisable ()
	{
		Application.RegisterLogCallback(null);
	}

	void Log (string message, string stackTrace, LogType type) {
		LogItem tmp = new LogItem();
		tmp.message = message;
		tmp.stackTrace = stackTrace;
		tmp.type = type;


		// dwie opcje w sumie
		//logs.Add(tmp); // nowe wiadomosci na koncu
		if(logs.Count >= 1) { // nie wrzucamy wiadomoci jesli mamy juz taka wiadomosc ostatnia
			// wrzucilem to bo duzo bledow od soundow co sekunde milion i nic nie widac w konsoli :)
			if(logs[0].message != message) {
				logs.Insert (0, tmp); // nowe wiadomosci na poczatku
			}
		}
		else {
			logs.Insert (0, tmp); // nowe wiadomosci na poczatku
		}


	}


	void OnGUI() {
		Event e = Event.current;

		if (!consoleShown) return;
		if (e.type == EventType.KeyUp) {
			if (e.keyCode == KeyCode.Return) {
					handleCommand (polecenie);
					polecenie = "";
			} else if (e.keyCode == KeyCode.Escape) {
					consoleShown = false;
					polecenie = "";
			}
		}

		GUI.skin.label.normal.textColor = new Color (255, 255, 255);
		GUI.skin.label.fontSize = 15;
		GUI.skin.label.alignment = TextAnchor.UpperLeft;
		GUI.skin.box.fontSize = 15;

		GUI.Box(new Rect(0,0,Screen.width,200), "------ CONSOLE ------");
		GUI.BeginGroup (new Rect (0, 0, Screen.width, 200));
		 
		int i = 0;
		foreach (LogItem item in logs) {
			GUI.Label (new Rect (15, i * 25 + 25, Screen.width, 25), item.message );// + " | " + item.stackTrace);
			++i;
		}
		GUI.EndGroup ();

		GUI.SetNextControlName ("poletekstowe");
		if (polecenie.StartsWith ("`")) polecenie = polecenie.Substring (1);
		polecenie = GUI.TextField(new Rect(0, 200, Screen.width, 25) , polecenie);

		GUI.FocusControl("poletekstowe");

	}
}
