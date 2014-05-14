using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public float timeDeath;

	private Vector3 lastPosition;

	// Use this for initialization
	void Start () {
		lastPosition = gameObject.transform.position;
	}

	// Update is called once per frame
	void Update () {

		RaycastHit hit;
		Vector3 v = new Vector3();
		v = gameObject.transform.position - lastPosition;

		Ray ray = new Ray(lastPosition, v);

		Debug.DrawRay(lastPosition, v);
		if(Physics.Raycast(ray, out hit, Vector3.Magnitude(v)));//.rigidbody.velocity)))
		{
			//hit.transform.localScale = new Vector3(2, 2, 2);
			if(hit.transform != null)
			{
				if (hit.transform.tag == "BulletObstacle")//obiekty do niszczenia
				{
					Destroy(hit.transform.gameObject);
					Destroy(gameObject);
				}else
				{
					Destroy(gameObject);
				}
			}

			//Destroy(gameObject);
		}
		lastPosition = gameObject.transform.position;

		Destroy (gameObject, timeDeath);
	}

	void OnCollisionEnter(Collision collision)
	{
		//Destroy(collision.gameObject);
		//Destroy(gameObject);
		//Debug.Log("ajsjas");
	}
}
