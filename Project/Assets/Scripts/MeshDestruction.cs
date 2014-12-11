using UnityEngine;
using System.Collections;

public class MeshDestruction : MonoBehaviour {
	public GameObject shardToSpawn;
	public int amountOfShards = 15;

	void OnCollisionEnter(Collision col) {
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		int[] triangles = mesh.triangles;
		int[] newTriangles = new int[triangles.Length - 3];
		for (int i = 0; i < triangles.Length - 3; i++) {
			newTriangles[i] = triangles[i];
		}
		mesh.triangles = newTriangles;
		mesh.RecalculateBounds();

		for (int j = 0; j < 3; j++) {
			GameObject shard = Instantiate(shardToSpawn, col.collider.transform.position + new Vector3(3, 3, 3), Quaternion.identity) as GameObject;
			shard.rigidbody.AddForce(Random.value, Random.value, Random.value);
		}
	}
}
