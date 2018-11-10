using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameScript : MonoBehaviour {

	public GameObject player;
	public Text scoreText;
	public int multiplier;

	public GameObject checkpointGUI;
	private Text checkpointText;
	private int score;
	private int highScore;
	private float initial;
	private float distanceTravelled;
	public float distBetweenCheckpoints;
	private float checkpointPos;
	private GameObject lastCheckpoint;
	private bool checkedNewSpawn;


	void Start () {
		initial = player.transform.position.x;
		checkpointPos = player.transform.position.x;
		distanceTravelled = 0;
		score = 0;
		highScore = 0;
		scoreText.text = "Score : 0";
		checkpointText = checkpointGUI.GetComponent<Text>();
		lastCheckpoint = GameObject.Find("Checkpoint-init");
		checkedNewSpawn = false;
	}


	GameObject findLatestCheckpoint() {
		GameObject[] checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
		float minDist = Mathf.Infinity;
		GameObject closest = null;
		float playerX = player.transform.position.x;
		foreach(GameObject checkpoint in checkpoints) {
			float checkpointX = checkpoint.transform.position.x;

			if(playerX > checkpointX) {
				if(playerX - checkpointX < minDist) {
					closest = checkpoint;
					minDist = playerX - checkpointX;
				}
			}
		}
		
		if(closest == null) {
			return lastCheckpoint;
		}
		return closest;
	}

	GameObject findNextCheckpoint() {
		GameObject[] checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
		float minDist = Mathf.Infinity;
		GameObject next = null;
		float playerX = player.transform.position.x;
		foreach (GameObject checkpoint in checkpoints) {
			float checkpointX = checkpoint.transform.position.x;
			if (playerX < checkpointX) {
				if (checkpointX - playerX > distBetweenCheckpoints && checkpointX - playerX < minDist) {
					next = checkpoint;
					minDist = checkpointX - playerX;
				}
			}
		}
		if(next == null) {
			return lastCheckpoint;
		}
		return next;


	}


	IEnumerator checkPointGUIAnimate() {
		checkpointGUI.SetActive(true);
		char[] cpText = "CHECKPOINT REACHED".ToCharArray();
		for(int i = 0; i < cpText.Length; i += 1) {
			checkpointText.text += cpText[i];
			yield return new WaitForSeconds(.06f);
		}
		yield return new WaitForSeconds(.7f);
		checkpointGUI.SetActive(false);
		checkpointText.text = "";
		checkedNewSpawn = false;
	}

	public void setScore(int i){
		score = i;
	}
	void Update () {
		distanceTravelled = Mathf.Max (distanceTravelled, player.transform.position.x - initial);

		if(!checkedNewSpawn && player.transform.position.x - checkpointPos > distBetweenCheckpoints) {
			checkedNewSpawn = true;
			//lastCheckpoint = findLatestCheckpoint();
			lastCheckpoint = findNextCheckpoint();
			lastCheckpoint.GetComponent<CheckPoint> ().isActive = true;
			lastCheckpoint.transform.GetChild (0).gameObject.SetActive (true);
			checkpointPos = lastCheckpoint.transform.position.x;
			//player.GetComponent<PlayerMovement>().setSpawn(lastCheckpoint);
			//StartCoroutine("checkPointGUIAnimate");
		} 

		score = (int)(distanceTravelled) * multiplier;
		if (score > highScore) {
			highScore = score;
			PlayerPrefs.SetInt ("highscore", highScore);
		}

		scoreText.text = "Score : " + score.ToString();
	}
}
