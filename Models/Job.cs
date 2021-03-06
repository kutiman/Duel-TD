﻿using UnityEngine;
using System.Collections;
using System;

public class Job {

	public Tile tile { get; protected set; }

	public string immovableType { get; protected set; }

	float jobTime;
	Action<Job> cbJobComplete;
	Action<Job> cbJobCancel;

	public Job (Tile tile, string immovableType, Action<Job> cbJobComplete, float jobTime = 0.1f) {
		this.tile = tile;
		this.immovableType = immovableType;
		this.jobTime = jobTime;
		this.cbJobComplete += cbJobComplete; 
	}

	public void RegisterJobCompleteCallback (Action<Job> cb) {
		cbJobComplete += cb;
	}

	public void RegisterJobCancelCallback (Action<Job> cb) {
		cbJobCancel += cb;
	}

	public void UnregisterJobCompleteCallback (Action<Job> cb) {
		cbJobComplete -= cb;
	}

	public void UnregisterJobCancelCallback (Action<Job> cb) {
		cbJobCancel -= cb;
	}

	public void DoJob (float workTime) {
		jobTime -= workTime;

		if (jobTime <= 0) {
			if (cbJobComplete != null) {
				cbJobComplete(this);
			}
		}
	}

	public void CancelJob () {
		if (cbJobCancel != null) {
			cbJobCancel(this);
		}
	}
}
