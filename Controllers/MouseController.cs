using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour {

	Vector3 lastFramePosition;
	Vector3 currFramePosition;
	Vector3 dragStartPosition;

	int mouse_z = 0;

	bool isDragging = false;

	Plane[] planes;

	public GameObject cursorMarker;
	List<GameObject> dragMarkersGameObjects;

	// Use this for initialization
	void Start () {
		planes = new Plane[WorldController.Instance.world.Depth];
		// creating the planes for mouse interaction with a world layer
		for (int i = 0; i < planes.Length; i++) {
			planes[i] = new Plane( new Vector3(0,i,1), new Vector3(1,i,0), new Vector3(-1,i,0) );
		}
		dragMarkersGameObjects = new List<GameObject>();
	}

	void Update () {
		
		currFramePosition = GetCurrentMousePosition ();

		UpdateDragging ();
		UpdatePanning();

		lastFramePosition = GetCurrentMousePosition();

		// changing te mouse_z - meaning  what layer the mouse cursor will intersect
		//(world z axis, but really y axis... this is dumb)
		if (Input.GetKeyDown(KeyCode.A)) mouse_z++;
		if (Input.GetKeyDown(KeyCode.Z)) mouse_z--;
		mouse_z = Mathf.Clamp(mouse_z, 0, WorldController.Instance.world.Depth - 1);

	}

	// send a ray to intersect the plane and get back the position of intersection
	Vector3 GetCurrentMousePosition () {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow);
		float rayDistance;
		planes[mouse_z].Raycast(ray, out rayDistance);
		Vector3 pos = ray.GetPoint(rayDistance);
		return pos;
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
					Tile t = WorldController.Instance.world.GetTileAt (x, y, mouse_z);
					if (t != null) {
						GameObject go = SimplePool.Spawn (cursorMarker, new Vector3 (x, mouse_z + 0.01f, y), Quaternion.identity);
						go.transform.SetParent (this.transform, true);
						dragMarkersGameObjects.Add (go);
					}
				}
				
			}
		}
		// creating the wanted objects 
		if (Input.GetMouseButtonUp (0) && isDragging) {

			isDragging = false;
			BuildController bc = GameObject.FindObjectOfType<BuildController>();
			
			for (int x = start_x; x <= end_x; x++) {
				for (int y = start_y; y <= end_y; y++) {
					Tile t = WorldController.Instance.world.GetTileAt (x, y, mouse_z);
					if (t != null) {
						// call the build controller
						bc.BuildStuff(t);
					}
				}
			}
		}
	}

	void UpdatePanning () {
		// Panning camera if mouse buttons are held

		float maxHeight = 25f;
		float minHeigt = 4f;
		if (Input.GetMouseButton (1) || Input.GetMouseButton (2)) {

			Vector3 diff = lastFramePosition - currFramePosition;
			Camera.main.transform.Translate (diff, Space.World);
		}

		float d = Input.GetAxis ("Mouse ScrollWheel") * Camera.main.transform.position.y/2;
		Vector3 camPos = Camera.main.transform.position;

		if ((camPos.y - d <= minHeigt || camPos.y - d >= maxHeight) == false) {
			Camera.main.transform.Translate (Vector3.forward * d, Space.Self);
		}

		camPos = new Vector3(camPos.x, Mathf.Clamp(camPos.y, minHeigt, maxHeight), camPos.z);
	}


	void OnImmovableJobComplete (string immovableType, Tile t) {
		WorldController.Instance.world.PlaceImmovable(immovableType, t);

	}

}