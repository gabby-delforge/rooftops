using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour {

	public void switchScene(int sceneIndex) {
		if (sceneIndex == 3) {
			if (PlayerPrefs.GetInt ("started") != 1) {
				PlayerPrefs.SetInt ("started", 1);
				SceneManager.LoadScene (2);
			} else {
				SceneManager.LoadScene (sceneIndex);
			} 
		} else {
			SceneManager.LoadScene (sceneIndex);
		}
	}
}

