using UnityEngine;
using System.Collections;
using System.Threading;

public class CameraMovement : MonoBehaviour {

	public Vector3 panDir = Vector3.zero;
	public float panSpeed = 8f;

	void Update ()
	{
		if (panDir != Vector3.zero) {
			transform.Translate (panDir * panSpeed * Time.deltaTime, Space.World) ;
		}

		if (Input.GetKey(KeyCode.LeftArrow)) panDir.x = -1;
		else if (Input.GetKey(KeyCode.RightArrow)) panDir.x = 1;
		else panDir.x = 0;
		if (Input.GetKey(KeyCode.DownArrow)) panDir.z = -1;
		else if (Input.GetKey(KeyCode.UpArrow)) panDir.z = 1;
		else panDir.z = 0;

	}


}
