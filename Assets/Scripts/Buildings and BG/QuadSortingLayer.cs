using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadSortingLayer : MonoBehaviour {

	public string sortingLayer;
	public int orderInLayer;
	// Use this for initialization
	void Awake () {
		setSortingLayer ();
	}
	[ContextMenu ("Update Sorting Layer settings")]
	void updateSortingLayerSettings () {
		setSortingLayer ();
	}
	
	private void setSortingLayer() {
		Renderer rend = GetComponent<Renderer> ();
		rend.sortingLayerName = sortingLayer;
		rend.sortingOrder = orderInLayer;
	}
}
