using UnityEngine;
using System.Collections;

public class Shooter : MonoBehaviour {
	private GameObject weapon;
	private int actualWeapon;

	private int weaponCount;
	public int weaponMax;

	public GameObject camera;
	public GameObject[] weaponsList;
	public GameObject[] weapons;

	public bool forceShooting = false;

	private Vector3 weaponPosition = new Vector3(2,0,1);

	// Use this for initialization
	void Start () {
		weaponCount = 0;
		for(int i = 0; i < weaponMax; i++)
		{
			if (weaponsList[i] != null)
			{
				weaponCount++;
			}
		}
		//ChangeWeapon(0);
		PrepareWeapons(0);
		//weapon.transform.parent = GameObject.Find ("PlayerCam").transform;// this.transform;
	}
	
	// Update is called once per frame
	void Update () {
		Ray ray = new Ray(camera.transform.position, camera.transform.rotation * new Vector3(0, 0, 1));
		Debug.DrawRay(camera.transform.position, camera.transform.rotation * new Vector3(0, 0, 100000000));

		if (forceShooting) weapon.GetComponent<Weapon>().StartShooting();

		if(Input.GetMouseButtonDown(0))
		{
			weapon.GetComponent<Weapon>().StartShooting();
			appwarp.notifyShooting = true;

			//byle co zeby tylko narastala ilosc kills - absolutnie do wywalenia
			BaseCharacter baseCharScript = GetComponent<BaseCharacter>();
			baseCharScript.kills++;
		}
		if(Input.GetMouseButtonUp(0))
		{
			weapon.GetComponent<Weapon>().StopShooting();
			appwarp.notifyShooting = false;
		}
		
		if(Input.GetKey("r"))
		{
			weapon.GetComponent<Weapon>().StartReloading();
		}

		if(Input.GetKey("0"))
		{
			ChangeWeapon(0);
		}
		if(Input.GetKey("1"))
		{
			ChangeWeapon(1);
		}
		if(Input.GetKey("2"))
		{
			ChangeWeapon(2);
		}
		if(Input.GetKey("3"))
		{
			ChangeWeapon(3);
		}
	}

	void PrepareWeapons(int n)
	{
		for (int i = 0; i < weaponMax; i++)
		{
			weapons[i] = Instantiate(weaponsList[i]) as GameObject;
			weapons[i].GetComponent<Weapon>().SetShooter(gameObject);
			weapons[i].transform.localRotation = camera.transform.rotation;
			weapons[i].transform.parent = camera.transform;
			weapons[i].transform.localPosition = weaponPosition;
			weapons[i].renderer.enabled = false;

		}
		actualWeapon = -1;
		ChangeWeapon(n);
	}

	void ChangeWeapon(int n)
	{
		if (n >= weaponsList.Length || weaponsList[n] == null || actualWeapon == n)
		{
			return;
		}
		if (weapon != null)
		{
			weapon.renderer.enabled = false;
		}
		if (n >= weapons.Length || weapons[n] == null)
		{
			weapon = Instantiate(weaponsList[n]) as GameObject;
			
			weapon.GetComponent<Weapon>().SetShooter(gameObject);
			//weapon.transform.position = this.transform.position + new Vector3(0, 0, 2);
			weapon.transform.localRotation = camera.transform.rotation;
			weapon.transform.parent = camera.transform;
			weapon.transform.localPosition = weaponPosition;
			weapons[n] = weapon;

		}
		actualWeapon = n;
		weapon = weapons[n];
		weapon.renderer.enabled = true;
		//Destroy(weapon);

	}

	public Vector3 GetTarget()
	{
		RaycastHit hit;
		Ray ray = new Ray(camera.transform.position, camera.transform.rotation * new Vector3(0, 0, 1));
		if(Physics.Raycast(ray, out hit, 100000000))
		{
			if(hit.transform != null)
			{
				return hit.point;
			}

		}
		return camera.transform.position + (camera.transform.rotation * new Vector3(0, 0, 1000));
	}

	void DeleteWeapon(int n)
	{
		weaponsList[n] = null;
		weaponCount--;
	}

	void AddWeapon(GameObject ob)
	{
		if (weaponCount < weaponMax)
		{
			for(int i = 0; i < weaponMax; i++)
			{
				if(weaponsList[i] == null)
				{
					weaponsList[i] = ob;
				}
			}
			weaponCount++;
		}
	}
}
