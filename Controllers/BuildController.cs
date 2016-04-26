using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class BuildController : MonoBehaviour {

	Tile.TileType tileBuildMode = Tile.TileType.Ground;
	string buildmodeObjectType;
	bool buildModeIsObjects = false;

	// Use this for initialization

	public void BuildStuff (Tile t) {
		if (buildModeIsObjects == true) {
			// create an immovable in the tile
			string immovableType = buildmodeObjectType;

			if (WorldController.Instance.world.IsImmovablePositionValid (t)) {

				Job job = new Job (t, immovableType, (theJob) => {WorldController.Instance.world.PlaceImmovable (immovableType, theJob.tile); } );
				WorldController.Instance.world.jobQueue.Enqueue (job);

				t.jobPending = job;

			}
		}
		else {
			// change the tile
			t.Type = tileBuildMode;
		}
	}

	public void SetMode_BuildGround () {
		buildModeIsObjects = false;
		tileBuildMode = Tile.TileType.Ground;
	}

	public void SetMode_BuildImmovable ( string objectType) {
		buildModeIsObjects = true;
		buildmodeObjectType = objectType;
	}

	public void SetMode_Bulldoze () {
		buildModeIsObjects = false;
		tileBuildMode = Tile.TileType.Empty;
	}

	void OnImmovableJobComplete (string immovableType, Tile t) {
		WorldController.Instance.world.PlaceImmovable(immovableType, t);
	}

	//----------------
	// *** Testing ***
	//----------------

	public void PathFindingTest () {
		WorldController.Instance.world.CreateExampleWorld();

		//Path_TileGraph tileGraph = new Path_TileGraph(WorldController.Instance.world);
	}

}





