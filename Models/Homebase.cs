using UnityEngine;
using System.Collections;
using System;

public class Homebase {

	public Tile tile;

	int maxHealth;
	int health;

	public int Health {
		get { return health;}
		set { 
			if (value <= 0) {
				health = 0;
				if (cbBaseDestroyed != null) {
					cbBaseDestroyed();
				}
			}
			if (value > maxHealth) {
				health = maxHealth;
			}
		}
	}

	Action cbBaseDestroyed;

	public Homebase (Tile t, int health = 100) {
		this.tile = t;
		this.maxHealth = this.health = health;
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
