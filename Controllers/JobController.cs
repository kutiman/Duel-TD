﻿using UnityEngine;
using System.Collections.Generic;

public class JobController : MonoBehaviour {

	ImmovablesController ic;

	Dictionary<Job, GameObject> jobQueueObjectsMap;

	public Material ghostMaterial;
	
	// Use this for initialization
	void Start () {
		ic = GameObject.FindObjectOfType<ImmovablesController>();
		jobQueueObjectsMap = new Dictionary<Job, GameObject>();

		WorldController.Instance.world.jobQueue.RegisterJobCreatedCallback(OnJobCreated);
	}
	
	void OnJobCreated (Job j) {
		GameObject jobObject = (GameObject) Instantiate(ic.itemsMap[j.immovableType]);
		MeshRenderer mr = jobObject.transform.GetChild(0).GetComponent<MeshRenderer>();
		Destroy(jobObject.transform.GetChild(0).GetComponent<MeshCollider>());

		jobObject.transform.position = new Vector3(j.tile.X, 0, j.tile.Y);
		jobObject.transform.SetParent(this.transform, true);

		if (ghostMaterial != null) {
			Material[] mats = new Material[mr.materials.Length];

			for (int i = 0; i < mats.Length; i++) {
				mats[i] = ghostMaterial;
			}

			mr.materials = mats;
		}


		j.RegisterJobCompleteCallback ( OnJobEnd );
		j.RegisterJobCancelCallback ( OnJobEnd );

	}

	void OnJobEnd (Job j) {
		
	}

}
