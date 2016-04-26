using UnityEngine;
using System.Collections.Generic;

public class Path_TileGraph {

	public Dictionary<Tile, Path_Node<Tile>> nodes;

	public Path_TileGraph (World world) {
		// looping thourh all the tiles in the world.
		// for each tile, create a node
		// do not create a node for a blocked tile

		nodes = new Dictionary<Tile, Path_Node<Tile>> ();

		for (int x = 0; x < world.Width; x++) {
			for (int y = 0; y < world.Height; y++) {

				Tile t = world.GetTileAt (x, y);

				// tiles with a movement cost of 0 are unwalkable
				if (t.movementCost > 0) {
					Path_Node<Tile> n = new Path_Node<Tile> ();
					n.data = t;
					nodes.Add (t, n);
				}
			}
		}

		// now loop through all nodes again
		// and create edges for neighbors

		foreach (Tile t in nodes.Keys) {
			// Get a list of neighbors for the tile
			// for every walkable neighbor, create an edge
			Path_Node<Tile> n = nodes [t];

			List<Path_Edge<Tile>> edges = new List<Path_Edge<Tile>>();

			Tile[] neighbors = t.GetNeighbors (true);

			for (int i = 0; i < neighbors.Length; i++) {
				if (neighbors [i] != null && neighbors [i].movementCost > 0) {
					// this neighbor exists and is walkable so crate an edge for it

					Path_Edge<Tile> e = new Path_Edge<Tile>();
					e.cost = neighbors[i].movementCost;
					edges.Add(e);
				}
			}

			n.edges = edges.ToArray();

		}
	} 
}

