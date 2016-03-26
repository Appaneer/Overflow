using UnityEngine;
using System.Collections;

public class GameOver : MonoBehaviour {

	void OnTriggerExit(Collider other){
		Debug.Log ("Game over");
	}
}
