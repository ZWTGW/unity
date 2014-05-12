using UnityEngine;
using System.Collections;

public class ExplosionController : MonoBehaviour {

	void Awake()
	{
		Destroy( gameObject, 30);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
