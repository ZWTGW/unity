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
		print ("test test test");
		print ("druga linijka");
		Debug.Log ("to samo inaczej");
		print ("test test test");
		print ("druga linijka");
		Debug.Log ("to samo inaczej");
		print ("test test test");
		print ("druga linijka");
		Debug.Log ("to samo inaczej");
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.BackQuote)) { // tylda
						consoleShown = !consoleShown;
			GUI.FocusControl("poletekstowe");
		} 
	}

	void handleCommand(string polecenie) {
		polecenie.Split (' ');
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
		logs.Insert (0, tmp); // nowe wiadomosci na poczatku
	}

	void OnGUI() {
		Event e = Event.current;

		if (!consoleShown) return;
		if (e.keyCode == KeyCode.Return) {
			handleCommand(polecenie);
			polecenie = "";
		}

		GUI.skin.label.normal.textColor = new Color (255, 255, 255);
		GUI.skin.label.fontSize = 15;
		GUI.skin.label.alignment = TextAnchor.UpperLeft;
		GUI.skin.box.fontSize = 15;

		GUI.Box(new Rect(0,0,Screen.width,200), "------ CONSOLE ------");
		GUI.BeginGroup (new Rect (0, 0, Screen.width, 200));
		 
		int i = 0;
		foreach (LogItem item in logs) {
			GUI.Label (new Rect (15, i * 25 + 25, Screen.width, 25), item.message + " | " + item.stackTrace);
			++i;
		}
		GUI.EndGroup ();

		GUI.SetNextControlName ("poletekstowe");
		polecenie = GUI.TextField(new Rect(0, 200, Screen.width, 25) , polecenie);

	}
}
