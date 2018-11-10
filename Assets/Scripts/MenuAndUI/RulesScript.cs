using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RulesScript : MonoBehaviour {

	public Text rulesText;
	public float speed = 0.01F;
	public GameObject startButton;
	[TextArea(15,20)]
	public string textToAnimate;
	private string str;

	void Start () {

		//textToAnimate = textToAnimate.Replace ("\\n", "\n");

			StartCoroutine (AnimateText (textToAnimate));

		
	}
	
	IEnumerator AnimateText(string text) {
		int i = 0;
		str = "";
		while (i < text.Length) {
			str += text [i++];
			rulesText.text = str;
			yield return new WaitForSeconds (Random.Range(speed/2, speed*2));
		}
		startButton.SetActive (true);

	}


}
