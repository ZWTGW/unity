using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UserSettings  : MonoBehaviour {
	//public ArrayList keys = new ArrayList();
	//private Dictonary<string, KeyCode> keys = new Dictonary<string, KeyCode>();
	private Hashtable keys = new Hashtable();


	public UserSettings() {
		// tutaj trzeba to odczytac a jak nie ma plika z konfigiem
		// to wtedy defaulotwo, ale na razie stale defaultowo
		
		keys.Add("up", KeyCode.W);
		keys.Add("down", KeyCode.S);
		keys.Add("left", KeyCode.A);
		keys.Add("right", KeyCode.D);
		keys.Add("jump", KeyCode.Space);
		keys.Add("run", KeyCode.LeftShift);
		keys.Add("crouch", KeyCode.C);
		
		//Debug.Log (keys ["w"]);
	}

	public bool GetKey(string name) {
		return Input.GetKey ((KeyCode)keys [name]);
	}

	void Start () {


	}

	
	// Update is called once per frame
	void Update () {
	
	}
}
