using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class BuildController : MonoBehaviour {

	enum BuildMode {tile, immovable, zombie};
	BuildMode buildMode = BuildMode.tile;

	Tile.TileType tileType = Tile.TileType.Ground;
	string immovableType;
	string zombieType;


	// Use this for initialization

	public void BuildStuff (Tile t) {
		switch (buildMode) {

			case BuildMode.tile:
				t.Type = tileType;
				break;

			case BuildMode.immovable:
				if (WorldController.Instance.world.IsImmovablePositionValid (t)) {
					WorldController.Instance.world.PlaceImmovable(immovableType, t);
				}
				break;

			case BuildMode.zombie:
				if (WorldController.Instance.world.IsImmovablePositionValid (t)) {
					WorldController.Instance.world.CreateZombie(zombieType, t);
				}
				break;

			default:
				break;
		}


	}

	public void SetMode_BuildGround () {
		buildMode = BuildMode.tile;
		tileType = Tile.TileType.Ground;
	}

	public void SetMode_BuildImmovable ( string objectType) {
		buildMode = BuildMode.immovable;
		immovableType = objectType;
	}

	public void SetMode_BuildZombie ( string objectType) {
		buildMode = BuildMode.zombie;
		zombieType = objectType;
	}

	public void SetMode_Bulldoze () {
		buildMode = BuildMode.tile;
		tileType = Tile.TileType.Empty;
	}

//	void OnImmovableJobComplete (string immovableType, Tile t) {
//		WorldController.Instance.world.PlaceImmovable(immovableType, t);
//	}

	//----------------
	// *** Testing ***
	//----------------

	public void PathFindingTest () {
		WorldController.Instance.world.CreateExampleWorld();

		//Path_TileGraph tileGraph = new Path_TileGraph(WorldController.Instance.world);
	}

}





