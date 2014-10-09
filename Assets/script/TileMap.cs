using UnityEngine;
using System.Collections;

public class TileMap : MonoBehaviour {

    /** the tile atlas, a texture containing one or more tile images evenly spaced **/
    public Texture2D atlas;

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

    public GameObject DrawTile(int x, int y, int i, int j, int tilecode)
    {
		if (tilecode >= (AtlasWidth * AtlasHeight)) {
			throw new System.IndexOutOfRangeException("Tile code index is out of range of tile atlas.");
		}
	
    	//extract tile from atlas into sprite
		GameObject go = new GameObject("Tile (" + ((2 * x) + i) + ", " + ((2 * y) + j) + "): " + tilecode, typeof(SpriteRenderer));
		((SpriteRenderer)(go.renderer)).sprite = 
			Sprite.Create(atlas, getTileRect(tilecode), Vector2.zero);

		go.transform.Translate ((i / 100f) * 32, (j / 100f) * 32, 0f);

		return go;
    }

	// Use this for initialization
	void Start () {
		int[,] map = new int[AtlasHeight, AtlasWidth];

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

		// quad count
		int c = 0;

	    for (int y = 0; y < AtlasWidth; y += 2) {
	        for (int x = 0; x < AtlasHeight; x += 2) {
				GameObject quad = new GameObject("Quad " + c);
				quad.transform.Translate((x / 100f) * 32, (y / 100f) * 32, 0f);
				quad.transform.parent = this.transform;

				for (int j = y; j < y + 2; j++) {
					for (int i = x; i < x + 2; i++) {
						if (i > -1 && i < map.GetLength(0)
						    && j > -1 && j < map.GetLength(1)) {							
							int tilecode = map[j, i];
							//Debug.Log("Source rect of " + tilecode + " = " + getTileRect(tilecode));
							GameObject newTile = DrawTile(x, y, i, j, tilecode);
							newTile.transform.parent = quad.transform;
						}
					}
				}

				// increment quad count
				c++;
	        }
	    }
	}
}
