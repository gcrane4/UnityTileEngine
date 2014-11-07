using UnityEngine;
using UnityEditor;
using System.Collections;

[RequireComponent (typeof(Atlas))]
public class TileMap : MonoBehaviour {
	/** a two-dimensional array of integers representing tilecodes at each map tile */
	private int[,] map;
	private GameObject[,] tileObjects;
	private Sprite[,] sprites;
	private Atlas atlas;

    // TODO: account for tile atlas padding

	/** the width of the map in tiles */
	public int mapWidth;

	/** the height of the map in tiles */
	public int mapHeight;

	public Rect getTileRect(int tilecode) 
	{
		return new Rect ((tilecode % atlas.RowLength) * atlas.tileWidth, 
		                 ((atlas.ColLength - (tilecode / atlas.RowLength)) * atlas.tileHeight) - atlas.tileHeight, 
		                 atlas.tileWidth, atlas.tileHeight);
	}

    public GameObject DrawTile(int x, int y, int tilecode)
    {
		// alias atlas from Atlas component
		if (atlas == null) {
			atlas = this.GetComponent<Atlas> ();
		}

		// create an empty map if it does not already exist
		if (map == null) {
			map = new int[mapHeight, mapWidth];
			tileObjects = new GameObject[mapHeight, mapWidth];
			sprites = new Sprite[mapHeight, mapWidth];
		}

		if (mapWidth > map.GetLength (1) || mapHeight > map.GetLength (0)) {
			// create a new map that is larger than the original
			int[,] newMap = new int[mapHeight, mapWidth];
			GameObject[,] newTO = new GameObject[mapHeight, mapWidth];
		    Sprite[,] newSprites = new Sprite[mapHeight, mapWidth];

			// copy old map data to new map
			for (int j = 0; j < map.GetLength(0); j++) {
				for (int i = 0; i < map.GetLength(1); i++) {
					newMap[j, i] = map[j, i];
					newTO[j, i] = tileObjects[j, i];
					newSprites[j, i] = sprites[j, i];
				}
			}

			// replace old map with new map by reference
			map = newMap;
			tileObjects = newTO;
			sprites = newSprites;
		}

		// check target against map dimensions
		if (x > -1 && x < mapWidth &&
			y > -1 && y < mapHeight) {
			if (tilecode == -1) 
			{
				// disable this tile
				tileObjects[y, x].renderer.enabled = false;
				return tileObjects[y, x];
			} else if (tileObjects[y, x] != null) {
				tileObjects[y, x].renderer.enabled = true;
			}
			
			if (tilecode > atlas.LastTile) {
				throw new System.IndexOutOfRangeException("Tile code index is out of range of tile atlas.");
			}
			
			// SINGLETON
			// if a sprite exists for this tile already, replace it with a new tile sprite
			if (sprites[y, x] != null) {
				// destroy existing sprite data
				DestroyImmediate(sprites[y, x]);
				// assign new sprite data
				sprites[y, x] = atlas.GetTile(tilecode);
				// update GameObject sprite with new sprite
				((SpriteRenderer)(tileObjects[y, x].renderer)).sprite = sprites[y, x];
				// return updated GameObject
				return tileObjects[y, x];
			}
			
			// create a new sprite if one does not exist already
			if (tileObjects[y, x] == null) {
				tileObjects[y, x] = new GameObject("Tile: " + tilecode, typeof(SpriteRenderer));

				// parent new GameObject to TileMap
				tileObjects[y, x].transform.parent = this.transform;

				// create a new sprite
				sprites[y, x] = atlas.GetTile(tilecode);
				tileObjects[y, x].transform.Translate ((x / 100f) * atlas.tileWidth, (y / 100f) * atlas.tileHeight, 0f);

				// set new sprite to GameObject
				((SpriteRenderer)(tileObjects[y, x].renderer)).sprite = sprites[y, x];

				// return new GameObject
				return tileObjects[y, x];
			}
		}

		// the tile cannot be drawn outside of the tilemap array
		return null;
    }
}