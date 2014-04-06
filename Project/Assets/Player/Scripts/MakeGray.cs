using UnityEngine;
using System.Collections;

public class MakeGray : MonoBehaviour {
	public Shader myShader;
	// Use this for initialization

	private float power = 0.84f;

	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void SetPower(float power) {
		this.power = power;
	}

	void OnGUI() {
		Shader.SetGlobalFloat ("PowerOfGray", power);
		if(enabled) camera.RenderWithShader (myShader, "");
	}

	public void test() {
		Debug.Log ("jestem testem");
	}
}
