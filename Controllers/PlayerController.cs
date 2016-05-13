using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public World world { get { return WorldController.Instance.world; } }

	public Inventory inventory;
	public Homebase homebase;

	void Start () {
		inventory = new Inventory();
		// FIXME: change from hardcoded to something better
		homebase = new Homebase(world.GetTileAt(world.Width-1, (world.Height-1)/2));
		//Debug.Log(homebase.tile.X +"_"+ homebase.tile.Y);
		PlaceHomebase (homebase);
	}

	void PlaceHomebase (Homebase hb) {
		 world.PlaceImmovable("Homebase", hb.tile);
	}
}
