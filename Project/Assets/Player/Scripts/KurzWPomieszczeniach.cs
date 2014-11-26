using UnityEngine;
using System.Collections;

public class KurzWPomieszczeniach : MonoBehaviour {
	public ParticleSystem particleSystem;
	public Transform[] dustSpawnPointTransforms;

	void Start () {
		foreach (Transform transform in dustSpawnPointTransforms) {
			GameObject kurz = Instantiate(particleSystem, transform.position, transform.rotation) as GameObject;
			Transform kurzTransform = kurz.GetComponent<Transform>();
			kurzTransform.localScale = transform.localScale;
		}
	}
}
