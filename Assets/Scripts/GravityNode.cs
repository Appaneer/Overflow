using UnityEngine;
using System.Collections;

public class GravityNode : Node {

	public Transform centerOfGravity;
	public float pullForce;
	public bool isBomb;
	Rigidbody rb;

	void Start () {
		rb = GetComponent<Rigidbody> ();
		if (value != 0)
			isBomb = false;
	}
		
	void FixedUpdate () {
		Vector3 forceDirection = centerOfGravity.position - transform.position;
		rb.AddForce (forceDirection.normalized * Time.fixedDeltaTime * pullForce);
	}

	void OnDestroy(){
		GameObject[] arr = GameObject.FindGameObjectsWithTag ("Node");
		int temp = 0;
		if (isBomb) {
			for(int i = 0; i < arr.Length && temp < 9; i++){
				if(Vector2.Distance(transform.position, arr[i].transform.position) <= 1.5f){//root 2 + some tolerance
					Destroy (arr[i]);
					temp++;
				}
			}
		}
	}
}
