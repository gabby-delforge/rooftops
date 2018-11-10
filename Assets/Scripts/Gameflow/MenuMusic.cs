using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuMusic : MonoBehaviour {

	static bool AudioBegin = false; 
	void Awake()
	{
		if (!AudioBegin) {
			GetComponent<AudioSource>().Play ();
			DontDestroyOnLoad (gameObject);
			AudioBegin = true;
		} 
	}
	void Update () {
		if(Application.loadedLevelName == "Main")
		{
			GetComponent<AudioSource>().Stop();
			AudioBegin = false;
		}
	}
}