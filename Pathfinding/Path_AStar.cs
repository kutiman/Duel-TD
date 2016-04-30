using UnityEngine;
using System.Collections.Generic;
using Priority_Queue;
using System.Linq;

public class Path_AStar {

	Queue<Tile> path;

	public Path_AStar (World world, Tile tileStart, Tile tileEnd) {

		if (world.tileGraph == null) {
			world.tileGraph = new Path_TileGraph (world);
		}

		Dictionary<Tile, Path_Node<Tile>> nodes = world.tileGraph.nodes;

		if (nodes.ContainsKey (tileStart) == false) {
			Debug.LogError ("Path_AStar: the start tile does not exist in the nodes list");
			return;
		}

		if (nodes.ContainsKey (tileEnd) == false) {
			Debug.LogError ("Path_AStar: the end tile does not exist in the nodes list");
			return;
		}

		Path_Node<Tile> start = nodes [tileStart];
		Path_Node<Tile> goal = nodes [tileEnd];

		List<Path_Node<Tile>> ClosedSet = new List<Path_Node<Tile>> ();

//		List<Path_Node<Tile>> OpenSet = new List<Path_Node<Tile>> ();
//		OpenSet.Add (start);

		SimplePriorityQueue<Path_Node<Tile>> OpenSet = new SimplePriorityQueue<Path_Node<Tile>> ();
		OpenSet.Enqueue (start, 0);

		Dictionary<Path_Node<Tile>, Path_Node<Tile>> Came_From = new Dictionary<Path_Node<Tile>, Path_Node<Tile>> ();

		Dictionary<Path_Node<Tile>, float> g_score = new Dictionary<Path_Node<Tile>, float> ();
		foreach (Path_Node<Tile> n in nodes.Values) {
			g_score [n] = Mathf.Infinity;
		}
		g_score [start] = 0;

		Dictionary<Path_Node<Tile>, float> f_score = new Dictionary<Path_Node<Tile>, float> ();
		foreach (Path_Node<Tile> n in nodes.Values) {
			f_score [n] = Mathf.Infinity;
		}
		f_score [start] = heuristc_cost_estimate (start, goal);

		while (OpenSet.Count > 0) {
			Path_Node<Tile> current = OpenSet.Dequeue ();

			if (current == goal) {
				// we have reached our goal
				// convert this into an actual sequence of tiles
				// then end the search
				reconstruct_path (Came_From, current);
				return;
			}

			ClosedSet.Add (current);

			foreach (Path_Edge<Tile> edge_neighbor in current.edges) {
				Path_Node<Tile> neighbor = edge_neighbor.node;

				if (ClosedSet.Contains (neighbor) == true) {
					// ignore this already completed neighbor
					continue;
				}

				float movement_cost_to_neighbor = neighbor.data.movementCost * heuristc_cost_estimate (current, neighbor);

				float tentative_g_score = g_score [current] + movement_cost_to_neighbor;

				if (OpenSet.Contains (neighbor) && tentative_g_score >= g_score [neighbor]) {
					continue;
				}

				Came_From [neighbor] = current;
				g_score [neighbor] = tentative_g_score;
				f_score [neighbor] = g_score [neighbor] + heuristc_cost_estimate (neighbor, goal);

				if (OpenSet.Contains (neighbor) == false) {
					OpenSet.Enqueue (neighbor, f_score [neighbor]);
				}
				else {
					OpenSet.UpdatePriority(neighbor, f_score [neighbor]);
				}
			}
		}

	}

	void reconstruct_path (
		Dictionary<Path_Node<Tile>, Path_Node<Tile>> Came_From,
		Path_Node<Tile> current
	) {
		// at this point, current is he goal
		// so reconstruct the map from end to start

		Queue<Tile> total_path = new Queue<Tile>();
		total_path.Enqueue(current.data);

		while (Came_From.ContainsKey (current)) {
			// Came_Form is a map, where the key-value relation
			// that means: some_node => we got the from this node
			current = Came_From[current];
			total_path.Enqueue(current.data);
		}

		path = new Queue<Tile> (total_path.Reverse());
	}

	float heuristc_cost_estimate (Path_Node<Tile> a, Path_Node<Tile> b) {
		return Vector3.Distance(new Vector3 (a.data.X, a.data.Y, a.data.Z), new Vector3 (b.data.X, b.data.Y, b.data.Z));
	}

	public int Length () {
		if (path == null) return 0;

		return path.Count;
	}

	public Tile Dequeue () {
		return path.Dequeue();
	}

}