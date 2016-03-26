using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpaceLevelManager : MonoBehaviour {

	public GameObject[] bricks;

	public HashSet<GravityNode> selectedNodes;
	public int sum;
	RaycastHit hit;

	public float timeToSpawn;
	float acc;
	public Transform[] spawnPoints;
	int index = 0;

	int score;

	void Start(){
		acc = timeToSpawn;
		score = 0;
		selectedNodes = new HashSet<GravityNode> ();
		InitMap ();
	}

	void Update(){
		acc -= Time.deltaTime;
		if (acc <= 0.0f) {
			Instantiate (bricks [Random.Range (1, bricks.Length)], spawnPoints [index++].position, Quaternion.identity);
			acc = timeToSpawn;
			if (index == spawnPoints.Length)
				index = 0;
		}

		if (Input.touchCount == 1) {

			foreach (Touch touch in Input.touches) {
				if (Physics.Raycast (Camera.main.ScreenPointToRay (touch.position), out hit)) {
					selectedNodes.Add (hit.transform.gameObject.GetComponent<GravityNode> ());
				}

				switch (touch.phase) {
				case TouchPhase.Ended:
					{
						// the touch is ended so now we can calculate the time and distance
						int temp = 0;
						string stuff = "";
						foreach (GravityNode node in selectedNodes) {
							stuff += node.value + " ";
							temp += node.value;
						}
						Debug.Log (stuff+" ===>"+temp);
						if (temp == sum) {
							//UIManager.updateScore (++score);
							//TODO animation
							foreach (GravityNode node in selectedNodes) {
								Destroy (node.gameObject);
							}
						} else {
							//TODO wrong sum animation 
						}
						selectedNodes.Clear ();
						temp = 0;
						break;
					}
				}
			}
		}
	}

	void InitMap(){
		for(float r = 2; r >= -2; r--){
			for(float c = 2; c >= -2; c--){
				Instantiate (bricks[Random.Range (1,bricks.Length)], new Vector2 (r, c), Quaternion.identity);
			}
		}
	}
}
