using UnityEngine;
using System.Collections;

public class MakeGray : MonoBehaviour {
	public Shader myShader;
	// Use this for initialization

	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnGUI() {
		if(enabled) camera.RenderWithShader (myShader, "");
	}

	public void test() {
		Debug.Log ("jestem testem");
	}
}
