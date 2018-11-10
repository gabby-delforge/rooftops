using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DigitalRuby.RainMaker
{

public class WeatherController : MonoBehaviour {

	public float probability;
	public RainScript2D rainScript;
	float angle;
	float maxRain;
	float duration;
	public bool raining;

	// Use this for initialization
	void Start () {
		raining = false;
		
	}
	
	// Update is called once per frame
	void Update () {
		if (!raining) {
		float randFloat = Random.Range (0f, 1f);
				if (randFloat <= (probability)) {
					maxRain = Random.Range (0f, 1f);
					duration = Random.Range(0, 0.001f);
					angle = 1 * (Mathf.PI / 180);
					raining = true;
					StartCoroutine (startRain ());
			}
		}
		
	}

	IEnumerator startRain() {
			while (angle < (Mathf.PI)) {
				float intensity = maxRain * Mathf.Sin(angle);
				Debug.Log (intensity);
				rainScript.RainIntensity = intensity;
				angle += duration * (Mathf.PI / 180);
				yield return null;
			}
			rainScript.RainIntensity = 0;
			raining = false;
			yield return null;

	}
		
		
	}
}

