using UnityEngine;
using System.Collections;
using System;

public class Homebase {

	public Tile tile;

	int maxLives;
	int lives;

	public int Lives {
		get { return lives;}
		set { 
			if (value <= 0) {
				lives = 0;
				if (cbBaseDestroyed != null) {
					cbBaseDestroyed();
				}
				return;
			}
			if (value > maxLives) {
				lives = maxLives;
				return;
			}
			lives = value;
		}
	}

	Action cbBaseDestroyed;

	public Homebase (Tile t, int lives = 20) {
		this.tile = t;
		this.maxLives = this.lives = lives;
	}
	/// <summary>
	/// Registers the base destroyed callback.
	/// </summary>
	/// <param name="cb">Cb.</param>
	public void RegisterBaseDestroyedCallback (Action cb) {
		cbBaseDestroyed += cb;
	}
	/// <summary>
	/// Unregisters the base destroyed callback.
	/// </summary>
	/// <param name="cb">Cb.</param>
	public void UnregisterBaseDestroyedCallback (Action cb) {
		cbBaseDestroyed -= cb;
	}
}
