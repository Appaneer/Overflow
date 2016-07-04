using UnityEngine;
using System.Collections;

public class Node : MonoBehaviour {

	/// <summary>
	/// The value of this node
	/// </summary>
	public int value;
	public bool isPowerUp;
	public PowerUp myPowerUp;
	/// <summary>
	/// The semi transparent quad. Used for indicated current node has been selected
	/// </summary>
	public Transform semiTransparentQuad;
	protected Animator anim;

	/// <summary>
	/// The column position of the node, range 1-6, 1 being the far last 6 being the far right
	/// </summary>
	public int col;

	void Start(){
		semiTransparentQuad = GetComponentsInChildren<Transform> () [2];
		anim = gameObject.GetComponent<Animator> ();
		if (value != 0)
			isPowerUp = false;
	}

	/// <summary>
	/// Destroy this instance.
	/// </summary>
	public void Destroy(){
		Destroy (80f);
	}

	public void Destroy(float force){
		if(isPowerUp){
			GameObject[] arr = GameObject.FindGameObjectsWithTag ("Node");
			int temp = 0;
			if(myPowerUp == PowerUp.bomb){
				for(int i = 0; i < arr.Length && temp < 8; i++){
					if(Vector2.Distance(transform.position, arr[i].transform.position) <= 1.8f && transform.position - arr[i].transform.position != Vector3.zero
						&& arr[i].GetComponent<Node>().myPowerUp != PowerUp.bomb){//root 2 + some tolerance
						arr [i].GetComponent<Node> ().Destroy (200f);
						temp++;
					}
				}
			}
			else if(myPowerUp == PowerUp.coin){
				PlayerPrefs.SetInt ("Coins", PlayerPrefs.GetInt ("Coins") + 1);
			}
		}
		if (col != 0) {
			//this means the current level is tetris level but not space level
			--TetrisLevelManager.numberOfNodesInCol[col - 1];
		}
		Destroy (gameObject);
	}	

	/// <summary>
	/// Displaies the semi transparent quad to indicate this node has been selected.
	/// </summary>
	public void DisplayQuad(){
		semiTransparentQuad.localPosition = new Vector3(0f,0f,1f);
	}

	/// <summary>
	/// Hides the semi transparent quad to indicate this node has been deselected.
	/// </summary>
	public void HideQuad(){
		semiTransparentQuad.localPosition = new Vector3(0f,0f,0f);
	}
}
	
public enum PowerUp
{
	bomb,
	coin
};