using UnityEngine;
using System.Collections.Generic;
using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

public class World : IXmlSerializable {

	Tile[,] tiles;
	public List<Character> characters;
	public List<Immovable> immovables;

	// path used to navigate the world
	public Path_TileGraph tileGraph;

	public int Width {get; protected set;}
	public int Height {get; protected set;}

	Action<Immovable> cbImmovableCreated;
	Action<Tile> cbTileChanged;
	Action<Character> cbCharacterCreated;

	public JobQueue jobQueue;

	Dictionary<string, Immovable> immovablesPrototypes; 

	public World (int width, int height) {

		SetupWorld(width, height);

	}

	void SetupWorld (int width, int height) {

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

		CreateImmovablePrototypes();

		characters = new List<Character>();
		immovables = new List<Immovable>();

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
		}

		return imvb;
		
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

		InvalidateTileGraph();
	}

	public void InvalidateTileGraph () {
		tileGraph = null;
	}

	public bool IsImmovablePositionValid (Tile t) {
		if (t.jobPending != null || t.immovable != null) {
			return false;
		}

		return true;
	}

	void CreateImmovablePrototypes () {

		immovablesPrototypes = new Dictionary<string, Immovable>();

		immovablesPrototypes.Add ("Barrel", Immovable.CreatePrototype ("Barrel", 0, 1, 1));
		immovablesPrototypes.Add ("Tree_Pine", Immovable.CreatePrototype ("Tree_Pine", 0, 1, 1));
		immovablesPrototypes.Add ("Cave", Immovable.CreatePrototype ("Cave", 0, 1, 1));
		immovablesPrototypes.Add ("Tree_Gum", Immovable.CreatePrototype ("Tree_Gum", 0, 1, 1));

	}

	//----------------
	// *** Testing ***
	//----------------

	public void CreateExampleWorld () {
		for (int x = 0; x < Width; x++) {
			for (int y = 0; y < Height; y++) {
				if ((x % 5 == 3 && y % 7 < 6) || (y % 5 == 2 && x % 5 < 4)) {
					PlaceImmovable("Barrel", GetTileAt(x, y));

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
		writer.WriteAttributeString ("Width", Width.ToString());
		writer.WriteAttributeString ("Height", Height.ToString());

		writer.WriteStartElement("Tiles");
		for (int x = 0; x < Width; x++) {
			for (int y = 0; y < Height; y++) {
				writer.WriteStartElement("Tile");
				tiles[x, y].WriteXml(writer);
				writer.WriteEndElement();
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

		writer.WriteStartElement("Characters");
		foreach (Character c in characters) {
			writer.WriteStartElement("Character");
			c.WriteXml(writer);
			writer.WriteEndElement();
			
		}
		writer.WriteEndElement();
	}

	public void ReadXml (XmlReader reader) {
		// load game info here
		int width = int.Parse (reader.GetAttribute ("Width"));
		int height = int.Parse (reader.GetAttribute ("Height"));

		Debug.Log("Reading the tilessssssssssssssssssssss");
		SetupWorld (width, height);

		while (reader.Read()) {
			switch (reader.Name) {
				case "Tiles":
					ReadXml_Tiles(reader);
					break;
				case "Immovables":
					ReadXml_Immovables(reader);
					break;
				case "Characters":
					ReadXml_Characters(reader);
					break;
			}
		}
	}

	void ReadXml_Tiles (XmlReader reader) {
		while (reader.Read ()) {
			if (reader.Name != "Tile")
				return;

			int x = int.Parse (reader.GetAttribute ("X"));
			int y = int.Parse (reader.GetAttribute ("Y"));
			tiles[x, y].ReadXml(reader);
		}
	}

	void ReadXml_Immovables (XmlReader reader) {
		while (reader.Read ()) {
			if (reader.Name != "Immovable")
				return;

			int x = int.Parse (reader.GetAttribute ("X"));
			int y = int.Parse (reader.GetAttribute ("Y"));

			Immovable imvb = PlaceImmovable(reader.GetAttribute("ObjectType"), tiles[x, y]);
			imvb.ReadXml(reader);
		}
	}

	void ReadXml_Characters (XmlReader reader) {
		while (reader.Read ()) {
			if (reader.Name != "Character")
				return;

			int x = int.Parse (reader.GetAttribute ("X"));
			int y = int.Parse (reader.GetAttribute ("Y"));

			Character c = CreateCharacter(reader.GetAttribute("CharacterType"), tiles[x, y]);
			c.ReadXml(reader);
		}
	}

}
