using UnityEngine;
using System.Collections;

public class Grenade : MonoBehaviour {
	public float timeToExplode = 2.0f;
	public float blowSize = 15.0f;
	public float maxDamage = 60.0f;
	public GameObject explosionPrefab;

	// Invoked once - just before Instantiate() is being invoked
	void Awake()
	{
		Invoke("Explode", timeToExplode);
	}

	// Update is called once per frame
	void Update () {

	}

	void Explode()
	{
		Instantiate(explosionPrefab,gameObject.transform.position, gameObject.transform.rotation );
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
