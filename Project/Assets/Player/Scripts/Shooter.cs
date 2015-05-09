using UnityEngine;
using System.Collections;

public class Shooter : MonoBehaviour {
	private GameObject weapon;
	private int actualWeapon;

	private int weaponCount;
	public int weaponMax;

	public GameObject sCamera;
	public GameObject[] weaponsList;
	public GameObject[] weapons;

	public bool forceShooting = false;
	public GameObject arms;

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
		Ray ray = new Ray(sCamera.transform.position, sCamera.transform.rotation * new Vector3(0, 0, 1));
		Debug.DrawRay(sCamera.transform.position, sCamera.transform.rotation * new Vector3(0, 0, 100000000));

		if (forceShooting) weapon.GetComponent<Weapon>().StartShooting();

		if(Input.GetMouseButtonDown(0))
		{
			StartShooting();
		}
		if(Input.GetMouseButtonUp(0))
		{
			StopShooting ();
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
		if(Input.GetKey("4"))
		{
			ChangeWeapon(4);
		}
		if(Input.GetKey("5"))
		{
			ChangeWeapon(5);
		}
		if(Input.GetKey("6"))
		{
			ChangeWeapon(6);
		}
		if(Input.GetKey("7"))
		{
			ChangeWeapon(7);
		}
		if(Input.GetKey("8"))
		{
			ChangeWeapon(8);
		}	
		if(Input.GetKey("9"))
		{
			ChangeWeapon(9);
		}

	}

	public void StartShooting()
	{
		weapon.GetComponent<Weapon>().StartShooting();
		appwarp.notifyShooting = true;
		
		//byle co zeby tylko narastala ilosc kills - absolutnie do wywalenia
		BaseCharacter baseCharScript = GetComponent<BaseCharacter>();
		baseCharScript.kills++;
	}

	public void StopShooting()
	{
		weapon.GetComponent<Weapon>().StopShooting();
		appwarp.notifyShooting = false;
	}

	void PrepareWeapons(int n)
	{
		for (int i = 0; i < weaponMax; i++)
		{
			weapons[i] = Instantiate(weaponsList[i]) as GameObject;
			weapons[i].GetComponent<Weapon>().SetShooter(gameObject);
			weapons[i].transform.localRotation = sCamera.transform.rotation;
			weapons[i].transform.parent = sCamera.transform;
			weapons[i].transform.localPosition = weaponPosition;
			weapons[i].GetComponent<Renderer>().enabled = false;

		}
		actualWeapon = -1;
		ChangeWeapon(n);
	}

	void ChangeWeapon(int n)
	{
		if(n == 0)
			arms.GetComponent<Animation> ().Play ("bazooka_up");
		if(n == 1)
			arms.GetComponent<Animation> ().Play ("railgun_up");
		if(n == 2)
			arms.GetComponent<Animation> ().Play ("bazooka_up");
		if(n == 3)
			arms.GetComponent<Animation> ().Play ("bazooka_down");
		if(n == 4)
			arms.GetComponent<Animation> ().Play ("railgun_up");
		if(n == 5)
			arms.GetComponent<Animation> ().Play ("railgun_down");

		if (n >= weaponsList.Length || weaponsList[n] == null || actualWeapon == n)
		{
			return;
		}
		if (weapon != null)
		{
			weapon.GetComponent<Renderer>().enabled = false;
		}
		if (n >= weapons.Length || weapons[n] == null)
		{
			weapon = Instantiate(weaponsList[n]) as GameObject;
			
			weapon.GetComponent<Weapon>().SetShooter(gameObject);
			//weapon.transform.position = this.transform.position + new Vector3(0, 0, 2);
			weapon.transform.localRotation = sCamera.transform.rotation;
			weapon.transform.parent = sCamera.transform;
			weapon.transform.localPosition = weaponPosition;
			weapons[n] = weapon;

		}
		actualWeapon = n;
		weapon = weapons[n];
		weapon.GetComponent<Renderer>().enabled = true;
		//Destroy(weapon);


	}

	public Vector3 GetTarget()
	{
		RaycastHit hit;
		Ray ray = new Ray(sCamera.transform.position, sCamera.transform.rotation * new Vector3(0, 0, 1));
		if(Physics.Raycast(ray, out hit, 100000000))
		{
			if(hit.transform != null)
			{
				return hit.point;
			}

		}
		return sCamera.transform.position + (sCamera.transform.rotation * new Vector3(0, 0, 1000));
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
