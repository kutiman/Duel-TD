﻿using UnityEngine;
using System.Collections;

public class AutoVerticalSize : MonoBehaviour {

	public float childHeight = 30f;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void AdjustSize () {
		Vector2 size = this.GetComponent<RectTransform>().sizeDelta;
		size.y = this.transform.childCount * childHeight;
		this.GetComponent<RectTransform>().sizeDelta = size;
	}
}
