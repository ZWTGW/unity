using UnityEngine;
using System.Collections;

public class FPSMouselook : MonoBehaviour {
	public enum RotationAxis {MouseXY = 0, MouseX = 1, MouseY =2}
	public RotationAxis RotXY = RotationAxis.MouseXY | RotationAxis.MouseX | RotationAxis.MouseY;
	//os X
	public float SensitivityX = 400f;
	public float MinimumX = -360f;
	public float MaximumX = 360f;
	private float RotationX = 0f; // w danej chwili

	//os Y
	public float SensitivityY = 400f;
	public float MinimumY = -75f;
	public float MaximumY = 75f;
	private float RotationY = 0f; // w danej chwili

	public Quaternion OriginalRotation; // quaternion - uzywane do reprezentowania obrotow

	// Use this for initialization
	void Start () {

		OriginalRotation = transform.localRotation;
	
	}
	
	// Update is called once per frame
	void Update () {
		if (RotXY == RotationAxis.MouseXY) {
			RotationX += Input.GetAxis("Mouse X") * SensitivityX * Time.deltaTime;
			RotationY -= Input.GetAxis("Mouse Y") * SensitivityY * Time.deltaTime;
			RotationX=ClampAngle (RotationX,MinimumX,MaximumX);
			RotationY=ClampAngle (RotationY,MinimumY,MaximumY);
			Quaternion XQuaternion = Quaternion.AngleAxis(RotationX, Vector3.up);
			Quaternion YQuaternion = Quaternion.AngleAxis(RotationY, Vector3.right);
			transform.localRotation = OriginalRotation * XQuaternion * YQuaternion;

		
		}
		else if(RotXY == RotationAxis.MouseX){
			RotationX += Input.GetAxis("Mouse X") * SensitivityX * Time.deltaTime; /* Mouse X jest zdefiniowane juz w unity
			(edit project settings input). 
			
			Delta Time - the time in seconds it took to complete the last frame (Read Only).
			Use this function to make your game frame rate independent.*/
			RotationX=ClampAngle (RotationX,MinimumX,MaximumX);
			Quaternion XQuaternion = Quaternion.AngleAxis(RotationX, Vector3.up);
			transform.localRotation = OriginalRotation * XQuaternion;
		}
		else if(RotXY == RotationAxis.MouseY){
			RotationY -= Input.GetAxis("Mouse Y") * SensitivityY * Time.deltaTime; /* Mouse X jest zdefiniowane juz w unity
			(edit project settings input). 
			
			Delta Time - the time in seconds it took to complete the last frame (Read Only).
			Use this function to make your game frame rate independent.*/
			RotationY=ClampAngle (RotationY,MinimumY,MaximumY);
			Quaternion YQuaternion = Quaternion.AngleAxis(RotationY, Vector3.right);
			transform.localRotation = OriginalRotation * YQuaternion;
		}
	}
	public static float ClampAngle (float Angle, float Min, float Max){
		if(Angle <-360){
			Angle +=360;
		}
		if(Angle >360){
			Angle -=360;
		}
		return Mathf.Clamp (Angle, Min, Max);
	}

}
