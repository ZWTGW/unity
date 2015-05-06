using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary; 
using System.IO;


// pc - taki test w sumie czy można by nagrać demko i np. odtworzyć
// i w ten sposób zasymulować wielu graczy później
// dodać to do jakiegoś pustego gameobjecta najlepiej albo co

public class RecordAndPlayDemo : MonoBehaviour {

	/////////////// TE PIERDOŁY WYKLIKAĆ PO DODANIU SKRYPTU W GUI UNITY //////

	// obsluga klawiszy, tylko na jednym obiekcie powinno zostać tylko, na obiekcie gracza aktualnego
	// na pozostałych false
	public bool enableKeys = false;

	// jak null to odtwarzamy na pierwszym graczu pierdoły (ktory jest spawnowany automatycznie przez sieć)
	public GameObject playerObject = null;

	// jak enable keys jest na false, to to trzeba, automatycznie zacznie odtwarzac takie demko
	public string plikDemaBezRozszerzenia = ""; 
	// ten plik musi być w c:\Users\<NAZWA USERA>\AppData\LocalLow\DefaultCompany\Project 
	// sama nazwa tutaj

	/// /////////////////////////////////////////////////////////////////

	bool recording = false;
	bool playing = false;
	int currentFrame = 0;

	// jak sie tutaj cos doda to format plikow demek oczywiscie nie bedzie kompatybilny ze starym (chyba)
	[System.Serializable] 
	struct demoFrame { // klatka dema czy coś
		public float x, y, z; // pozycja
		public float rw, rx, ry, rz; // rotacja
		public int shoot;
	};

	List<demoFrame> demo = new List<demoFrame>();


	// Use this for initialization
	void Start () {
		if(!enableKeys) {
			LoadFromFile(plikDemaBezRozszerzenia);
		}

	}

	void recordFrame() {
		demoFrame tmp;
		GameObject player = GameObject.FindGameObjectsWithTag ("Player")[0];
		tmp.x = player.transform.position.x;
		tmp.y = player.transform.position.y;
		tmp.z = player.transform.position.z;
		tmp.rw = player.transform.rotation.w;
		tmp.rx = player.transform.rotation.x;
		tmp.ry = player.transform.rotation.y;
		tmp.rz = player.transform.rotation.z;

		tmp.shoot = 0;
		if(Input.GetMouseButtonDown(0)) {
			tmp.shoot = 1;
		}
		if (Input.GetMouseButtonUp (0)) {
			tmp.shoot = -1;
		}
		demo.Add(tmp); 
	}

	void playFrame() {
		// tutaj możnaby zmienić indeks i przesuwać innego typa
		GameObject player;

		if(enableKeys) player = GameObject.FindGameObjectsWithTag ("Player")[0];
		else player = playerObject;

		demoFrame tmp = demo[currentFrame];
		player.transform.position = new Vector3(tmp.x, tmp.y, tmp.z);
		player.transform.rotation = new Quaternion(tmp.rx, tmp.ry, tmp.rz, tmp.rw);

		if (tmp.shoot > 0) {
			player.GetComponent<Shooter>().StartShooting();
		}
		if (tmp.shoot < 0) {
			player.GetComponent<Shooter>().StopShooting();
		}
		// zaczynamy od poczatku jak dojdziemy do konca
		currentFrame++;
		if(currentFrame >= demo.Count) currentFrame = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if(enableKeys) {
			if( Input.GetKeyDown (KeyCode.O)) {
				recording = !recording;
				playing = false;
			}
			
			if( Input.GetKeyDown (KeyCode.P)) {
				recording = false;
				playing = !playing;
			}

			if( Input.GetKeyDown (KeyCode.K)) {
				recording = false;
				playing = false;
				SaveToFile();
			}

			if( Input.GetKeyDown (KeyCode.L)) {
				recording = false;
				playing = false;
				LoadFromFile();
			}
		}

		if(!enableKeys) {
			playFrame();
			return;
		}

		if(recording) {
			recordFrame();
		}
		else if(playing) {
			playFrame();
		}
	}

	void SaveToFile(string name = "demko") { // zapisujemy demko
		string filePath = Application.persistentDataPath + "/" + name + ".demko";
		if(File.Exists(filePath)) File.Delete(filePath); // wywalamy stary plik i sie o nic nie pytamy (!)
		BinaryFormatter bf = new BinaryFormatter();
		Debug.Log ("Zapisujemy demko do: " + filePath);
		FileStream file = File.Create (filePath);
		bf.Serialize(file, demo);
	}

	void LoadFromFile(string name = "demko") {
		string filePath = Application.persistentDataPath + "/" + name + ".demko";
		demo.Clear();
		if(!File.Exists(filePath)) {
			Debug.Log ("PLIK Z DEMKIEM " + name + " NIE ISTNIEJE!!111111111");
			return;
		}
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Open(filePath, FileMode.Open);
		demo = (List<demoFrame>)bf.Deserialize(file);
		Debug.Log("Załadowno demko z " + demo.Count + " klatkami"); // jak tu jest 0 a coś mamy to źle raczej

	}
}
