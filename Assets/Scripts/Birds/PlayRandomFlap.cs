using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayRandomFlap : MonoBehaviour {
	private AudioClip[] flaps;
	public const int NumFlapFiles = 3;

	void Start()
	{
		flaps = new AudioClip[NumFlapFiles];
		for (int i = 1; i <= NumFlapFiles; i++)
		{
			flaps[i - 1] = Resources.Load<AudioClip>("Sound/SFX/Birdflaps/" + i);
		}

	}

	public void RandomFlap() {
		int FileToPlay = (int) Random.Range(0, NumFlapFiles);
		Debug.Log ("Playing flaps");
		AudioSource.PlayClipAtPoint(flaps[FileToPlay], transform.position);
	}
}
