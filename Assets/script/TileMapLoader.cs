using UnityEngine;
using System.Collections;

public class TileMapLoader : MonoBehaviour {

	// Use this for initialization
	void Start () {
		// load all tilemaps in this scene
		foreach (TileMap tilemap in FindObjectsOfType<TileMap>()) {
			Debug.Log(tilemap.name);
		}
	}
}
