using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour {

	Tile.TileType tileBuildMode = Tile.TileType.Ground;
	string buildmodeObjectType;
	bool buildModeIsObjects = false;


	Vector3 lastFramePosition;
	Vector3 currFramePosition;
	Vector3 dragStartPosition;

	bool isDragging = false;

	Plane plane = new Plane(new Vector3(0,0,1), new Vector3(1,0,0), new Vector3(-1,0,0));

	public GameObject cursorMarker;
	List<GameObject> dragMarkersGameObjects;

	// Use this for initialization
	void Start () {
		dragMarkersGameObjects = new List<GameObject>();
	}

	void Update () {
		
		currFramePosition = GetCurrentMousePosition ();

		// showing the mouse marker
		UpdateCursor ();
		UpdateDragging ();
		UpdatePanning();

		lastFramePosition = GetCurrentMousePosition();

	}

	// send a ray to intersect the plane and get back the position of intersection
	Vector3 GetCurrentMousePosition () {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow);
		float rayDistance;
		plane.Raycast(ray, out rayDistance);
		Vector3 pos = ray.GetPoint(rayDistance);
		return pos;
	}

	void UpdateCursor () {
		// showing the mouse marker
		Tile tileUnderMouse = WorldController.Instance.GetTileAtWorldCoord (currFramePosition);

		if (cursorMarker != null) {

			if (tileUnderMouse != null) {
				cursorMarker.SetActive (true);
				Vector3 markerPosition = new Vector3 (tileUnderMouse.X, 0, tileUnderMouse.Y);
				cursorMarker.transform.position = markerPosition;
			}
			else {
				cursorMarker.SetActive (false);
			}
		}
	}

	void UpdateDragging () {
		// if mouse is over a UI element, escape this
		if (EventSystem.current.IsPointerOverGameObject () && !isDragging) return;

		// the starting position of a drag
		if (Input.GetMouseButtonDown (0)) {
			dragStartPosition = currFramePosition;
			isDragging = true;
		}
		// makeing all positions tile-friendly
		int start_x = Mathf.FloorToInt (dragStartPosition.x);
		int start_y = Mathf.FloorToInt (dragStartPosition.z);
		int end_x = Mathf.FloorToInt (currFramePosition.x);
		int end_y = Mathf.FloorToInt (currFramePosition.z);

		if (end_x < start_x) {
			int tmp = start_x;
			start_x = end_x;
			end_x = tmp;
		}


		if (end_y < start_y) {
			int tmp = start_y;
			start_y = end_y;
			end_y = tmp;
		}
		// destroying markers of dragging
		while (dragMarkersGameObjects.Count > 0) {
			GameObject go = dragMarkersGameObjects [0];
			dragMarkersGameObjects.RemoveAt (0);
			SimplePool.Despawn (go);
		}

		// creating the markers of a dragging action
		if (Input.GetMouseButton (0) && isDragging) {

			for (int x = start_x; x <= end_x; x++) {
				for (int y = start_y; y <= end_y; y++) {
					Tile t = WorldController.Instance.World.GetTileAt (x, y);
					if (t != null) {
						GameObject go = SimplePool.Spawn (cursorMarker, new Vector3 (x, 0.01f, y), Quaternion.identity);
						go.transform.SetParent (this.transform, true);
						dragMarkersGameObjects.Add (go);
					}
				}
				
			}
		}
		// creating the wanted objects 
		if (Input.GetMouseButtonUp (0) && isDragging) {

			isDragging = false;
			
			for (int x = start_x; x <= end_x; x++) {
				for (int y = start_y; y <= end_y; y++) {
					Tile t = WorldController.Instance.World.GetTileAt (x, y);
					if (t != null) {
						if (buildModeIsObjects == true) {
							// create an installed object in the tile
							WorldController.Instance.World.PlaceInstalledObject(buildmodeObjectType, t);
						}
						else {
							// change the tile
							t.Type = tileBuildMode;
						}
					}
				}
			}
		}
	}

	void UpdatePanning () {
		// Panning camera if mouse buttons are held
		if (Input.GetMouseButton (1) || Input.GetMouseButton (2)) {

			Vector3 diff = lastFramePosition - currFramePosition;
			Camera.main.transform.Translate(diff, Space.World);
		}

		float d = Input.GetAxis("Mouse ScrollWheel") * Camera.main.transform.position.y;
		Camera.main.transform.Translate(new Vector3(0, d, 0), Space.World);

		Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Mathf.Clamp(Camera.main.transform.position.y, 5f, 25f), Camera.main.transform.position.z);
	}

	public void SetMode_BuildGround () {
		buildModeIsObjects = false;
		tileBuildMode = Tile.TileType.Ground;
	}

	public void SetMode_BuildInstalledObject ( string objectType) {
		buildModeIsObjects = true;
		buildmodeObjectType = objectType;
	}

	public void SetMode_Bulldoze () {
		buildModeIsObjects = false;
		tileBuildMode = Tile.TileType.Empty;
	}

}





