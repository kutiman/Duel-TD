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
		world.Update (Time.deltaTime);

		// FIXME: ake out the quitting from here
		if (Input.GetKeyDown (KeyCode.Escape)) {
			QuitGame();
		}
	}

	void CreateEmptyWorld () {
		world = new World (30, 30);
		Debug.Log("Created a new world!");
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

		PlayerPrefs.SetString("SaveGame00", writer.ToString());

		Debug.Log(PlayerPrefs.GetString("SaveGame00"));

		StreamWriter sr = File.CreateText("MyFile.txt");
		sr.WriteLine ( writer.ToString());
        sr.Close();
	}

	public void LoadWorld () {
		
		loadWorld = true;
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	void CreateWorldFromSave () {

		XmlSerializer serializer = new XmlSerializer(typeof(World));
		Debug.Log(PlayerPrefs.GetString("SaveGame00"));
		TextReader reader = new StringReader(PlayerPrefs.GetString("SaveGame00"));
		world = (World) serializer.Deserialize(reader);
		if (world == null) Debug.LogError("world came back null from save file");
		reader.Close();

		// setting the camera to the middle of the world
		Camera.main.transform.position = new Vector3 (world.Width/2, Camera.main.transform.position.y, world.Height/2 + Camera.main.transform.position.z);
	}

	//////////////////////////////////

	public void QuitGame () {
		Application.Quit();
	}

}