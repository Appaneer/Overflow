using UnityEngine;
using System.Collections;

public class GravityNode : MonoBehaviour {

	public int value;
	public Transform centerOfGravity;
	public float pullForce;
	Rigidbody rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
	}

	// Update is called once per frame
	void FixedUpdate () {
		Vector3 forceDirection = centerOfGravity.position - transform.position;
		rb.AddForce (forceDirection.normalized * Time.fixedDeltaTime * pullForce);
	}
}
