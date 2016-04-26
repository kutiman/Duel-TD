using UnityEngine;
using System.Collections.Generic;

public class Path_AStar {

	Queue<Tile> path;

	public Path_AStar (World world, Tile tileStart, Tile tileEnd) {

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

		List<Path_Node<Tile>> OpenSet = new List<Path_Node<Tile>> ();
		OpenSet.Add (start);

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
			
		}

	}

	float heuristc_cost_estimate (Path_Node<Tile> a, Path_Node<Tile> b) {
		return Mathf.Sqrt (
			Mathf.Pow(a.data.X - b.data.X, 2) +
			Mathf.Pow(a.data.Y - b.data.Y, 2)
		);
	}

}

