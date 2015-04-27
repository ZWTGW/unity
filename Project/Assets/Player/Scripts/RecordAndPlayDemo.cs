using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// pc - taki test w sumie czy można by nagrać demko i np. odtworzyć
// i w ten sposób zasymulować wielu graczy później
// dodać to do jakiegoś pustego gameobjecta najlepiej albo co

public class RecordAndPlayDemo : MonoBehaviour {

	bool recording = false;
	bool playing = false;
	int currentFrame = 0;

	struct demoFrame { // klatka dema czy coś
		public float x, y, z;
	};

	List<demoFrame> demo = new List<demoFrame>();


	// Use this for initialization
	void Start () {


	}

	void recordFrame() {
		demoFrame tmp;
		GameObject player = GameObject.FindGameObjectsWithTag ("Player")[0];
		tmp.x = player.transform.position.x;
		tmp.y = player.transform.position.y;
		tmp.z = player.transform.position.z;
		demo.Add(tmp);
	}

	void playFrame() {
		// tutaj możnaby zmienić indeks i przesuwać innego typa
		GameObject player = GameObject.FindGameObjectsWithTag ("Player")[0];
		demoFrame tmp = demo[currentFrame];
		player.transform.position = new Vector3(tmp.x, tmp.y, tmp.z);



		// zaczynamy od poczatku jak dojdziemy do konca
		currentFrame++;
		if(currentFrame >= demo.Count) currentFrame = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if( Input.GetKeyDown (KeyCode.O)) {
			recording = !recording;
			playing = false;
		}
		
		if( Input.GetKeyDown (KeyCode.P)) {
			recording = false;
			playing = !playing;
		}

		if(recording) {
			recordFrame();
		}
		else if(playing) {
			playFrame();
		}
	}
}
