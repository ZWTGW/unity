using UnityEngine;
using System.Collections;

public class Dzwieki : MonoBehaviour {

	public AudioSource jumpSound;
	public AudioSource stepSound;

	void Start () {

	}

	void Update () {
		if( Input.GetButtonDown("Jump") ) {
			//audio.Play ();
			playJump();
		}
		if ( Input.GetKey("w") || Input.GetKey("s") || Input.GetKey("d") || Input.GetKey("a") ) {
			playStep();
		}
		else{
			stopStep();
		}
	}

	public void playJump(){
		//audio.PlayOneShot(jumpSound);
		jumpSound.Play ();
	}

	public void playStep(){
		//audio.PlayOneShot (stepSound);
		if(stepSound.isPlaying == false){
			stepSound.Play();
		}
	}
	public void stopStep(){
		stepSound.Stop ();
	}
}
