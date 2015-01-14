using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshDestruction : MonoBehaviour {
	public GameObject shardToSpawn;
	public int amountOfShards = 5;

	void OnCollisionEnter(Collision collision) {
		foreach (ContactPoint contact in collision.contacts) {
			Ray ray = new Ray(contact.point - contact.normal * 0.05f, contact.normal);
			RaycastHit hit;
			if (contact.thisCollider.Raycast(ray, out hit, 100.0F)) {
				Mesh mesh = hit.collider.GetComponent<MeshFilter>().mesh;
				List<int> indices = new List<int>(mesh.triangles);
				indices.RemoveRange(hit.triangleIndex * 3, 3);
				mesh.triangles = indices.ToArray();
				mesh.RecalculateBounds();
				
				for (int j = 0; j < amountOfShards; j++) {
					GameObject shard = Instantiate(shardToSpawn, contact.point, Quaternion.identity) as GameObject;
					shard.rigidbody.AddForce(Random.value, Random.value, Random.value);
				}
			}
		}
	}
}
