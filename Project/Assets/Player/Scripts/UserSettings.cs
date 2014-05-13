using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public class UserSettings  { //: MonoBehaviour {

	public Hashtable keys = new Hashtable();
	public ArrayList sortedKeys = new ArrayList();

	
	public UserSettings() {
		// tutaj trzeba to odczytac a jak nie ma plika z konfigiem
		// to wtedy defaulotwo, ale na razie stale defaultowo
		
		keys.Add("up", KeyCode.W);
		sortedKeys.Add ("up");
		keys.Add("down", KeyCode.S);
		sortedKeys.Add ("down");
		keys.Add("left", KeyCode.A);
		sortedKeys.Add ("left");
		keys.Add("right", KeyCode.D);
		sortedKeys.Add ("right");
		keys.Add("jump", KeyCode.Space);
		sortedKeys.Add ("jump");
		keys.Add("run", KeyCode.LeftShift);
		sortedKeys.Add ("run");
		keys.Add("crouch", KeyCode.C);
		sortedKeys.Add ("crouch");
		keys.Add("granade", KeyCode.G);
		sortedKeys.Add ("granade");
		
		//Debug.Log (keys ["w"]);
	}

	public bool GetKey(string name) {
		return Input.GetKey ((KeyCode)keys [name]);
	}

	public bool GetKeyDown(string name) {
		return Input.GetKeyDown ((KeyCode)keys [name]);
	}

	public void ChangeKey(string name, KeyCode toWhat) {
		keys[name] = toWhat;
	}

	void Start () {


	}

	
	// Update is called once per frame
	void Update () {
	
	}

	public void Save() {
		// pobieramy katalog z danymi aplikacji
		
		string path = Application.persistentDataPath; // czasami da sie tam zapisac
		// a czasami nie O_O (brak dostepu etc) nie wiem dlaczego, chyba jakis bug w unity :/

		Debug.Log (path);

		string fileName = path + "/" + "keyconfig.txt";
		Debug.Log (fileName);
		string[] content = new string[sortedKeys.Count];
		int i = 0;
		foreach (string name in sortedKeys) {
			string line = name + ", " + keys[name];
			content[i] = line;
			i++;
		}
		System.IO.File.WriteAllLines (fileName, content);

	}

	public void Load() {
		string path = Application.persistentDataPath;
		string fileName = path + "/" + "keyconfig.txt";

		string[] content = new string[sortedKeys.Count];

		if(File.Exists(fileName)) {
			content = System.IO.File.ReadAllLines(fileName);
		}
		else {
			return;
		}


		foreach (string line in content) {
			string lewo = Regex.Split(line, ",")[0].Trim();
			string prawo = Regex.Split(line, ",")[1].Trim();
			Debug.Log (lewo + "=" + prawo);
			keys[lewo] = System.Enum.Parse( typeof(KeyCode), prawo);
		}
	}
}
