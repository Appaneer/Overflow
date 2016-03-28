using UnityEngine;
using System.Collections;

public class Node : MonoBehaviour {

	public int value;
	public bool isPowerUp;
	public powerUp myPowerUp;

	public int Activate(){
		if(isPowerUp){
			GameObject[] arr = GameObject.FindGameObjectsWithTag ("Node");
			int temp = 0;
			if (myPowerUp == powerUp.rowEliminator) {
				for(int i = 0; i < arr.Length && temp < 6; i++){
					if(Mathf.Abs(transform.position.y - arr[i].transform.position.y) <= 0.2f){
						Destroy (arr[i]);
						temp++;
					}
				}
			}
			else if(myPowerUp == powerUp.bomb){
				for(int i = 0; i < arr.Length && temp < 9; i++){
					if(Vector2.Distance(transform.position, arr[i].transform.position) <= 1.5f){
						Destroy (arr[i]);
						temp++;
					}
				}
			}

		}
		return value;
	}

}

public enum powerUp
{
	rowEliminator,
	bomb,
	coin
};