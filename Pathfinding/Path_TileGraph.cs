using UnityEngine;
using System.Collections.Generic;

public class Path_TileGraph {

	public Dictionary<Tile, Path_Node<Tile>> nodes;

	public Path_TileGraph (World world) {

//		Debug.Log ("Path_TileGraph");
		// looping thourh all the tiles in the world.
		// for each tile, create a node
		// do not create a node for a blocked tile

		nodes = new Dictionary<Tile, Path_Node<Tile>> ();

		for (int x = 0; x < world.Width; x++) {
			for (int y = 0; y < world.Height; y++) {
				for (int z = 0; z < world.Depth; z++) {
					Tile t = world.GetTileAt (x, y, z);

					// tiles with a movement cost of 0 are unwalkable
					//if (t.movementCost > 0) {
					Path_Node<Tile> n = new Path_Node<Tile> ();
					n.data = t;
					nodes.Add (t, n);
					//}
				}
					
			}
		}

//		Debug.Log ("Path_TileGraph: Created " + nodes.Count + " nodes.");

		// now loop through all nodes again
		// and create edges for neighbors

		int edgeCount = 0;

		foreach (Tile t in nodes.Keys) {
			// Get a list of neighbors for the tile
			// for every walkable neighbor, create an edge
			Path_Node<Tile> n = nodes [t];

			List<Path_Edge<Tile>> edges = new List<Path_Edge<Tile>> ();

			Tile[] neighbors = t.GetNeighbors (false);

			for (int i = 0; i < neighbors.Length; i++) {
				if (neighbors [i] != null && neighbors [i].movementCost > 0) {
					// this neighbor exists and is walkable so crate an edge for it

					// removed because going only in straight lines
//					if (IsClippingCorner (t, neighbors [i])) {
//						continue;
//					}

					Path_Edge<Tile> e = new Path_Edge<Tile>();
					e.cost = neighbors[i].movementCost;
					edges.Add(e);
					e.node = nodes[ neighbors[i] ];

					edgeCount++;
				}
			}

			n.edges = edges.ToArray();

		}

//		Debug.Log("Path_TileGraph: Created "+edgeCount+" edges.");
	} 

//	bool IsClippingCorner (Tile curr, Tile neigh) {
//
//		int dX = curr.X - neigh.X;
//		int dY = curr.Y - neigh.Y;
//		int dZ = curr.Z - neigh.Z;
//
//		if (Mathf.Abs (dX) + Mathf.Abs (dY) == 2) {
//
//			if (curr.world.GetTileAt(curr.X - dX, curr.Y, curr.Z).movementCost == 0) return true;
//			if (curr.world.GetTileAt(curr.X, curr.Y - dY, curr.Z).movementCost == 0) return true;
//		}
//
//		return false;
//	}
}

