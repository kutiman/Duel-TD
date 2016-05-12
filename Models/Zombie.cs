using UnityEngine;
using System.Collections;
using System;

public class Zombie {

	public string zombieType {get; protected set;}
	
	public Tile currTile {get; protected set;}
	Tile destTile;
	Tile nextTile;
	Path_AStar pathAStar;

	PlayerController enemy;

	float movementPercentage;
	float speed = 2f;

	Action<Zombie> cbZombieMoved;
	Action<Zombie> cbZombieReachedEnd;


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


	public Zombie (Tile tile, string zombieType, PlayerController enemy) {
		currTile = nextTile = tile;
		destTile = enemy.homebase.tile;
		this.zombieType = zombieType;
		this.enemy = enemy;
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
				if (pathAStar.Length () == 0) {
					//Debug.LogError ("pathAStar returned no path to destination");
					// FIXME: the job should be re-enqued
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

}
