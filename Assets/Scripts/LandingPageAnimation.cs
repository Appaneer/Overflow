using UnityEngine;
using System.Collections;

public class LandingPageAnimation : MonoBehaviour {

	float timeToSpawn = 0.35f;
	protected float accumulator;

	public GameObject brick;
	void Start () {
		accumulator = timeToSpawn;
	}

	void Update () {
		accumulator -= Time.deltaTime;
		if (accumulator <= 0.0f) {
			Instantiate (brick, new Vector3(Random.Range(-2.5f,2.5f),Random.Range(5.5f,6f),-1f), Quaternion.identity);
			accumulator = timeToSpawn;
		}
	}
}