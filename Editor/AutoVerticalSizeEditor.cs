using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(AutoVerticalSize))]
public class AutoVerticalSizeEditor : Editor {

	public override void OnInspectorGUI () {
		DrawDefaultInspector();

		if (GUILayout.Button("Recalc size")) {
			((AutoVerticalSize) target).AdjustSize();
		}
	}
}
