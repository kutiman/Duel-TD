using UnityEngine;
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
		GameObject jobObject = (GameObject)Instantiate (ic.itemsMap [j.immovableType]);

		// FIXME
		if (jobQueueObjectsMap.ContainsKey (j)) {
			Debug.Log("OnJobCreated for job_go already exists -- probably RE-QUEUED");
			return; 
		}

		MeshRenderer mr = jobObject.transform.GetChild(0).GetComponent<MeshRenderer>();
		Destroy(jobObject.transform.GetChild(0).GetComponent<MeshCollider>());

		jobObject.transform.position = new Vector3(j.tile.X, j.tile.Z, j.tile.Y);
		jobObject.transform.SetParent(this.transform, true);

		if (ghostMaterial != null) {
			Material[] mats = new Material[mr.materials.Length];

			for (int i = 0; i < mats.Length; i++) {
				mats[i] = ghostMaterial;
			}

			mr.materials = mats;
		}

		jobQueueObjectsMap.Add(j, jobObject);

		j.RegisterJobCompleteCallback ( OnJobEnd );
		j.RegisterJobCancelCallback ( OnJobEnd );

	}

	void OnJobEnd (Job job) {

		GameObject job_go = jobQueueObjectsMap[job];

		job.UnregisterJobCompleteCallback ( OnJobEnd );
		job.UnregisterJobCancelCallback ( OnJobEnd );

		Destroy(job_go);
	}

}
