using UnityEngine;
using System;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[RequireComponent (typeof(Atlas))]
public class TileMap : MonoBehaviour
{
	private GameObject[,] tileObjects;
	private Sprite[,] sprites;
	private Atlas atlas;

	// TODO: account for tile atlas padding

	/** the width of the map in tiles */
	public int mapWidth;

	/** the height of the map in tiles */
	public int mapHeight;

	public Rect getTileRect (int tilecode)
	{
		return new Rect ((tilecode % atlas.Columns) * atlas.tileWidth, 
			((atlas.Rows - (tilecode / atlas.Columns)) * atlas.tileHeight) - atlas.tileHeight, 
			atlas.tileWidth, atlas.tileHeight);
	}

	void CreateSprite (int x, int y, int tilecode)
	{
		// assign new sprite data
		sprites [y, x] = atlas.GetTile (tilecode);
		tileObjects [y, x] = new GameObject ("Tile: " + tilecode, typeof(SpriteRenderer));
		tileObjects [y, x].transform.parent = this.transform;

		Vector2 tileVector = new Vector2(x, y);

		tileVector.x *= atlas.tileWidth;
		tileVector.x /= 100f;

		tileVector.y *= atlas.tileHeight;
		tileVector.y /= 100f;

		// use tilemap transform position to translate new sprite
		tileVector.x += transform.position.x;
		tileVector.y += transform.position.y;

		tileObjects [y, x].transform.position = tileVector;

		// update GameObject sprite with new sprite
		((SpriteRenderer)(tileObjects [y, x].renderer)).sprite = sprites [y, x];
	}

	public GameObject DrawTile (int x, int y, int tilecode)
	{
		// alias atlas from Atlas component
		if (atlas == null) {
			atlas = this.GetComponent<Atlas> ();
		}

		// create a map of GameObjects
		if (tileObjects == null) {
			tileObjects = new GameObject[mapHeight, mapWidth];
		}

		// create a map of Sprites
		if (sprites == null) {
			sprites = new Sprite[mapHeight, mapWidth];
		}

		if (mapWidth > tileObjects.GetLength (1) || mapHeight > tileObjects.GetLength (0)) {
			// create a new map that is larger than the original
			int[,] newMap = new int[mapHeight, mapWidth];
			GameObject[,] newTO = new GameObject[mapHeight, mapWidth];
			Sprite[,] newSprites = new Sprite[mapHeight, mapWidth];

			// copy old map data to new map
			for (int j = 0; j < tileObjects.GetLength(0); j++) {
				for (int i = 0; i < tileObjects.GetLength(1); i++) {
					newTO [j, i] = tileObjects [j, i];
					newSprites [j, i] = sprites [j, i];
				}
			}

			// replace old map with new map by reference
			tileObjects = newTO;
			sprites = newSprites;
		}

		// check target against map dimensions
		if (x > -1 && x < mapWidth &&
			y > -1 && y < mapHeight) {
			if (tilecode == -1) {
				// disable this tile
				tileObjects [y, x].renderer.enabled = false;
				return tileObjects [y, x];
			} else if (tileObjects [y, x] != null) {
				tileObjects [y, x].renderer.enabled = true;
			}

			if (tilecode > atlas.LastTile) {
				throw new System.IndexOutOfRangeException ("Tile code index is out of range of tile atlas.");
			}

			// SINGLETON
			// if a sprite exists for this tile already, replace it with a new tile sprite
			if (tileObjects [y, x] != null) {
				// destroy existing object data
				DestroyImmediate (sprites [y, x]);
				DestroyImmediate (tileObjects [y, x]);

				// create a new sprite for this object
				CreateSprite (x, y, tilecode);
			}
			
			// create a new sprite if one does not exist already
			else {
				CreateSprite (x, y, tilecode);
			}
		} else throw new UnityException ("The new point [" + x + ", " + y + "] is out of the TileMap's range.");

		// return the current map data
		return tileObjects[y, x];
	}
}