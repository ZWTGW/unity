//szablon ze strony http://wiki.unity3d.com/index.php?title=RigidbodyFPSWalker
using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (CapsuleCollider))]

public class FPSControlsRigid : BaseCharacter { //NIE WIEM CZY TO JEST SLUSZNY SPOSOB BY TO ZROBIC :(... ale dziala :)
	
	private int crchSpeed = 11; //predkosc kucania
	private int normalSpeed = 22; //predkosc normalnego biegu
	private int runSpeed = 34; //predkosc sprintu
	private int tiredSpeed = 14; //predkosc po zmeczeniu sie
	
	private float gravity = 97.0f; //grawitacja
	private int maxVelocityChange = 15; //zmiana predkosci
	private bool canJump = true;
	private float inAirControl = 0.3f; //poruszanie sie w locie
	private float jumpHeight = 4.5f; //wysokosc skoku
	public float speed; //szybkosc aktualna, moze sie przydac w headbobber
	public bool canSprint = true;
	public int restTime=100;
	
	private bool grounded = false;
	//	private bool crouch;
	private static float forwardSpeedModifier = 0.90f; //wolniejszy ruch przod tyl
	private static float sidesSpeedModifier = 0.85f; //wolniejszy strafe
	private static float jumpSpeedModifier = 0.95f;	//mniejsza predkosc po skoku
	
	public float xVel; //do odczytywania predkosc - tylko do testow
	public float yVel; //jw
	public float falldmg; //obrazenia od upadku
	public float alpha;
	public bool blood = false;
	public GUIStyle BloodSplat;
//	public Texture2D textureToDisplay;
	public Camera Transform;
//	GameObject Player;
//	GameObject HeadCube;


	private Transform tr;
	private float dist; // distance to ground
	private int slopeLimit = 30; //na jakie pochylosci mozna wchodzic
	
	UserSettings us;
	void Start ()
	{
		tr = transform;
		dist = 1.0f; // calculate distance to ground
		alpha = 0f;
	}
	
	void Awake () {
		rigidbody.freezeRotation = true;
		rigidbody.useGravity = true;
		
	}
	void OnGUI(){
		GUI.color = new Color (255, 0, 0, alpha);
		GUI.Label(new Rect(0,0, Screen.width, Screen.height), "", BloodSplat);
		GUI.color = new Color (255, 255, 255, 0);
	}

	void keyboardUpdate()
	{
		GraGUI gg = gameObject.GetComponent<GraGUI> ();
		Headbobber cbob = GameObject.Find("PlayerCam").GetComponent<Headbobber> ();
		
		us = gg.us;
		canJump = true;
		float vScale = 1.5f;
		speed = normalSpeed;
		xVel = rigidbody.velocity.magnitude;
		yVel = rigidbody.velocity.y;
		if (grounded) {
			
			//USTAWIENIA STAMINY
			if (stamina>1 && restTime>=100){
				speed=normalSpeed;
				canSprint=true;
			}
			
			else if (stamina<15) {
				speed=tiredSpeed;
				canSprint=false;
				restTime=1;
			}
			
			
			if (rigidbody.velocity.magnitude<3 && stamina<=100){
				stamina+=0.5f;
				if (restTime<100) restTime+=1;
			}
			else if (speed==normalSpeed && stamina<=100){
				stamina+=0.025f;
				if (restTime<100) restTime+=1;
			}
			else if (speed==crchSpeed && stamina<=100){
				stamina+=0.2f;
				if (restTime<100) restTime+=1;
			}
			else if (speed==tiredSpeed && stamina<=100){
				stamina+=0.15f;
				if (restTime<100) restTime+=1;
			}
			//
			//WYCHYLANIE SIE (do poprawki)
			if(Input.GetKeyDown("q")) {
				cbob.lean=-3;
				
			}
			if(Input.GetKeyDown("e")) {
				cbob.lean=3;
				
			}
			if(Input.GetKeyDown("b")) {
				cbob.lean=0;
				
			}
			//SPRINT
			if (us.GetKey("run") && us.GetKey("up") && canSprint == true)
				
			{
				speed = runSpeed; //szybkosc poruszania sie w biegu
				stamina-=0.8f;
				
			}
			//
			
			//KUCANIE
			if (us.GetKey("crouch"))
			{ 
				//				crouch = true;
				canJump = false;
				vScale = 0.85f;
				speed = crchSpeed; //szybkosc poruszania sie w przykucnieciu
				
			}
			//
			
			//BIEG
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
			//kucanie - do poprawki, glowa nie moze "kucac", a obecnie kuca. W ogole glowa moze byc do usuniecia, tylko wtedy nalezaloby zrobic osobne hitboxy glowy
			float ultScale = tr.localScale.y; // crouch/stand up smoothly
			
			Vector3 tmpScale = tr.localScale;
			Vector3 tmpPosition = tr.position;
			
			tmpScale.y = Mathf.Lerp(tr.localScale.y, vScale, 20 * Time.deltaTime);
			tr.localScale = tmpScale;
			
			tmpPosition.y += dist * (tr.localScale.y - ultScale); // fix vertical position
			tr.position = tmpPosition;
			//
			//
			//SKOK
			if (canJump && us.GetKey("jump")) {
				rigidbody.velocity = new Vector3(velocity.x*jumpSpeedModifier, CalculateJumpVerticalSpeed(), velocity.z*jumpSpeedModifier);
				canJump = false;
			}
			
			//UPADEK z duzej wysokosci
			if (falldmg<-50)
			{
				//Debug.Log(falldmg*0.35);
				falldmg*=-0.35f;
				vScale = 0.8f;
				//	float asd = cbob.midpoint;
				getDmg((int)falldmg);
				//GUI.color.a=1f; //
				alpha=0.5f;
				blood=true;
				cbob.midpoint=1.1f;
				
				//	Debug.Log(asd);
				//	cbob.midpoint=1; ruch kamery przy upadku
				falldmg=0;
				
			}
			
			//usuwanie krwi z ekranu
			if (blood == true && alpha >0){
				alpha -= Time.deltaTime/4;
				if (cbob.midpoint <2.0f){cbob.midpoint += Time.deltaTime*1.5f;}
				if (cbob.midpoint >2.0f) {cbob.midpoint = 2.0f;}
				if (alpha <=0){ blood = false;}
				
			}
			//
			
			
			
			
			//
			grounded = false;
		}
		else
			
		{	//upadek
			if(rigidbody.velocity.y < -50.0)
			{	falldmg=rigidbody.velocity.y;
			}
			
			
			// poruszanie sie w locie
			
			Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));	
			targetVelocity = transform.TransformDirection(targetVelocity) * inAirControl;
			rigidbody.AddForce(targetVelocity, ForceMode.VelocityChange);
			
		}		
		// We apply gravity manually for more tuning control
		rigidbody.AddForce(new Vector3 (0, -gravity * rigidbody.mass, 0));
	}

	void FixedUpdate () 
	{
		GameObject cam = transform.FindChild ("PlayerCam").gameObject;
		GameObject hcube = transform.FindChild ("HeadCube").gameObject;


		if (!networkView.isMine) {
			cam.SetActive (false);
		} else {
			keyboardUpdate();
			hcube.renderer.enabled = false;
		}
	

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
	
	void transformCam(int doWartosci){

	}
	public void getDmg(int amount){
		currHP-=amount;
		
		
	}
	
}

