using UnityEngine;
using System.Collections;
using System;

public class Character {

	public string characterType {get; protected set;}
	
	public Tile currTile {get; protected set;}
	public Tile destTile {get; set;}

	float movementPercentage;
	float speed = 2;

	Action<Character> cbCharacterMoved;
	
	public float X {
		get { 
			return Mathf.Lerp(currTile.X, destTile.X, movementPercentage);
		}
	}

	public float Y {
		get { 
			return Mathf.Lerp(currTile.Y, destTile.Y, movementPercentage);
		}
	}

	public Character (Tile tile, string characterType) {
		currTile = destTile = tile;
		this.characterType = characterType;
	}

	public void SetDestination (Tile tile) {
		if (currTile.IsNeighbor (destTile, true) == false) { 
			Debug.Log("The destination tile is not my neighbor");
		}

		destTile = tile;
	}

	Job myJob;

	public void Update (float deltaTime) {

		// Get a Job!
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
		if (currTile == destTile) {
			if (myJob != null) {
				myJob.DoJob(deltaTime);
			}
			return;
		}

		float distToTravel = Mathf.Sqrt (Mathf.Pow (currTile.X - destTile.X, 2) + Mathf.Pow (currTile.Y - destTile.Y, 2));
		float percThisFram = (speed * deltaTime) / distToTravel;

		movementPercentage += percThisFram;

		if (movementPercentage >= 1) {
			// get the next destination tile from the pathfinding system

			currTile = destTile;
			movementPercentage = 0;
		}

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

