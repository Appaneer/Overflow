using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NodeManager : MonoBehaviour {

	public HashSet<Node> selectedNodes;
	public int sum = 7;
	RaycastHit hit;

	// Use this for initialization
	void Start () {
		selectedNodes = new HashSet<Node> ();
	}

	void Update(){
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
						Debug.Log ("yeaaah");
					} else
						Debug.Log ("nooooo");

					selectedNodes.Clear ();
					temp = 0;
					break;
				}
			}
		}
	}
}
