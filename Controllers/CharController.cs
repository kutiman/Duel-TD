﻿using UnityEngine;
using System.Collections.Generic;

public class CharController : MonoBehaviour {

//	public World world { get { return WorldController.Instance.world; } }

//	Dictionary <Character, GameObject> CharacterGameObjectMap;
//	public Dictionary <string, GameObject> CharactersMap { get; protected set; }

	/// <summary>
	/// this list is to be populated in the inspector.
	/// creates a corresponding map item in CharacterGameObjectMap
	/// </summary>
//	public GameObject[] charactersList;


//	void Start () {
//		CharacterGameObjectMap = new Dictionary<Character, GameObject>();
//		CharactersMap = Utilities.PopulateGameObjectsDictionary(charactersList);
//		world.RegisterCharacterCreated (OnCharacterCreated);
//
//		foreach(Character c in world.characters) {
//			OnCharacterCreated(c);
//		}
//	} 

//	public void OnCharacterCreated (Character c) {
//
//		GameObject char_go = Instantiate(CharactersMap[c.characterType]);
//
//		CharacterGameObjectMap.Add(c, char_go);
//
//		char_go.name = c.characterType;
//		char_go.transform.position = new Vector3(c.X, 0, c.Y);
//		char_go.transform.SetParent(this.transform, true);
//
//		c.RegisterCharacterMovedCallback(OnCharacterMoved);
//	}
//
//	void OnCharacterMoved (Character c) {
//
//		if (CharacterGameObjectMap.ContainsKey (c) == false) {
//			Debug.LogError("Trying to move character not which is not in our map");
//			return;
//		}
//
//		GameObject char_go = CharacterGameObjectMap[c];
//		char_go.transform.position = new Vector3 (c.X, 0, c.Y);
//	}
}