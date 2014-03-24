using UnityEngine;
using System.Collections;

public class Dzwieki : MonoBehaviour {

	public AudioClip jumpSound;
	void Start () {

	}

	void Update () {
		if( Input.GetButtonDown("Jump") ) {
			//audio.Play ();
			playJump();
		}
	}

	public void playJump(){
		audio.PlayOneShot(jumpSound);
	}
}
