using UnityEngine;
using System.Collections;

public class CoinManager : MonoBehaviour {
	
	public static void Deposit(int amount){
		PlayerPrefs.SetInt ("Coins", PlayerPrefs.GetInt ("Coins") + amount);
	}

	public static void Withdraw(int amount){
		PlayerPrefs.SetInt ("Coins", PlayerPrefs.GetInt ("Coins") - amount);
	}

}
