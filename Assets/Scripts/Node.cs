using UnityEngine;
using System.Collections;

public class Node : MonoBehaviour {

	public int value;
	public bool isPowerUp;
	public PowerUp myPowerUp;

	void Start(){
		if (value != 0)
			isPowerUp = false;
	}

	void OnDestroy(){
		if(isPowerUp){
			GameObject[] arr = GameObject.FindGameObjectsWithTag ("Node");
			int temp = 0;
			if (myPowerUp == PowerUp.horizontal) {
				for(int i = 0; i < arr.Length && temp < 6; i++){
					if(Mathf.Abs(transform.position.y - arr[i].transform.position.y) <= 0.2f){
						Destroy (arr[i]);
						temp++;
					}
				}
			}
			else if (myPowerUp == PowerUp.vertical) {
				for(int i = 0; i < arr.Length && temp < 6; i++){
					if(Mathf.Abs(transform.position.x - arr[i].transform.position.x) <= 0.2f){
						Destroy (arr[i]);
						temp++;
					}
				}
			}
			else if(myPowerUp == PowerUp.bomb){
				for(int i = 0; i < arr.Length && temp < 9; i++){
					if(Vector2.Distance(transform.position, arr[i].transform.position) <= 1.5f){//root 2 + some tolerance
						Destroy (arr[i]);
						temp++;
					}
				}
			}
			else if(myPowerUp == PowerUp.coin){
				CoinManager.Deposit (1);
			}
		}
	}
}

public enum PowerUp
{
	horizontal,
	vertical,
	bomb,
	coin
};