using UnityEngine;
using System.Collections;

public class KurzWPomieszczeniach : MonoBehaviour {
	public ParticleSystem dustParticleSystem;
	public ParticleSystem mistParticleSystem;
	public Transform[] effectSpawnPoints;

	void Start () {
		foreach (Transform effectSpawnPoint in effectSpawnPoints) {
			GameObject kurz = Instantiate(dustParticleSystem, effectSpawnPoint.position, effectSpawnPoint.rotation) as GameObject;
			GameObject mgla = Instantiate(mistParticleSystem, effectSpawnPoint.position, effectSpawnPoint.rotation) as GameObject;
			kurz.transform.localScale = effectSpawnPoint.localScale;
			mgla.transform.localScale = new Vector3(effectSpawnPoint.localScale.x, effectSpawnPoint.localScale.y / 5, effectSpawnPoint.localScale.z);
		}
	}
}
