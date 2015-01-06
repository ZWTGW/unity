//szablon ze strony http://wiki.unity3d.com/index.php?title=RigidbodyFPSWalker
using UnityEngine;
using System.Collections;
using System.Timers;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (CapsuleCollider))]

public class FPSControlsRigid : BaseCharacter { //NIE WIEM CZY TO JEST SLUSZNY SPOSOB BY TO ZROBIC :(... ale dziala :)
	
	private int crchSpeed = 12; //predkosc kucania
	private int normalSpeed = 23; //predkosc normalnego biegu
	private int runSpeed = 34; //predkosc sprintu
	private int tiredSpeed = 15; //predkosc po zmeczeniu sie
	
	private float gravity = 93.0f; //grawitacja
	private int maxVelocityChange = 15; //zmiana predkosci
	private bool canJump = true;
	public bool padjump = false; //zmienna do kontrolowania tego czy skoczylismy przy uzyciu jumppada czy normalnie - dodalem to po to by miec wieksza kotrole horyzontalna po skoku na jumppadzie niz po skoku zwyklym
	private float inAirControl = 0.5f; //poruszanie sie w locie
	private float jpadInAirControl = 0.8f; //poruszanie sie w locie - jumppad
	private float jumpHeight = 5.4f; //wysokosc skoku
	public float speed; //szybkosc aktualna, moze sie przydac w headbobber
	public bool canSprint = true;
	public int restTime=100;

	public float timer = 1.5f; //timer na razie oblicza czas od upadku do ustawienia zmiennej jumppadowej

	public bool grounded = false;
	private static float forwardSpeedModifier = 0.90f; //wolniejszy ruch przod tyl
	private static float sidesSpeedModifier = 0.85f; //wolniejszy strafe
	private static float jumpSpeedModifier = 0.95f;	//mniejsza predkosc po skoku
	private static float jumpPadSpeedModifier = 1.1f;	//mniejsza predkosc po skoku
	
	public float xVel; //do odczytywania predkosc - tylko do testow
	public float yVel; //jw
	public float falldmg; //obrazenia od upadku
	public float alpha;
	public bool blood = false;
	public GUIStyle BloodSplat;

	private Transform tr;
	private float dist; // distance to ground
	private int slopeLimit = 30; //na jakie pochylosci mozna wchodzic

	public AudioSource source;
	public AudioClip stepSound;
	public AudioClip fallSound;
	public AudioClip teleSound;
	public AudioClip jump_landSound;
	public AudioClip jump_startSound;

	public float footstepDelay = 0.6f;
	public float stepVol = 0.6f;
	private float nextFootstep = 0;

	UserSettings us;

	//PC:
	private bool canUseTeleport = true;
	private float teleportOpacity = 0.0f;
	Timer timerTeleport = new Timer(2000.0);
	Timer timerTeleportAnim = new Timer(100.0);
	Vector3 teleportPos = Vector3.zero;

	private static CapsuleCollider bodyCollider;
	private static Texture2D _staticRectTexture;
	private static GUIStyle _staticRectStyle;
	
	// Note that this function is only meant to be called from OnGUI() functions.
	public static void GUIDrawRect( Rect position, Color color )
	{
		if( _staticRectTexture == null )
		{
			_staticRectTexture = new Texture2D( 1, 1 );
		}
		
		if( _staticRectStyle == null )
		{
			_staticRectStyle = new GUIStyle();
		}
		
		_staticRectTexture.SetPixel( 0, 0, color );
		_staticRectTexture.Apply();
		
		_staticRectStyle.normal.background = _staticRectTexture;
		
		GUI.Box( position, GUIContent.none, _staticRectStyle );
		
		
	}

	void Start ()
	{
		tr = transform;
		dist = 1.0f; // calculate distance to ground
		alpha = 0f;

		CapsuleCollider bodyCollider = transform.GetComponent<CapsuleCollider>();

		source = GetComponent<AudioSource>();

		timerTeleport.Elapsed += OnTimedEvent;
		timerTeleport.Enabled = true;

		timerTeleportAnim.Elapsed += TeleportAnimTimerEvent;
		timerTeleportAnim.Enabled = false;
	}

	private void TeleportAnimTimerEvent(object source, ElapsedEventArgs e) {
		teleportOpacity -= 0.05f;
		if(teleportOpacity < 0.0f) teleportOpacity = 0.0f;
	}

	private void OnTimedEvent(object source, ElapsedEventArgs e) {
		Debug.Log ("mozna juz teleportowac");
		canUseTeleport = true;
	}

	void Awake () {
		rigidbody.freezeRotation = true;
		rigidbody.useGravity = true;
		
	}
	void OnGUI(){
		if(teleportOpacity > 0.0f) GUIDrawRect(new Rect(0, 0, Screen.width, Screen.height), new Color(1f, 1f, 1f, teleportOpacity));

		GUI.color = new Color (255, 0, 0, alpha);
		GUI.Label(new Rect(0,0, Screen.width, Screen.height), "", BloodSplat);
		GUI.color = new Color (255, 255, 255, 0);


	}

	void keyboardUpdate()
	{
		GraGUI gg = gameObject.GetComponent<GraGUI> ();
		Headbobber cbob = GameObject.Find("PlayerCam").GetComponent<Headbobber> ();
		GrenadeThrow gt = gameObject.GetComponent<GrenadeThrow> ();


		us = gg.us;
		canJump = true;
		float vScale = 1.5f;
		speed = normalSpeed;
		xVel = rigidbody.velocity.magnitude;
		yVel = rigidbody.velocity.y;

		if (grounded) {
			if (padjump == true){
				timer-=Time.deltaTime;
				if (timer < 0){
					padjump = false;
					timer = 0.6f;
				}
			}
			//Debug.Log(padjump);
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
			/*if(Input.GetKeyDown("q")) {
				cbob.lean=-3;
				
			}
			if(Input.GetKeyDown("e")) {
				cbob.lean=3;
				
			}
			if(Input.GetKeyDown("b")) {
				cbob.lean=0;
				
			}*/
			if (grounded && us.GetKey("up") || grounded && us.GetKey("right") || grounded && us.GetKey("down") || grounded && us.GetKey("left"))
			{
				footstepDelay = 0.6f;
				stepVol = 0.7f;
				nextFootstep -= Time.deltaTime;
				if (us.GetKey("run") && us.GetKey("up") && canSprint == true){//SPRINT
					footstepDelay = 0.5f;
					speed = runSpeed; //szybkosc poruszania sie w biegu
					stamina-=0.8f;
					stepVol = 0.85f;
				}
				else if (us.GetKey("crouch"))
				{ 	footstepDelay = 0.7f;
					//				crouch = true;
					canJump = false;
					//bodyCollider.height = 10;
					vScale = 0.85f;
					stepVol = 0.25f;
					speed = crchSpeed; //szybkosc poruszania sie w przykucnieciu
				}
			}

			if (nextFootstep <= 0) {
				audio.PlayOneShot(stepSound, stepVol);
				nextFootstep += footstepDelay;
			}
			//BIEG
			Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			targetVelocity = transform.TransformDirection(targetVelocity);
			targetVelocity *= speed;
			//Debug.Log (rigidbody.velocity.magnitude);
			
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
				source.PlayOneShot(jump_startSound,1f);
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
				source.PlayOneShot(fallSound,1f);
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

			if (us.GetKeyDown("granade")) //GRANAT
			{
				gt.Throw();
				
			}
		}
		else // not grounded
			
		{	//upadek
			if(rigidbody.velocity.y < -50.0 && padjump == false)
			{	falldmg=rigidbody.velocity.y;
			}

			
			// poruszanie sie w locie
			
			Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			if (padjump == true)
			{
				inAirControl = 0.8f;
			}
			else {inAirControl = 0.4f;}
			targetVelocity = transform.TransformDirection(targetVelocity) * inAirControl;
			rigidbody.AddForce(targetVelocity, ForceMode.VelocityChange);


		}		
		// We apply gravity manually for more tuning control
			rigidbody.AddForce(new Vector3 (0, -gravity * rigidbody.mass, 0));

		// PC: teleportowanie
		if (us.GetKeyDown("teleport") && canUseTeleport) {
			//rigidbody.velocity = Vector3.zero;
			// to naprawia kolizje jak player jest bardzo szybki
			rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			/*Vector3 targetVelocity = transform.TransformDirection(0, 0, 190);
			rigidbody.AddForce(targetVelocity, ForceMode.VelocityChange); 
*/
			padjump = true; //zeby gracz nie otrzymywal obrazen jak teleportuje sie z gory, z wysoka na dol
			source.PlayOneShot(teleSound,1f);
			GameObject cam = transform.FindChild ("PlayerCam").gameObject;

			//Ray ray = cam.camera.ScreenPointToRay(Input.mousePosition);
			Ray ray = cam.camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
			//Vector3 targetVelocity = transform.TransformDirection(ray.direction.x * 190, ray.direction.y * 190, ray.direction.z * 190);
			//rigidbody.AddForce(targetVelocity, ForceMode.VelocityChange); 
			//transform.position = ray.origin + ray.direction * 75;
			RaycastHit hitInfo = new RaycastHit();
			bool traf = Physics.Raycast(ray, out hitInfo);

			if(traf) { 
				// takie cos dziala ale instant teleport, w sumie nie wiem czy pan W tak nie chcial
				//transform.position = hitInfo.point;

				// eskperymenty
				/// nie wiem jak to zrobic ok, bo na cialo ma wplyw takze grawitacja i inne smieszne rzeczy
				/// jakby nie miala to chyba by cos takiego (albo podobnego) smigalo
				/// ale musimy teraz nasza sila przeciwdzialac grawitacji i nie wiem jak to zrobic :/
				//Vector3 sila = (hitInfo.normal) * hitInfo.distance;
				//print (hitInfo.normal);
				//print ((hitInfo.point - transform.position).normalized);
				//rigidbody.AddForce(sila, ForceMode.VelocityChange);
				//rigidbody.transform.position = Vector3.MoveTowards(rigidbody.transform.position, hitInfo.point, 40);



				canUseTeleport = false;
				timerTeleport.Stop();
				timerTeleport.Start();
				teleportOpacity = 0.4341231233f;
				timerTeleportAnim.Stop();
				timerTeleportAnim.Start();
				teleportPos = hitInfo.point;
				teleportPos.y += 2; // workaround buga wchodzenie w sufity 

			}
			else {
				teleportPos = Vector3.zero;
			}


		}

	}
	
	void FixedUpdate () 
	{
		GameObject cam = transform.FindChild ("PlayerCam").gameObject;
		GameObject hcube = transform.FindChild ("HeadCube").gameObject;
		//FPSMouselook ml2 = hcube.GetComponent (FPSControlsRigid);
		FPSMouselook ml = gameObject.GetComponent<FPSMouselook> ();


		if (!networkView.isMine) {
			cam.SetActive (false);

			//ml.RotXY=FPSMouselook.RotationAxis.Off;
			//ml.SensitivityX=0;
			//ml.SensitivityY=0;
			ml.enabled=false;

		} else {
			keyboardUpdate();
			hcube.renderer.enabled = false;
			ml.enabled=true;

		}

		if(teleportPos != Vector3.zero) {
			print("no cos robimy - pozdrawiam");
			rigidbody.transform.position = Vector3.MoveTowards(rigidbody.transform.position, teleportPos, 5);
			if ((rigidbody.transform.position - teleportPos).magnitude < 0.0001f)  {
				teleportPos = Vector3.zero;
			}
		}
	

	}
	
	
	void TrackGrounded(Collision col)
	{
		float minimumHeight = 150f;
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
		if (col.gameObject.tag == "JumpPad") {
			padjump = true;
			Vector3 velocity = rigidbody.velocity;
			rigidbody.velocity = new Vector3(velocity.x*jumpPadSpeedModifier, 100, velocity.z*jumpPadSpeedModifier);
			//Debug.Log(padjump);
		}
		
	}

	float CalculateJumpVerticalSpeed () {
		// From the jump height and gravity we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt(2 * jumpHeight * gravity);
	}

	public void getDmg(int amount){
		currHP-=amount;
		//animation.Play("Death");
		
	}
}


