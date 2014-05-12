using UnityEngine;
using System.Collections;

public class GrenadeThrow : MonoBehaviour {
	
	public Camera camera;
	public GameObject granadePref;
	private uint grenadesNumber;
	
	// Use this for initialization
	void Start () {
		grenadesNumber = 10005;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Grenade") ){
			if( grenadesNumber > 0 )
			{
				GameObject granade = Instantiate( granadePref ) as GameObject;
				granade.transform.position = camera.transform.position + camera.transform.forward;
				granade.GetComponent("Rigidbody").rigidbody.AddForce( camera.transform.forward*8000 );
				--grenadesNumber;
			}
			else{
				Debug.Log( "no granades left!" );
			}
		}
	}
}
