using UnityEngine;
using System.Collections;
using System;

public class Zombie {

	public string zombieType {get; protected set;}
	
	public Tile currTile {get; protected set;}
	Tile destTile;
	Tile nextTile;
	public Path_AStar pathAStar;

	public PlayerController enemy;

	float movementPercentage;
	float speed;
	public float xp {get; protected set;}
	public float payout {get; protected set;}

	int maxHealth;
	int health;

	public int Health {
		get { return health;}
		set { 
			if (value <= 0) {
				health = 0;
				if (cbZombieKilled != null) {
					cbZombieKilled(this);
				}
			}
			if (value > maxHealth) {
				health = maxHealth;
			}
		}
	}

	Action<Zombie> cbZombieMoved;
	Action<Zombie> cbZombieReachedEnd;
	Action<Zombie> cbZombieKilled;

	// graphics location of the character
	public float X { 
		get { 
			if (nextTile == null) return currTile.X;
			return Mathf.Lerp(currTile.X, nextTile.X, movementPercentage); 
		}
	}
	public float Y { 
		get { 
			if (nextTile == null) return currTile.Y;
			return Mathf.Lerp(currTile.Y, nextTile.Y, movementPercentage); 
		} 
	}

	public Zombie (string zombieType, int maxHealth, int xp, int payout, float speed) {

		this.zombieType = zombieType;
		this.maxHealth = maxHealth;
		this.xp = xp;
		this.payout = payout;
		this.speed = speed;
	}

	/// Copy constructor
	protected Zombie (Zombie other) { 

		this.zombieType = other.zombieType;
		this.maxHealth = this.health = other.maxHealth;
		this.xp = other.xp;
		this.payout = other.payout;
		this.speed = other.speed;

	}

	virtual public Zombie Clone () {
		return new Zombie(this);
	}

	static public Zombie PlaceInstance (Zombie proto, Tile tile, PlayerController enemy) {

		Zombie zom = proto.Clone();
		zom.currTile = zom.nextTile = tile;
		zom.enemy = enemy;
		zom.destTile = zom.enemy.homebase.tile;

		return zom;
	}

	void Update_DoMovement (float deltaTime) {
		if (currTile == destTile) {
			pathAStar = null;
			if (cbZombieReachedEnd != null) {
				cbZombieReachedEnd (this);
			}
			return; // where are already where we want to be
		}

		if (nextTile == null || nextTile == currTile) {
			// get the next tile from the pathfinder

			if (pathAStar == null || pathAStar.Length () == 0) {
				// generate a new path to our destination
				pathAStar = new Path_AStar (WorldController.Instance.world, currTile, destTile);
				Debug.Log("making a path");
				if (pathAStar.Length () == 0) {
					Debug.LogError ("pathAStar returned no path to destination");
					pathAStar = null;
					return;
				}
				nextTile = pathAStar.Dequeue ();
			}
			// FIXME: discarding the first node, the current tile of the character
			// from the pathfinding system. Maybe there's a better way?

			nextTile = pathAStar.Dequeue ();

			if (nextTile == currTile) {
				Debug.LogError ("Update_DoMovement: nextTile is currTile?");
			}
		}

		float distToTravel = Vector2.Distance(new Vector2 (currTile.X, currTile.Y), new Vector2 (nextTile.X, nextTile.Y));
		// float distToTravel = Mathf.Sqrt (Mathf.Pow (currTile.X - nextTile.X, 2) + Mathf.Pow (currTile.Y - nextTile.Y, 2));


		if (nextTile.movementCost == 0) {
		//FIXME : this is getting called too much. shouldnt be at all?
//			Debug.LogError("character trying to traverse an unwalkble tile");
			nextTile = null;
			pathAStar = null;
			return;
		}
		float percThisFram = (speed * deltaTime) / (distToTravel * nextTile.movementCost);

		movementPercentage += percThisFram;

		if (movementPercentage >= 1) {
			// get the next destination tile from the pathfinding system

			currTile = nextTile;
			movementPercentage = 0;
		}

	} 

	public void Update (float deltaTime) {

		Update_DoMovement(deltaTime);

		if (cbZombieMoved != null) {
			cbZombieMoved(this);
		}

	}

	public void RegisterZombieMovedCallback (Action<Zombie> cb) {
		cbZombieMoved += cb;
	}

	public void UnregisterZombieMovedCallback (Action<Zombie> cb) {
		cbZombieMoved -= cb;
	}

	public void RegisterZombieReachedEndCallback (Action<Zombie> cb) {
		cbZombieReachedEnd += cb;
	}

	public void UnregisterZombieReachedEndCallback (Action<Zombie> cb) {
		cbZombieReachedEnd -= cb;
	}

	public void RegisterZombieKilledCallback (Action<Zombie> cb) {
		cbZombieKilled += cb;
	}

	public void UnregisterZombieKilledCallback (Action<Zombie> cb) {
		cbZombieKilled -= cb;
	}

}
