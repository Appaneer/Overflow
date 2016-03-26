using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map : MonoBehaviour {

	public GameObject[] bricks;
	public int height = 9;
	public int width;

	public HashSet<Node> selectedNodes;
	public int sum = 7;
	RaycastHit hit;

	public float timeToSpawn = 3.0f;
	float[] stuff = { -2.5f, -1.5f, -0.5f, 0.5f, 1.5f, 2.5f};
	int index = 0;

	void Start(){
		selectedNodes = new HashSet<Node> ();
		InitMap ();
	}

	void Update(){
		timeToSpawn -= Time.deltaTime;
		if (timeToSpawn <= 0.0f) {
			Instantiate (bricks[Random.Range (1,bricks.Length)], new Vector2 (stuff[index++], 9f), Quaternion.identity);
			timeToSpawn = 3.0f;
			if(index == stuff.Length)
				index = 0;
		}

		if (Input.touchCount == 1)
		{

			foreach (Touch touch in Input.touches)
			{
				if(Physics.Raycast(Camera.main.ScreenPointToRay(touch.position), out hit)){
					selectedNodes.Add (hit.transform.gameObject.GetComponent<Node>());
				}

				switch (touch.phase)
				{

				case TouchPhase.Canceled://the touch is cancelled
				case TouchPhase.Ended:// the touch is ended so now we can calculate the time and distance
					int temp = 0;
					foreach (Node node in selectedNodes) {
						temp += node.value;
					}
					if (temp == sum) {
						foreach (Node node in selectedNodes) {
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

	void InitMap(){
		for(float r = 0; r < height; r++){
			for(float c = 0; c < width; c++){
				Instantiate (bricks[Random.Range (1,bricks.Length)], new Vector2 (c-2.5f, r-2), Quaternion.identity);
			}
		}
	}


}
