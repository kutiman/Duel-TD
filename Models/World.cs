using UnityEngine;
using System.Collections.Generic;
using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

public class World : IXmlSerializable {

	Tile[,] tiles;
	public List<Zombie> zombies;
	public List<Immovable> immovables;

	// path used to navigate the world
	public Path_TileGraph tileGraph;

	public int Width {get; protected set;}
	public int Height {get; protected set;}

	Action<Immovable> cbImmovableCreated;
	Action<Tile> cbTileChanged;
	Action<Zombie> cbZombieCreated;

	Dictionary<string, Immovable> immovablesPrototypes; 
	Dictionary<string, Zombie> zombiePrototypes;

	public World (int width, int height) {
		SetupWorld(width, height);
	}

	void SetupWorld (int width, int height) {

		Width = width;
		Height = height;

		tiles = new Tile[width, height];

		// creating the tiles
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				tiles[x, y] = new Tile(this, x, y);
				tiles[x, y].RegisterTileChangedCallback ( OnTileChanged );
			
			}
		}

		CreateImmovablePrototypes();
		CreateZombiePrototypes();

		immovables = new List<Immovable>();
		zombies = new List<Zombie>();

	}

	public void Update (float deltaTime) {
		foreach (Zombie c in zombies) {
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

	public Immovable PlaceImmovable (string objectType, Tile t) {
		if (immovablesPrototypes.ContainsKey (objectType) == false) {
			Debug.LogError ("immovablesPrototypes doesnt contain an prototype for key " + objectType);
			return null;
		}

		Immovable imvb = Immovable.PlaceInstance (immovablesPrototypes [objectType], t);

		if (imvb == null) {
			// Failed to place object -- most likely there was already something there.
			return null;
		}

		immovables.Add(imvb);
		//in this stage, an immovable already exists in the tile, but it not yet assigned a visual gameobject
		if (cbImmovableCreated != null) {
			cbImmovableCreated (imvb);
			InvalidateTileGraph();

			// removing the path of all zombies, making them recalculate the path.
			foreach (Zombie zom in zombies) {
				zom.pathAStar = null;
			}
		}

		return imvb;
		
	}

	public Zombie CreateZombie (string zombieType, Tile t) {

		Debug.Log("Created a zombie");

		if (zombiePrototypes.ContainsKey (zombieType) == false) {
			Debug.LogError ("zombiePrototypes doesnt contain an prototype for key " + zombieType);
			return null;
		}
		PlayerController enemy = GameObject.FindObjectOfType<PlayerController>();

		Zombie zom = Zombie.PlaceInstance (zombiePrototypes [zombieType], t, enemy);

		if (zom == null) {
			return null;
		}

		zombies.Add (zom);
		if (cbZombieCreated != null) {
			cbZombieCreated(zom);
		}

		return zom;
	}

	public void RegisterImmovableCreated (Action<Immovable> cb) {
		cbImmovableCreated += cb;
	}

	public void UnregisterImmovableCreated (Action<Immovable> cb) {
		cbImmovableCreated -= cb;
	}

	public void RegisterZombieCreated (Action<Zombie> cb) {
		cbZombieCreated += cb;
	}

	public void UnregisterZombieCreated (Action<Zombie> cb) {
		cbZombieCreated -= cb;
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

		InvalidateTileGraph();
	}

	public void InvalidateTileGraph () {
		tileGraph = null;
	}

	public bool IsImmovablePositionValid (Tile t) {
		if (t.immovable != null) {
			return false;
		}

		return true;
	}

	void CreateImmovablePrototypes () {

		// FIXME: read from a different file. Maybe xml?
		immovablesPrototypes = new Dictionary<string, Immovable>();

		immovablesPrototypes.Add ("Barrel", new Immovable ("Barrel", 0, 1, 1));
		immovablesPrototypes.Add ("Tree_Pine", new Immovable ("Tree_Pine", 2, 1, 1));
		immovablesPrototypes.Add ("Cave", new Immovable ("Cave", 0, 1, 1));
		immovablesPrototypes.Add ("Tree_Gum", new Immovable ("Tree_Gum", 0.5f, 1, 1));
		immovablesPrototypes.Add ("Turret", new Immovable ("Turret", 0, 1, 1));
		immovablesPrototypes.Add ("Grass", new Immovable ("Grass", 0, 1, 1));
		immovablesPrototypes.Add ("Snow", new Immovable ("Snow", 0, 1, 1));
		immovablesPrototypes.Add ("Homebase", new Immovable ("Homebase", 1, 1, 1));

		immovablesPrototypes["Turret"].imvbParameters["shoot_speed"] = 0;
		immovablesPrototypes["Turret"].updateActions += ImmovableActions.Turret_UpdateAction;

	}

	void CreateZombiePrototypes () {

		// FIXME: read from a different file. Maybe xml?
		zombiePrototypes = new Dictionary<string, Zombie>();

		zombiePrototypes.Add ("Zombie_1", new Zombie ("Zombie_1", 10, 1, 1, 1f));

	}

	//----------------
	// *** Testing ***
	//----------------

	public void CreateExampleWorld () {
		for (int x = 0; x < Width; x++) {
			for (int y = 0; y < Height; y++) {
				if ((x % 5 == 3 && y % 7 < 6) || (y % 5 == 2 && x % 5 < 4)) {
					PlaceImmovable("Grass", GetTileAt(x, y));

				}
			}
		}
	}


	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	///
	///												SAVE & LOAD
	///
	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	// empty constructor for xml serialization
	public World () {
		
	}

	public XmlSchema GetSchema () {
		return null;
	}

	public void WriteXml (XmlWriter writer) {
		// save game info here
		writer.WriteAttributeString ("Width", Width.ToString ());
		writer.WriteAttributeString ("Height", Height.ToString ());

		writer.WriteStartElement ("Tiles");
		for (int x = 0; x < Width; x++) {
			for (int y = 0; y < Height; y++) {
					writer.WriteStartElement ("Tile");
					tiles[x, y].WriteXml (writer);
					writer.WriteEndElement ();
			}
		}
		writer.WriteEndElement();

		writer.WriteStartElement("Immovables");
		foreach (Immovable imvb in immovables) {
			writer.WriteStartElement("Immovable");
			imvb.WriteXml(writer);
			writer.WriteEndElement();
			
		}
		writer.WriteEndElement();
	}

	public void ReadXml (XmlReader reader) {
		Debug.Log("World :: ReadXml");
		// load game info here
		Width = int.Parse (reader.GetAttribute ("Width"));
		Height = int.Parse (reader.GetAttribute ("Height"));


		SetupWorld (Width, Height);

		while (reader.Read()) {
			switch (reader.Name) {
				case "Tiles":
					ReadXml_Tiles(reader);
					break;
				case "Immovables":
					ReadXml_Immovables(reader);
					break;

			}
		}
	}

	void ReadXml_Tiles (XmlReader reader) {
		Debug.Log("ReadXml_Tiles");

		if (reader.ReadToDescendant("Tile")) {

			do {
				int x = int.Parse (reader.GetAttribute ("X"));
				int y = int.Parse (reader.GetAttribute ("Y"));
				tiles[x, y].ReadXml(reader);
			} while(reader.ReadToNextSibling("Tile"));


		}
	}

	void ReadXml_Immovables (XmlReader reader) {
		Debug.Log("ReadXml_Immovables");

		if (reader.ReadToDescendant("Immovable")) {

			do {
				int x = int.Parse (reader.GetAttribute ("X"));
				int y = int.Parse (reader.GetAttribute ("Y"));

				Immovable imvb = PlaceImmovable(reader.GetAttribute("ObjectType"), tiles[x, y]);
				imvb.ReadXml(reader);

			} while(reader.ReadToNextSibling("Immovable"));
		}
	}
}