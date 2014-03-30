// ze strony http://wiki.unity3d.com/index.php?title=RigidbodyFPSWalker
using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (CapsuleCollider))]

public class FPSControlsRigid : MonoBehaviour {

	private float crchSpeed = 11.0f; //predkosc kucania
	public float normalSpeed = 20.0f; //predkosc normalnego biegu
	private float runSpeed = 32.0f; //predkosc sprintu
	private float gravity = 97.0f; //grawitacja
	public float maxVelocityChange = 15.0f; //zmiana predkosci
	public bool canJump = true;
	private float inAirControl = 0.3f; //poruszanie sie w locie
	private float jumpHeight = 4.5f; //wysokosc skoku
	public float speed; //szybkosc aktualna, moze sie przydac w headbobber

	private bool grounded = false;
//	private bool crouch;
	private static float forwardSpeedModifier = 0.90f; //wolniejszy ruch przod tyl
	private static float sidesSpeedModifier = 0.85f; //wolniejszy strafe
	private static float jumpSpeedModifier = 0.95f;	//mniejsza predkosc po skoku

	public float xVel; //do odczytywania predkosc - tylko do testow

	//public GameObject player; //to sie jeszcze moze przydac 
	
	private Transform tr;
	private float dist; // distance to ground
	private CapsuleCollider capsule; //na razie nie ma uzycia, ale moze sie przydac chocby do kontroli kucania
	private int slopeLimit = 30;

	UserSettings us;
	

	void Start ()
	{


		tr = transform;
		//player = GameObject.Find("Player");
		dist = 1.0f; // calculate distance to ground
	}

	void Awake () {


		rigidbody.freezeRotation = true;
		rigidbody.useGravity = true;
		capsule = GetComponent<CapsuleCollider>();
	}
	
	void FixedUpdate () {
		GraGUI gg = GameObject.Find ("Player").GetComponent<GraGUI> ();
		us = gg.us;

		float vScale = 1.5f;
		speed = normalSpeed;
		xVel = rigidbody.velocity.magnitude;
		if (grounded) {
			canJump = true;
			// Calculate how fast we should be moving
			if (us.GetKey("run") && us.GetKey("up"))
			{
				speed = runSpeed;
			}

			//Debug.Log(us.GetKey("crouch"));

			if (us.GetKey("crouch"))
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
			velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange)*sidesSpeedModifier;
			velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange)*forwardSpeedModifier; //
			velocityChange.y = 0;
			rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
			//

			// Jump

			if (canJump && us.GetKey("jump")) {
				rigidbody.velocity = new Vector3(velocity.x*jumpSpeedModifier, CalculateJumpVerticalSpeed(), velocity.z*jumpSpeedModifier);
				canJump = false;
			}

			//kucanie - do poprawki, glowa nie moze "kucac", a obecnie kuca. W ogole glowa moze byc do usuniecia, tylko wtedy nalezaloby zrobic osobne hitboxy glowy
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


	void TrackGrounded(Collision col)
	{
		float minimumHeight = 15f;
		//ContactPoint c;
		foreach (ContactPoint c in col.contacts)
		{
			if (c.point.y < minimumHeight)
			{
				float slopeAngle = (-c.normal.y + 1.0f) * 100.0f;
				if (slopeAngle < slopeLimit)
				{
					// grounded is used in the FixedUpdate callback
					grounded = true;
				}
			}
		}
	}
	
	void OnCollisionStay(Collision col)
	{
		TrackGrounded(col);
	}
	
	void OnCollisionEnter(Collision col)
	{
		TrackGrounded(col);
	}

	float CalculateJumpVerticalSpeed () {
		// From the jump height and gravity we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt(2 * jumpHeight * gravity);
	}
}