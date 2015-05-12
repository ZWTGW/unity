using UnityEngine;
using System.Collections;

public class GrenadeThrow : MonoBehaviour {
	
	public Camera camera;
	public GameObject granadePref;
	private uint grenadesNumber;
	public GameObject arms;
	private float time;

	private bool cooldown;

	// Use this for initialization
	void Start () {
		grenadesNumber = 10005;
		cooldown = true;
	}
	
	// Update is called once per frame
	void Update () {
		}
	public void Throw (){
		if( grenadesNumber > 0 && cooldown)
		{
			arms.GetComponent<Animation> ().Play ("granade");
			time = arms.GetComponent<Animation> ().GetClip("granade").length;
			StartCoroutine(ThrowGranade());
			cooldown = false;
		}
		else{
			Debug.Log( "no granades left!" );
		}

	}

	IEnumerator ThrowGranade() {
		yield return new WaitForSeconds(time - 0.6f);
		cooldown = true;
		GameObject granade = Instantiate( granadePref ) as GameObject;
		granade.transform.position = camera.transform.position + camera.transform.forward + new Vector3(-1, 1, 0);
		granade.GetComponent("Rigidbody").GetComponent<Rigidbody>().AddForce( camera.transform.forward*8000 );
		--grenadesNumber;
	}
}
