using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	public Text highScoreText;
	// Use this for initialization
	void Start () {
		PlayerPrefs.SetInt ("started", 0);
	}
	// Update is called once per frame
	void Update () {
		if (PlayerPrefs.GetInt("started") == 1) {
			highScoreText.text = "Highscore : " + PlayerPrefs.GetInt ("highscore");
		}
		
	}
}
