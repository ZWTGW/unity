using UnityEngine;
using System.Collections;

public class Flag : MonoBehaviour {
	
	public string team; 
	public GameObject keeper = null;
	public bool keepFlag = false;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if (keepFlag) {
			
			Vector3 p = keeper.transform.position;
			
			transform.position = new Vector3(p.x, p.y, p.z);
			
		}
		
		if (Input.GetMouseButton(1)) 
		{
			keepFlag = false;
			collider.enabled = true;
			keeper = null;
		}
		
	}
	
	void OnCollisionEnter( Collision col )
	{
		if (col.gameObject.tag == "Player") {
			keepFlag = true;
			keeper = col.gameObject;
			collider.enabled = false;
		}
	}
}
