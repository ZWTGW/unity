using UnityEngine;

public class Minimap : MonoBehaviour 
{

	public Transform Target;

	void LateUpdate()
	{
		transform.position = new Vector3 (Target.position.x, transform.position.y, Target.position.z);
	
	}
}
