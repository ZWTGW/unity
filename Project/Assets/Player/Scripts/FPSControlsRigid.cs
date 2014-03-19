// ze strony http://wiki.unity3d.com/index.php?title=RigidbodyFPSWalker
using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (CapsuleCollider))]

public class FPSControlsRigid : MonoBehaviour {

	private float crchSpeed = 7.0f; //predkosc kucania
	private float normalSpeed = 14.0f; //predkosc normalnego biegu
	private float runSpeed = 21.0f; //predkosc sprintu
	private float gravity = 10.0f; //grawitacja
	private float maxVelocityChange = 12.0f; //zmiana predkosci
	public bool canJump = true;
	private float inAirControl = 0.25f; //poruszanie sie w locie
	private float jumpHeight = 4.5f; //wysokosc skoku
	public float speed; //szybkosc aktualna, moze sie przydac w headbobber

	private bool grounded = false;
//	private bool crouch;
	private static float SpeedModifier = 0.1f; //wolniejszy strafe
	private static float jumpSpeedModifier = 0.8f;	//wolniejsze poruszanie sie w locie

	public GameObject player; //sluzy do translacji kucania

	private CharacterMotor chMotor;
	private Transform tr;
	private float dist; // distance to ground
	//	public bool up = Input.GetKey('w' key);


	void Start ()
	{
//		chMotor = GetComponent<CharacterMotor>();
		tr = transform;
		player = GameObject.Find("Player");
		dist = 1.0f; // calculate distance to ground
	}

	void Awake () {
		rigidbody.freezeRotation = true;
		rigidbody.useGravity = true;
	}
	
	void FixedUpdate () {
		float vScale = 1.5f;
//		float speed = normalSpeed;
		speed = normalSpeed;
		if (grounded) {
			canJump = true;
			// Calculate how fast we should be moving

			if (Input.GetKey("left shift") && Input.GetKey("w"))
			{
				speed = runSpeed;
			}
			
			if (Input.GetKey("c"))
			{ // press C to crouch
//				crouch = true;
				canJump = false;
				vScale = 0.85f;
				speed = crchSpeed; // slow down when crouching

			}

			Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			targetVelocity = transform.TransformDirection(targetVelocity);
			targetVelocity *= speed;


			// Apply a force that attempts to reach our target velocity
			Vector3 velocity = rigidbody.velocity;
			Vector3 velocityChange = (targetVelocity - velocity);
			velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
			velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange)*SpeedModifier; //wolniejsze ruszanie sie przy strafe
			velocityChange.y = 0;
			rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
			//

			// Jump
			if (canJump && Input.GetButton("Jump")) {
				rigidbody.velocity = new Vector3(velocity.x*jumpSpeedModifier, CalculateJumpVerticalSpeed(), velocity.z*jumpSpeedModifier);

			}

			//kucanie
			float ultScale = tr.localScale.y; // crouch/stand up smoothly
			
			Vector3 tmpScale = tr.localScale;
			Vector3 tmpPosition = tr.position;
			
			tmpScale.y = Mathf.Lerp(tr.localScale.y, vScale, 20 * Time.deltaTime);
			tr.localScale = tmpScale;
			
			tmpPosition.y += dist * (tr.localScale.y - ultScale); // fix vertical position
			tr.position = tmpPosition;
			//

			grounded = false;
		}
		else
			
		{
			
			// poruszanie sie w locie
			
			Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));	
			targetVelocity = transform.TransformDirection(targetVelocity) * inAirControl;
			rigidbody.AddForce(targetVelocity, ForceMode.VelocityChange);
			
		}		
		// We apply gravity manually for more tuning control
		rigidbody.AddForce(new Vector3 (0, -gravity * rigidbody.mass, 0));
		

	}
	
	void OnCollisionStay () {
		grounded = true;    
	}

/*	function crouch() {
		this.GetComponent(BoxCollider).size -= Vector3(0,crouchDeltaHeight, 0);
		this.GetComponent(BoxCollider).center -= Vector3(0,crouchDeltaHeight/2, 0);
		crouching = true;
	}	*/

	float CalculateJumpVerticalSpeed () {
		// From the jump height and gravity we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt(2 * jumpHeight * gravity);
	}
}