using UnityEngine;
using System.Collections;

public class MainScript : MonoBehaviour {
	public GameObject playerPrefab;
	//public GameObject player2Prefab;


	private void SpawnPlayer()
	{
		int rand = Random.Range (10, 10);
		Instantiate(playerPrefab, new Vector3(70f + rand, 1.5f, 170f), Quaternion.identity);

		GraGUI.mapName = "Antique";
	}



	
	// Use this for initialization
	void Start () {
		SpawnPlayer ();
	}


	
	// Update is called once per frame
	void Update () {
	}
}
