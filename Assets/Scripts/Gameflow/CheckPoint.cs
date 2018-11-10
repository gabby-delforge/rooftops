using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour {

	private GameScript gameScript;
	private GameObject player;
	public bool isActive;
	public bool hasAnimated;
	static AudioClip checkpointSound;

	void Start() {
		player = GameObject.Find ("Player");
		gameScript = GameObject.Find ("GameController").GetComponent<GameScript> ();
		isActive = false;
		hasAnimated = false;
	}
		
	void OnTriggerEnter2D(Collider2D col) {
		if (col.gameObject.tag == "Player") {
			if (isActive) {
				checkpointSound = Resources.Load<AudioClip> ("Sound/SFX/checkpointsound");
				if (!hasAnimated) {
					gameScript.StartCoroutine ("checkPointGUIAnimate");
					AudioSource.PlayClipAtPoint (checkpointSound, Camera.main.transform.position, 0.5f);
					hasAnimated = true;
				}
				player.GetComponent<PlayerMovement> ().setSpawn (gameObject);
			}
		}
	}
}
