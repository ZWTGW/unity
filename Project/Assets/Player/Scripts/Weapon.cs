using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon : MonoBehaviour {

	public string name;
	public int range;
	public int strength;
	public float fireRate;
	public float bulletSpeed;

	public int ammoInMag;
	public int maxAmmoInMag;
	public int ammoInShot;

	public float reloadingTime;
	public float reloadingAdvance;

	public bool reloading;
	public bool shooting;

	private State state;

	public float lastShot;
	public float LastShot
	{
		get
		{
			return lastShot;
		}

		set
		{
			lastShot = value;
		}
	}

	public bool rail;
	public Type type;

	public Rigidbody Bullet;
	public Transform endPoint;

	// Use this for initialization
	void Start () {
		//state = new List<State>();
		if(rail)
		{
			type = new Rail(this);
		}else
		{
			type = new Gun(this);
		}

		endPoint.transform.position = transform.position + new Vector3(0, 0, 1);


		reloading = false;
		shooting = false;
		lastShot = fireRate;

		ammoInMag = maxAmmoInMag;


	}
	
	// Update is called once per frame
	void Update () {

		if(lastShot <= fireRate)
		{
			lastShot += Time.deltaTime;
			//lastShot += 1;
		}


		if(Input.GetMouseButtonDown(0))
		{
			StartShooting();
		}
		if(Input.GetMouseButtonUp(0))
		{
			StopShooting();
		}

		if(Input.GetKey("r"))
		{
			StartReloading();
		}

	

		//foreach (State st in state)
		if(state != null)
		{
			state.Update();
		}
	}

	public void StartShooting()
	{
		if(state == null)
		{
			State st = new Shooting(this);
			state = st;
		}
	}
	public void StopShooting()
	{
		if (state != null)
		{
			if (state.GetType().ToString() == "Shooting")
			{
 				DestroyState();
			}
		}
	}

	public void StartReloading()
	{
		bool b = true;
		if (state != null)
		{
			if(state.GetType().ToString()== "Reloading")
			{
				b = false;
			}
		}
		if(b)
		{
			State st = new Reloading(this);
			state = st;
			Debug.Log("reload");
		}
	}
	public void stopShooting()
	{
		if(state != null)
		{
			if (state.GetType().ToString() == "Reloading")
			{
				DestroyState();
			}
		}
	}

	public void DestroyState(State s)
	{
		//state.Remove(s);
	}
	public void DestroyState()
	{
		state = null;
	}

	public void Fire()
	{
		if (ammoInMag == 0)
		{
			return;
		}
		ammoInMag -= ammoInShot;

		//strzelanie
		Rigidbody bulletInstance;
		bulletInstance = Instantiate(Bullet, endPoint.position, new Quaternion()) as Rigidbody;
		bulletInstance.AddForce(new Vector3(0, 0, 1) * bulletSpeed);

		if(ammoInMag == 0)
		{
			StartReloading();
		}
	}

	/*
	public void Reloading()
	{
		reloading = true;
	}
	*/

	public void Reloaded(int ammo)
	{
		ammoInMag = ammo;
		reloading = false;
	}


}


//state/////////////////////////////////////////////////////////////////////////
#region
public abstract class State
{
	private Weapon weapon;
	public Weapon Weapon
	{
		get
		{
			return weapon;
		}
		set
		{

		}
	}


	public State(Weapon w)
	{
		weapon = w;
	}

	public abstract void Update();

	public void Destroy()
	{
		weapon.DestroyState();
	}
}

class Reloading:State
{
	public Reloading(Weapon w):base(w)
	{
		Weapon.reloadingAdvance = 0;
	}

	public override void Update()
	{
		Weapon.reloadingAdvance += Time.deltaTime;
		if(Weapon.reloadingAdvance >= Weapon.reloadingTime)
		{
			Weapon.Reloaded(Weapon.maxAmmoInMag);//do pomyślenia. Ilośc amunicji podawana od playera??
			Debug.Log("reloading end");
			Destroy();
		}

	}
}

class Shooting:State
{
	public Shooting(Weapon w):base(w)
	{

	}

	public override void Update()
	{
		if (Weapon.LastShot - Weapon.fireRate >= 0)
		{
			if (Weapon.ammoInMag == 0)
			{
				return;
			}
			Weapon.ammoInMag -= Weapon.ammoInShot;

			Weapon.LastShot = 0;
			Weapon.type.Fire();

			
			if(Weapon.ammoInMag == 0)
			{
				Weapon.StartReloading();
			}
		}
	}
}
#endregion


//type////////////////////////////////////////////////////////////////////////
#region
public abstract class Type: MonoBehaviour
{
	private Weapon weapon;
	public Weapon Weapon
	{
		get
		{
			return weapon;
		}
		set
		{
			
		}
	}

	public Type(Weapon _weapon)
	{
		weapon = _weapon;
	}

	public abstract void Fire();
}

public class Gun: Type
{
	public Gun(Weapon w): base(w)
	{}

	public override void Fire()
	{
		Rigidbody bulletInstance;
		bulletInstance = Instantiate(Weapon.Bullet, Weapon.endPoint.position, new Quaternion()) as Rigidbody;

		bulletInstance.AddForce((Weapon.transform.rotation * new Vector3(0, 0, 1)) * Weapon.bulletSpeed);
		//bulletInstance.transform.Rotate(Vector3.right, 90);

		//bulletInstance.AddForce(new Vector3(0, 0, 1) * Weapon.bulletSpeed);
		Debug.Log(Weapon.transform.rotation.ToString());
	}
}


public class Rail: Type
{
	public Rail(Weapon w): base(w)
	{}

	public override void Fire()
	{
		RaycastHit hit;
		Vector3 v = new Vector3(0, 0, 1);
		
		Ray ray = new Ray(Weapon.endPoint.position, v);

		if(Physics.Raycast(ray, out hit, Weapon.range));//.rigidbody.velocity)))
		{
			//hit.transform.localScale = new Vector3(2, 2, 2);
			if(hit.transform != null)
			{
				if (hit.transform.tag == "BulletObstacle")
				{
					Destroy(hit.transform.gameObject);
				}
			}
		}
	}
}
#endregion