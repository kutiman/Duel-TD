using UnityEngine;
using System.Collections;
using System;

public class Character {

	public string characterType {get; protected set;}
	
	public Tile currTile {get; protected set;}
	Tile destTile;
	Tile nextTile;
	Path_AStar pathAStar;

	float movementPercentage;
	float speed = 4f;

	Action<Character> cbCharacterMoved;
	
	public float X {
		get { 
			return Mathf.Lerp(currTile.X, nextTile.X, movementPercentage);
		}
	}

	public float Y {
		get { 
			return Mathf.Lerp(currTile.Y, nextTile.Y, movementPercentage);
		}
	}

	public Character (Tile tile, string characterType) {
		currTile = destTile = nextTile = tile;
		this.characterType = characterType;
	}

	public void SetDestination (Tile tile) {
		if (currTile.IsNeighbor (tile, true) == false) { 
			Debug.Log("The destination tile is not my neighbor");
		}

		destTile = tile;
	}

	Job myJob;

	void Update_DoJob (float deltaTime) {
		if (myJob == null) {
			myJob = currTile.world.jobQueue.Dequeue ();

			if (myJob != null) {
				// I got a job!
				destTile = myJob.tile;

				// Register the ended job callback
				myJob.RegisterJobCompleteCallback (OnJobEnded);
				myJob.RegisterJobCancelCallback (OnJobEnded);
			}
		}

		// moving the character
		if (myJob != null && currTile == myJob.tile) {
			myJob.DoJob(deltaTime);
		}

	}

	public void AbandonJob() {
		nextTile = destTile = currTile;
		pathAStar = null;
		currTile.world.jobQueue.Enqueue(myJob);
		myJob = null;
	}

	void Update_DoMovement (float deltaTime) {

		if (currTile == destTile) {
			pathAStar = null;
			return; // where are already where we want to be
		}

		if (nextTile == null || nextTile == currTile) {
			// get the next tile from the pathfinder
			if (pathAStar == null || pathAStar.Length() == 0) {
				// generate a new path to our destination
				pathAStar = new Path_AStar (WorldController.Instance.world, currTile, destTile);
				if (pathAStar.Length () == 0) {
					Debug.LogError ("pathAStar returned no path to destination");
					// FIXME: the job should be re-enqued
					AbandonJob();
					pathAStar = null;
					return;
				}
			}

			nextTile = pathAStar.Dequeue ();

			if (nextTile == currTile) {
				Debug.LogError("Update_DoMovement: nextTile is currTile?");
			}
		}

		float distToTravel = Mathf.Sqrt (Mathf.Pow (currTile.X - nextTile.X, 2) + Mathf.Pow (currTile.Y - nextTile.Y, 2));
		float percThisFram = (speed * deltaTime) / distToTravel;

		movementPercentage += percThisFram;

		if (movementPercentage >= 1) {
			// get the next destination tile from the pathfinding system

			currTile = nextTile;
			movementPercentage = 0;
		}

	} 

	public void Update (float deltaTime) {

		Update_DoJob(deltaTime);
		Update_DoMovement(deltaTime);

		if (cbCharacterMoved != null) {
			cbCharacterMoved(this);
		}

	}

	public void RegisterCharacterMovedCallback (Action<Character> cb) {
		cbCharacterMoved += cb;
	}

	public void UnregisterCharacterMovedCallback (Action<Character> cb) {
		cbCharacterMoved -= cb;
	}

	void OnJobEnded (Job j) {
		if (j != myJob) {
			Debug.LogError("character trying to end a job that is not his.");
			return;
		}

		myJob = null;
	}
}

