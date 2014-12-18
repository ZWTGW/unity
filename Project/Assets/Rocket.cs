using UnityEngine;
using System.Collections;

public class Rocket : MonoBehaviour {
	public float acceleration;
	public float timeDestruction;
	private float time;
	
	public float blowSize = 15.0f;
	public float maxDamage = 60.0f;
	public GameObject explosionPrefab;
	
	private Vector3 lastPosition;

	// Use this for initialization
	void Start () {
		time = 0;
		lastPosition = gameObject.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;

		gameObject.GetComponent<Rigidbody>().velocity *= acceleration;
		//kolizje z obiektami////////////////////////////////
		RaycastHit hit;
		Vector3 v = new Vector3();
		v = gameObject.transform.position - lastPosition;
		Ray ray = new Ray(lastPosition, v);
		if(Physics.Raycast(ray, out hit, Vector3.Magnitude(v)));//.rigidbody.velocity)))
		{
			if(hit.transform != null)
			{
				Explode(hit.point);//pozycja styku
				return;
			}

		}
		lastPosition = gameObject.transform.position;
		///////////////////////////////////////////////////
		if(time >= timeDestruction)
		{
			Explode(gameObject.transform.position);
		}
	}

	void Explode(Vector3 position)
	{
		//Instantiate(explosionPrefab,gameObject.transform.position, gameObject.transform.rotation );
		Instantiate(explosionPrefab, position, gameObject.transform.rotation );

		Collider[] hitColliders = Physics.OverlapSphere( transform.position, blowSize );
		
		foreach( Collider collision in hitColliders )
		{
			if( collision.gameObject.name == "Player(Clone)" )
			{
				FPSControlsRigid bs = collision.gameObject.GetComponent<FPSControlsRigid>() ;
				
				Vector3 distance = collision.gameObject.transform.position - gameObject.transform.position;
				float damageDone = (1.0f-distance.magnitude/blowSize)*maxDamage;
				bs.getDmg( (int)damageDone );
			}
			else
			{
				Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
				if( rb != null)
				{
					Debug.Log( "boom!");
					rb.AddExplosionForce( maxDamage*300, gameObject.transform.position, blowSize );
				}
			}
		}
		Destroy( this.gameObject );
	}
}
