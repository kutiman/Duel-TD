using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using System.Xml.Serialization;
using System.IO;

public class WorldController : MonoBehaviour {

	public static WorldController Instance { get; protected set; }
	
	public World world {get; protected set;}

	static bool loadWorld = false;

	void OnEnable () {

		if (Instance != null) Debug.Log ("Too many world controllers");

		Instance = this;

		// create a new world with tiles
		if (loadWorld == true) {
			loadWorld = false;
			CreateWorldFromSave ();
		}
		else {
			CreateEmptyWorld ();
		}
	}

	public void Update () {
		world.Update(Time.deltaTime);
	}

	void CreateEmptyWorld () {
		world = new World (100, 100);

		// setting the camera to the middle of the world
		Camera.main.transform.position = new Vector3 (world.Width/2, Camera.main.transform.position.y, world.Height/2 + Camera.main.transform.position.z);
	}

	public void NewWorld () {
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public Tile GetTileAtWorldCoord (Vector3 coord) {
		int x = Mathf.FloorToInt(coord.x);
		int y = Mathf.FloorToInt(coord.z);

		return world.GetTileAt(x, y);
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	///
	///												SAVE & LOAD
	///
	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public void SaveWorld () {
		XmlSerializer serializer = new XmlSerializer(typeof(World));
		TextWriter writer = new StringWriter();
		serializer.Serialize(writer, world);
		writer.Close();

		Debug.Log(writer.ToString());

		PlayerPrefs.SetString("SaveGame00", writer.ToString());
	}

	public void LoadWorld () {
		
		loadWorld = true;
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	void CreateWorldFromSave () {

		XmlSerializer serializer = new XmlSerializer(typeof(World));
		TextReader reader = new StringReader(PlayerPrefs.GetString("SaveGame00"));
		Debug.Log(reader.ToString());
		world = (World) serializer.Deserialize(reader);
		reader.Close();

		// setting the camera to the middle of the world
		Camera.main.transform.position = new Vector3 (world.Width/2, Camera.main.transform.position.y, world.Height/2 + Camera.main.transform.position.z);
	}

}
