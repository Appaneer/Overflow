using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {

	public GameObject[] bricks;
	protected int score;
	public Transform[] spawnPoints;
	protected int index;
	public int sum;
	public HashSet<Node> selectedNodes;
	public static bool isPaused = false;

	public float timeToSpawn = 3.0f;
	protected float accumulator;

	protected void GetInput<T>() where T : Node{
		RaycastHit hit;
		if (Input.touchCount == 1) {

			foreach (Touch touch in Input.touches) {
				if (Physics.Raycast (Camera.main.ScreenPointToRay (touch.position), out hit)) {
					selectedNodes.Add (hit.transform.gameObject.GetComponent<T> ());
				}

				switch (touch.phase) {
				case TouchPhase.Ended:
					{
						// the touch is ended so now we can calculate the time and distance
						int temp = 0;
						foreach (T node in selectedNodes) {
							temp += node.value;
						}
						if (temp == sum) {
							//TODO animation
							score += selectedNodes.Count;
							HowShouldINameThis ();
							UIManager.updateScore (score);
							foreach (T node in selectedNodes) {
								Destroy (node.gameObject);
							}
						} else {
							//TODO wrong sum animation 
						}
						temp = 0;
						selectedNodes = new HashSet<Node> ();
						break;
					}
				}
			}
		}
	}

	protected void HowShouldINameThis(){
		if (score >= 20 && score < 40)
			timeToSpawn = 1.75f;
		else if (score >= 40)
			timeToSpawn = 1.5f;
	}

	protected void SpawnNodes(){
		accumulator -= Time.deltaTime;
		if (accumulator <= 0.0f) {
			if(!isPaused)
				Instantiate (bricks [Random.Range (1, bricks.Length)], spawnPoints [index++].position, Quaternion.identity);
			accumulator = timeToSpawn;
			if (index == spawnPoints.Length)
				index = 0;
		}
	}

	public static void DeleteNodes(int amount){
		GameObject[] arr = GameObject.FindGameObjectsWithTag("Node");
		for (int i = 0; i < amount; i++) {
			Destroy (arr [i]);
		}
	}
}
