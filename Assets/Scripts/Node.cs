using UnityEngine;
using System.Collections;

public class Node : MonoBehaviour {

	public int value;
	public bool isPowerUp;
	public PowerUp myPowerUp;
	protected Animator anim;

	void Start(){
		anim = gameObject.GetComponent<Animator> ();
		if (value != 0)
			isPowerUp = false;
	}

	public void Destroy(){
		if(isPowerUp){
			GameObject[] arr = GameObject.FindGameObjectsWithTag ("Node");
			int temp = 0;
			if (myPowerUp == PowerUp.horizontal) {
				for(int i = 0; i < arr.Length && temp < 6; i++){
					if(Mathf.Abs(transform.position.y - arr[i].transform.position.y) <= 0.2f && transform.position - arr[i].transform.position != Vector3.zero){
						arr [i].GetComponent<Node> ().Destroy ();
						temp++;
					}
				}
			}
			else if (myPowerUp == PowerUp.vertical) {
				for(int i = 0; i < arr.Length && temp < 6; i++){
					if(Mathf.Abs(transform.position.x - arr[i].transform.position.x) <= 0.2f && transform.position - arr[i].transform.position != Vector3.zero){
						arr [i].GetComponent<Node> ().Destroy ();
						temp++;
					}
				}
			}
			else if(myPowerUp == PowerUp.bomb){
				for(int i = 0; i < arr.Length && temp < 8; i++){
					if(Vector2.Distance(transform.position, arr[i].transform.position) <= 1.5f && transform.position - arr[i].transform.position != Vector3.zero){//root 2 + some tolerance
						arr [i].GetComponent<Node> ().Destroy ();
						temp++;
					}
				}
			}
			else if(myPowerUp == PowerUp.coin){
				CoinManager.Deposit (1);
			}
		}
		StartCoroutine (WaitForSeconds(0.65f));
	}

	IEnumerator WaitForSeconds(float second){
		anim.SetTrigger ("Destroy");
		yield return new WaitForSeconds (second);
		Destroy (gameObject);
	}
}

public enum PowerUp
{
	horizontal,
	vertical,
	bomb,
	coin
};