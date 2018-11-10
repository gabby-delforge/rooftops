using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

	public AudioClip NewMusic;
	GameObject go;
	void Awake ()
	{
		go = GameObject.Find("GameMusic"); //Finds the game object called Game Music, if it goes by a different name, change this.
		go.GetComponent<AudioSource>().clip = NewMusic; //Replaces the old audio with the new one set in the inspector.
		go.GetComponent<AudioSource>().Play(); //Plays the audio.
	}
}