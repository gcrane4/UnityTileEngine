using UnityEngine;
using System.Collections;

public class AutoTileMap : TileMap {
	// Use this for initialization
	void Start () {
		for (int y = mapHeight; y > -1; y--) {
			for (int x = 0; x < mapWidth; x++) {
				DrawTile(x, y, 42);
			}
		}
	}
}
