using UnityEngine;
using System.Collections.Generic;

public class ZombieController : MonoBehaviour {

	public World world { get { return WorldController.Instance.world; } }

	Dictionary <Zombie, GameObject> ZombieGameObjectMap;
	public Dictionary <string, GameObject> ZombiesMap { get; protected set; }

	/// <summary>
	/// this list is to be populated in the inspector.
	/// creates a corresponding map item in the corresponding map
	/// </summary>
	public GameObject[] ZombiesList;


	void Start () {
		ZombieGameObjectMap = new Dictionary<Zombie, GameObject>();
		ZombiesMap = Utilities.PopulateGameObjectsDictionary(ZombiesList);
		world.RegisterZombieCreated (OnZombieCreated);
		//world.RegisterZombieCreated (OnZombieCreated);

		foreach(Zombie zom in world.zombies) {
			OnZombieCreated(zom);
		}
	} 

	public void OnZombieCreated (Zombie zom) {

		GameObject zom_go = Instantiate(ZombiesMap[zom.zombieType]);

		ZombieGameObjectMap.Add(zom, zom_go);

		zom_go.name = zom.zombieType;
		zom_go.transform.position = new Vector3(zom.X, 0, zom.Y);
		zom_go.transform.SetParent(this.transform, true);

		zom.RegisterZombieMovedCallback(OnZombieMoved);
		zom.RegisterZombieKilledCallback(OnZombieKilled);
		zom.RegisterZombieReachedEndCallback(OnZombieReachedEnd);
	}

	void OnZombieMoved (Zombie zom) {

		if (ZombieGameObjectMap.ContainsKey (zom) == false) {
			Debug.LogError("Trying to move character not which is not in our map");
			return;
		}

		GameObject zom_go = ZombieGameObjectMap[zom];
		zom_go.transform.position = new Vector3 (zom.X, 0, zom.Y);
	}

	void OnZombieReachedEnd (Zombie zom) {
		
		if (ZombieGameObjectMap.ContainsKey (zom) == false) {
			Debug.LogError("Trying to kill a zombie which is not in our map");
			return;
		}

		zom.enemy.homebase.Lives--;
		//GameObject zom_go = ZombieGameObjectMap[zom];
		Debug.Log("Lives: " + zom.enemy.homebase.Lives);
		Destroy(ZombieGameObjectMap[zom]);
		ZombieGameObjectMap.Remove(zom);
		world.zombies.Remove(zom);
	}

	void OnZombieKilled (Zombie zom) {

		if (ZombieGameObjectMap.ContainsKey (zom) == false) {
			Debug.LogError("Trying to kill a zombie which is not in our map");
			return;
		}

		ZombieGameObjectMap.Remove(zom);
	}

}