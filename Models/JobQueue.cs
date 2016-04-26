using UnityEngine;
using System.Collections.Generic;
using System;

public class JobQueue {

	public Queue<Job> jobQueue;

	Action<Job> cbJobCreated;

	public JobQueue () {
		jobQueue = new Queue<Job>();
	}

	public void Enqueue (Job j) {
		jobQueue.Enqueue( j );

		if (cbJobCreated != null) {
			cbJobCreated(j);
		}
	}

	public Job Dequeue () {
		if (jobQueue.Count == 0) {
			return null;
		}

		return jobQueue.Dequeue();
	}

	public void RegisterJobCreatedCallback (Action<Job> cbFunc) {
		cbJobCreated += cbFunc;
	}

	public void UnregisterJobCreatedCallback (Action<Job> cbFunc) {
		cbJobCreated -= cbFunc;
	}

}

