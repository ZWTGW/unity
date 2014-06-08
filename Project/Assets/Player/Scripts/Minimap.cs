using UnityEngine;

public class Minimap : MonoBehaviour 
{
	
	public Transform Target;
	public Camera Cam;
	public bool isFullScreen=false;
	public Rect camSize;
	
	void Start()
	{
		camSize = Cam.pixelRect;
	}
	void LateUpdate()
	{
		transform.position = new Vector3 (Target.position.x, transform.position.y, Target.position.z);
		
	}
	
	void Update()
	{
		if (Input.GetKeyDown (KeyCode.M)) 
		{
			if(!isFullScreen)
			{
				Cam.orthographicSize+=40;
				Cam.pixelRect=new Rect(10,10,Screen.width-20,Screen.height-20);
				isFullScreen=true;
			}
			else
			{
				Cam.orthographicSize-=40;
				Cam.pixelRect=camSize;
				isFullScreen=false;
			}
		}
	}
	
	void OnGUI() {
		if (GUI.Button(new Rect(Screen.width-20, 20, 25, 20), "-")) {
			Cam.orthographicSize+=20;
		}
		if (GUI.Button(new Rect(Screen.width-20, 60, 25, 20), "+")) {
			if(Cam.orthographicSize>30)
				Cam.orthographicSize-=20;
		}
		//if (GUI.Button(Rect(10,10,50,50),btnTexture)) {
		//	Debug.Log("Clicked the button!");
		//}
	}
}