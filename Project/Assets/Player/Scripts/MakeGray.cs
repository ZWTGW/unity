using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MakeGray : MonoBehaviour {
	public Shader myShader;
	// Use this for initialization

	private float power = 0.84f;

	ArrayList tmp = null;
	ArrayList tmp2 = null;

	public bool grayEnabled = false;

	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void SetPower(float power) {
		this.power = power;
	}

	void OnGUI() {

		if(grayEnabled) {
			GameObject[] gos = GameObject.FindGameObjectsWithTag ("CanBeGray");
			bool pleaseSave = false;
			if(tmp == null && tmp2 == null) {
				tmp = new ArrayList ();
				tmp2 = new ArrayList ();
				pleaseSave = true;
			}
			foreach (GameObject g in gos) {
				// musimy gdzies zapisac stare materialy aby je pozniej odtworzyc
				// w arraylist bo posortowane i zawsze ten sam indeks bedzie temu samemu materialowi
				// odpowiadal
				if(pleaseSave) {
					tmp.Add (g);
					tmp2.Add (g.renderer.material);
				}

				Shader.SetGlobalFloat ("PowerOfGray", power);
				g.renderer.material = new Material(myShader);
			}

			camera.Render ();
		}

		else {
			if(tmp != null && tmp2 != null) {
				for (int i = 0; i < tmp.Count; ++i) {
					// odzyskujemy materialy
					GameObject g = (GameObject) tmp[i];
					Material m = (Material) tmp2[i];
					g.renderer.material = m;
				} 
				
				tmp = null;
				tmp2 = null;
			}

		}


	}



	public void test() {
		Debug.Log ("jestem testem");
	}
}
