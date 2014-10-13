using UnityEngine;
using System.Collections;

public class TileMap : MonoBehaviour {

	private bool oneTouch = false;

    /** the tile atlas, a texture containing one or more tile images evenly spaced **/
    public Texture2D atlas;

	private int[,] map;
	private GameObject[,] tileSprites;

    // TODO: account for tile atlas padding

    /** the width of all tiles in the atlas in pixels **/
    public int tileWidth;

    /** the height of all tiles in the atlas in pixels **/
    public int tileHeight;

	/** return the width of the tile atlas in tiles **/
	public int AtlasWidth {
		get 
		{
			return (atlas.width / tileWidth);
		}
	}
	
	/** return the height of the tile atlas in tiles **/
	public int AtlasHeight {
		get
		{
			return (atlas.height / tileHeight);
		}
	}

	public Rect getTileRect(int tilecode) 
	{
		return new Rect ((tilecode % AtlasWidth) * tileWidth, ((AtlasHeight - (tilecode / AtlasWidth)) * tileHeight) - tileHeight, tileWidth, tileHeight);
	}

    public GameObject DrawTile(int x, int y, int tilecode)
    {
		if (tilecode == -1) 
		{
			// disable this tile
			tileSprites[y, x].renderer.enabled = false;
			return tileSprites[y, x];
		} else if (tileSprites[y, x] != null) {
			tileSprites[y, x].renderer.enabled = true;
		}

		if (tilecode >= (AtlasWidth * AtlasHeight)) {
			throw new System.IndexOutOfRangeException("Tile code index is out of range of tile atlas.");
		}
	
		// SINGLETON
		// if a sprite exists for this tile already, replace it with a new tile sprite
		if (tileSprites[y, x] != null) {
			((SpriteRenderer)tileSprites[y, x].renderer).sprite = Sprite.Create(atlas, getTileRect(tilecode), Vector2.zero);
			return tileSprites[y, x];
		}

    	// create a new sprite if one does not exist already
		if (tileSprites[y, x] == null) {
			GameObject go = new GameObject("Tile: " + tilecode, typeof(SpriteRenderer));
			((SpriteRenderer)(go.renderer)).sprite = 
				Sprite.Create(atlas, getTileRect(tilecode), Vector2.zero);
			go.transform.Translate ((x / 100f) * 32, (y / 100f) * 32, 0f);

			tileSprites[y, x] = go;
			return tileSprites[y, x];
		}

		// TODO: shouldn't get here yet
		return null;
    }

	// Use this for initialization
	void Start () {
		map = new int[AtlasHeight, AtlasWidth];
		// TODO: on a mostly empty map, don't use so much memory space
		// although these ARE pointers per C#
		tileSprites = new GameObject[AtlasHeight, AtlasWidth];

		// print all atlas tiles in order
		int tc = 0;
		for (int y = 0; y < AtlasHeight; y++)
		{
			for (int x = 0; x < AtlasWidth; x++) 
			{
				map[y, x] = tc++;
			}
		}

        // turn off texture filtering
        atlas.filterMode = FilterMode.Point;

		// render all tiles in map
		DrawRegion (0, 0, AtlasWidth, AtlasHeight);
	}

	void DrawRegion(int x, int y, int width, int height) 
	{
		for (int j = y; j < y + height; j++) 
		{
			for (int i = x; i < x + width; i++) 
			{
				tileSprites[j, i] = DrawTile(i, j, map[j, i]);
			}
		}
	}

	void AddTile (int x, int y, int tilecode) 
	{
		if (tilecode < (AtlasWidth * AtlasHeight)) {
			map [y, x] = tilecode;
		} else {
			map [y, x] = -1;
		}

		DrawTile (x, y, map [y, x]);
	}

	void RemoveTile (int x, int y)
	{
		// update map data
		map [y, x] = -1;

		// update tile sprite
		DrawTile (x, y, -1);
	}

	void Update() {
		if (Input.touchCount == 0) {
			oneTouch = false;
		}

		if (Input.GetMouseButtonDown (0) || Input.touchCount == 1) {

			Ray ray = new Ray();

			if (Input.GetMouseButtonDown(0)) {
				ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			} else if (Input.touchCount == 1 && !oneTouch) { 
				ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
				oneTouch = true;
			} else return;
			
			Debug.Log ("Got a click at: " + ray);
			
			if (Physics2D.Raycast(ray.origin, ray.direction)) {
				int tileX = (int)(ray.origin.x * 100f) / 32;
				int tileY = (int)(ray.origin.y * 100f) / 32;
				int tilecode = (tileY * 32) + tileX;
				
				Debug.Log ("The mouse hit a tile: (" +
				           tileX + ", " + tileY + "): " + tilecode);

				// check tilemap boundaries
				if (tileX < map.GetLength(1) && tileY < map.GetLength(0)) {
					// loop through all tilecodes
					if (map[tileY, tileX] == (AtlasWidth * AtlasHeight) - 1) {
						// loop to first tile code at last tilecode
						map[tileY, tileX] = 0;
					} else {
						// otherwise iterate ahead by one
						map[tileY, tileX] = map[tileY, tileX] + 1;
					}

					// create / render new sprite image
					DrawTile(tileX, tileY, map[tileY, tileX]);
				}
			}
		}
	}
}
