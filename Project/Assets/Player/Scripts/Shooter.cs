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

		if (forceShooting) weapon.GetComponent<Weapon>().StartShooting();

		if(Input.GetMouseButtonDown(0))
		{
			weapon.GetComponent<Weapon>().StartShooting();

			//byle co zeby tylko narastala ilosc kills - absolutnie do wywalenia
			BaseCharacter baseCharScript = GetComponent<BaseCharacter>();
			baseCharScript.kills++;
		}
		if(Input.GetMouseButtonUp(0))
		{
			weapon.GetComponent<Weapon>().StopShooting();
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
			weapons[i].transform.localRotation = camera.transform.rotation;
			weapons[i].transform.parent = camera.transform;
			weapons[i].transform.localPosition = new Vector3(0, -1, 2);
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
			weapon.transform.position = this.transform.position + new Vector3(0, 0, 2);
			weapon.transform.localRotation = camera.transform.rotation;
			weapon.transform.parent = camera.transform;
			weapon.transform.localPosition = new Vector3(0, -1, 2);
			weapons[n] = weapon;
		}
		actualWeapon = n;
		weapon = weapons[n];
		weapon.renderer.enabled = true;
		//Destroy(weapon);

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
