using UnityEngine;
using System.Collections;

public class SoundController : MonoBehaviour {

	public AudioClip[] soundEffects;
	AudioSource audioSource;

	float soundCooldown = 0;

	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource> ();
		if (audioSource == null) {
			audioSource = gameObject.AddComponent<AudioSource>();
		}

		WorldController.Instance.World.RegisterImmovableCreated(OnImmovableCreated);
		WorldController.Instance.World.RegisterTileChanged(OnTileChanged);
	}
	
	// Update is called once per frame
	void Update () {
		soundCooldown -= Time.deltaTime;
	}

	public void OnTileChanged (Tile tile_data) {
		if (soundCooldown > 0) {
			return;
		}

		audioSource.clip = soundEffects [0];
		audioSource.Play ();
		soundCooldown = 0.1f;
	}

	public void OnImmovableCreated (Immovable immovable) {
		if (soundCooldown > 0) {
			return;
		}

		audioSource.clip = soundEffects[1];
		audioSource.Play();
		soundCooldown = 0.1f;
	}
}
