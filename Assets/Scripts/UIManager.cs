using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public Canvas creditPage;
	public Canvas settingPage;
	public Canvas numberPage;
	public Button numberButton;

	public void LoadPlusLevel(){
		SceneManager.LoadScene ("Level");
	}

	public void ToggleCreditPage(){
		ToggleButton ();
		creditPage.enabled =  creditPage.enabled ? false : true;
	}

	public void ToggleSettingPage(){
		ToggleButton ();
		settingPage.enabled = settingPage.enabled ? false : true;
	}

	public void ToggleNumberPage(){
		ToggleButton ();
		numberPage.enabled = numberPage.enabled ? false : true;
	}

	private void ToggleButton(){
		numberButton.image.enabled = numberButton.enabled ? false : true;
		numberButton.enabled = numberButton.enabled ? false : true;
	}

	public void FacebookShare(){
		
	}
}
