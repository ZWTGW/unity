using UnityEngine;
using System.Collections;

public class fakeAnimation : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey ("v")) {
			gameObject.GetComponent<Animation>().Play("bazooka_dying");
			return;
		}
		if (Input.GetKey ("b")) {
			gameObject.GetComponent<Animation>().Play("bazooka_walking");
		}

	}
}
