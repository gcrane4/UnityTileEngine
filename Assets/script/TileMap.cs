using UnityEngine;
using System.Collections;

public class TileMap : MonoBehaviour {
    /** the tile atlas, a texture containing one or more tile images evenly spaced **/
    public Texture2D atlas;

	private int[,] map;
	private GameObject[,] tileObjects;
	private Sprite[,] sprites;

    // TODO: account for tile atlas padding

    /** the width of all tiles in the atlas in pixels **/
    public int tileWidth;

    /** the height of all tiles in the atlas in pixels **/
    public int tileHeight;

	/** return the width of the tile atlas in tiles **/
	public int AtlasWidth() {
		return (atlas.width / tileWidth);
	}
	
	/** return the height of the tile atlas in tiles **/
	public int AtlasHeight() {
		return (atlas.height / tileHeight);
	}

	public Rect getTileRect(int tilecode) 
	{
		return new Rect ((tilecode % AtlasWidth()) * tileWidth, ((AtlasHeight() - (tilecode / AtlasWidth())) * tileHeight) - tileHeight, tileWidth, tileHeight);
	}

    public GameObject DrawTile(int x, int y, int tilecode)
    {
		// create an empty map if it does not already exist
		if (map == null) {
			map = new int[32, 32];
		}

		if (tileObjects == null) {
			tileObjects = new GameObject[32, 32];
		}

		if (sprites == null) {
			sprites = new Sprite[32, 32];
		}

		// check target against map dimensions
		if (x > -1 && x < map.GetLength (1) &&
			y > -1 && y < map.GetLength (0)) {
			if (tilecode == -1) 
			{
				// disable this tile
				tileObjects[y, x].renderer.enabled = false;
				return tileObjects[y, x];
			} else if (tileObjects[y, x] != null) {
				tileObjects[y, x].renderer.enabled = true;
			}
			
			if (tilecode >= (AtlasWidth() * AtlasHeight())) {
				throw new System.IndexOutOfRangeException("Tile code index is out of range of tile atlas.");
			}
			
			// SINGLETON
			// if a sprite exists for this tile already, replace it with a new tile sprite
			if (sprites[y, x] != null) {
				// destroy existing sprite data
				DestroyImmediate(sprites[y, x]);
				// assign new sprite data
				sprites[y, x] = Sprite.Create(atlas, getTileRect(tilecode), Vector2.zero);
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
				sprites[y, x] = Sprite.Create(atlas, getTileRect(tilecode), Vector2.zero);
				tileObjects[y, x].transform.Translate ((x / 100f) * tileWidth, (y / 100f) * tileHeight, 0f);

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