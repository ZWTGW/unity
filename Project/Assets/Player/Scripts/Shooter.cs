using UnityEngine;
using System.Collections;

public class Shooter : MonoBehaviour {
	public GameObject weaponModel;
	private GameObject weapon;

	public GameObject camera;

	// Use this for initialization
	void Start () {
		weapon = Instantiate(weaponModel) as GameObject;
		weapon.transform.position = this.transform.position + new Vector3(0, 0, 2);
		weapon.transform.parent = camera.transform;
		//weapon.transform.parent = GameObject.Find ("PlayerCam").transform;// this.transform;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0))
		{
			weapon.GetComponent<Weapon>().StartShooting();
		}
		if(Input.GetMouseButtonUp(0))
		{
			weapon.GetComponent<Weapon>().StopShooting();
		}
		
		if(Input.GetKey("r"))
		{
			weapon.GetComponent<Weapon>().StartReloading();
		}
	}
}
