using UnityEngine;
using System.Collections;

public class MainScript : MonoBehaviour {
	public GameObject playerPrefab;
	//public GameObject player2Prefab;
	
	
	public void SpawnPlayer()
	{
		int rand = Random.Range (0, 5);
		int spawnpoint = Random.Range (0, 4);
		switch (spawnpoint) {
		case 3:	Instantiate(playerPrefab, new Vector3(600f + rand, 115f, -375f), Quaternion.identity);
			break;
		case 2:	Instantiate(playerPrefab, new Vector3(215f + rand, 65f, -272f), Quaternion.identity);
			break;
		case 1:	Instantiate(playerPrefab, new Vector3(180f + rand, 55f, -400f + rand), Quaternion.identity);
			break;
		case 0:	Instantiate(playerPrefab, new Vector3(545f + rand, 120f, -280f), Quaternion.identity);
			break;
		
		}

		
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