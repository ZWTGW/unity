using UnityEngine;
using System.Collections;

public class Headbobber: MonoBehaviour
{

	 
	//private FPSControlsRigid FPSScript;
	private float timer = 0.0f;
	public float bobbingSpeed = 0.22f; //zamiast tego mozna korzystac z parametru, ktorym jest aktualna predkosc
	public float bobbingAmount = 0.4f; 
	public float midpoint = 2.0f;
	//private float param;



	void Awake() {
		//FPSScript = GetComponent<FPSControlsRigid>();
	}

	void Update () {
		//GameObject player = GameObject.Find ("Player");
		//param = player.rigidbody.velocity.magnitude*0.025f;
		//Debug.Log(param);
		float waveslice = 0.0f;
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");
		
		Vector3 cSharpConversion = transform.localPosition;
		
		if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0) {
			timer = 0.0f;
		}
		else {
			waveslice = Mathf.Sin(timer);
			timer = timer + bobbingSpeed; //speed
			if (timer > Mathf.PI * 2) {
				timer = timer - (Mathf.PI * 2);
			}
		}
		if (waveslice != 0) {
			float translateChange = waveslice * bobbingAmount; //amount
			float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
			totalAxes = Mathf.Clamp (totalAxes, 0.0f, 1.0f);
			translateChange = totalAxes * translateChange;
			cSharpConversion.y = midpoint + translateChange;

		}
		else {
			cSharpConversion.y = midpoint;
		}
		
		transform.localPosition = cSharpConversion;
	}
	
	
	
}