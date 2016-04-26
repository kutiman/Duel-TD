using UnityEngine;
using System.Collections.Generic;
using System;

public class World {

	Tile[,] tiles;
	List<Character> characters;

	public int Width {get; protected set;}
	public int Height {get; protected set;}

	Action<Immovable> cbImmovableCreated;
	Action<Tile> cbTileChanged;
	Action<Character> cbCharacterCreated;

	public JobQueue jobQueue;

	Dictionary<string, Immovable> ImmovablesPrototypes; 

	public World (int width = 30, int height = 30) {

		jobQueue = new JobQueue();

		this.Width = width;
		this.Height = height;

		tiles = new Tile[width, height];

		// creating the tiles
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				tiles[x, y] = new Tile(this, x, y);
				tiles[x, y].RegisterTileChangedCallback ( OnTileChanged );
			}
		}

		// Creating the map of Immovable prototypes
		ImmovablesPrototypes = new Dictionary<string, Immovable>();

		ImmovablesPrototypes.Add ("Barrel", Immovable.CreatePrototype ("Barrel", 0, 1, 1));
		ImmovablesPrototypes.Add ("Tree_Pine", Immovable.CreatePrototype ("Tree_Pine", 0, 1, 1));
		ImmovablesPrototypes.Add ("Cave", Immovable.CreatePrototype ("Cave", 0, 1, 1));
		ImmovablesPrototypes.Add ("Tree_Gum", Immovable.CreatePrototype ("Tree_Gum", 0, 1, 1));

		characters = new List<Character>();

	}

	public void Update (float deltaTime) {
		foreach (Character c in characters) {
			c.Update (deltaTime);
		}
	}

	public Tile GetTileAt (int x, int y) {
		if (x < 0 || x >= Width || y < 0 || y >= Height) {
			//Debug.LogError("Tile ("+x+","+y+") is out of range");
			return null;
		}
		return tiles[x, y];
	}

	public void PlaceImmovable (string objectType, Tile t) {
		if (ImmovablesPrototypes.ContainsKey (objectType) == false) {
			Debug.LogError ("ImmovablesPrototypes doesnt contain an prototype for key " + objectType);
			return;
		}

		Immovable obj = Immovable.PlaceInstance (ImmovablesPrototypes [objectType], t);

		if (obj == null) {
			// Failed to place object -- most likely there was already something there.
			return;
		}

		//in this stage, an immovable already exists in the tile, but it not yet assigned a visual gameobject
		if (cbImmovableCreated != null) {
			cbImmovableCreated (obj);
		}
		
	}

	public Character CreateCharacter (string characterType, Tile t) {
		
		Character c = new Character (t, characterType);
		characters.Add (c);
		if (cbCharacterCreated != null) {
			cbCharacterCreated(c);
		}

		return c;
	}

	public void RegisterImmovableCreated (Action<Immovable> cb) {
		cbImmovableCreated += cb;
	}

	public void UnregisterImmovableCreated (Action<Immovable> cb) {
		cbImmovableCreated -= cb;
	}

	public void RegisterCharacterCreated (Action<Character> cb) {
		cbCharacterCreated += cb;
	}

	public void UnregisterCharacterCreated (Action<Character> cb) {
		cbCharacterCreated -= cb;
	}

	public void RegisterTileChanged (Action<Tile> cb) {
		cbTileChanged += cb;
	}

	public void UnregisterTileChanged (Action<Tile> cb) {
		cbTileChanged -= cb;
	}

	void OnTileChanged(Tile t) {
		if(cbTileChanged == null)
			return;
		cbTileChanged(t);
	}

	public bool IsImmovablePositionValid (Tile t) {
		if (t.jobPending != null || t.Immovable != null) {
			return false;
		}

		return true;
	}

}
