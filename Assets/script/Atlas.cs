using UnityEngine;
using System.Collections;

public class Atlas : MonoBehaviour {
	// VARIABLES VISIBLE IN EDITOR
	/** the texture used for the tile sprite atlas */
	public Texture2D atlasTex;
	/** the width of each tile */
	public int tileWidth;
	/** the height of each tile */
	public int tileHeight;

	// INTERNAL VARIABLES
	private Texture2D[] texList;
	[SerializeField]
	private bool texListGenerated;
	public int selected;

	// INTERNAL CLASS METHODS
	private bool CheckRequirements() {
		if (atlasTex == null) {
			Debug.LogError (name + "GetTile(x,y): please select an atlas texture");
			return false;
		}
		
		if (tileWidth < 2) {
			Debug.LogError (name + "GetTile(x,y): please specify a tile width greater than 1.");
			return false;
		}
		
		if (tileHeight < 2) {
			Debug.LogError (name + "GetTile(x,y): please specify a tile height greater than 1.");
			return false;
		}

		return true;
	}

	// CLASS DATA ACCESSORS
	/** returns the number of columns of tiles in the atlas */
	public int Columns {
		get {
			if (tileWidth > 0)
				return atlasTex.width / tileWidth;
			return 0;
		}
	}
	
	/** returns the number of rows of tiles in the atlas */
	public int Rows {
		get {
			if (tileHeight > 0)
				return atlasTex.height / tileHeight;
			return 0;
		}
	}

	/** returns the last tile index */
	public int LastTile {
		get {
			return (Columns * Rows) - 1;
		}
	}

	/** returns the state of division of the atlas */
	public bool TexListGenerated {
		get {
			return texListGenerated;
		}
	}


	public Rect getTileRect(int x, int y) 
	{
		return new Rect (x * tileWidth, atlasTex.height - ((y + 1) * tileHeight), 
			tileWidth, tileHeight);
	}
	
	// CLASS METHODS
	public Sprite GetTile(int x, int y) {
		/** check for sane values before pulling tile */
		if (CheckRequirements()) {
			/** check requested x and y values against atlas range */
			if (x < 0 || x > Columns ||
			    y < 0 || y > Rows)
				throw new UnityException("Tile requested is outside of bounds of sprite atlas.");

			// create sprite from sprite atlas texture and tile coordiantes
			return Sprite.Create(atlasTex, getTileRect (x, y), Vector2.zero);
		} 

		else throw new UnityException("Couldn't get tile from " + name);
	}

	public Sprite GetTile(int tilecode) {
		int x = tilecode % Columns;
		int y = tilecode / Columns;

		return GetTile (x, y);
	}

	public void DivideTiles ()
	{
		// update the texture list generated flag
		texListGenerated = true;

		// if a dimension is missing, do not divide
		if (Columns < 1 || Rows < 1) {
			texList = null;
			return;
		}

		// if new list textures are required, recalculate all tiles
		texList = new Texture2D[Columns * Rows];
		int k = 0;
		for (int j = atlasTex.height - tileHeight; j > -1; j -= tileHeight) {
			for (int i = 0; i < atlasTex.width; i += tileWidth) {
				texList [k] = new Texture2D (tileWidth, tileHeight);
				texList [k].SetPixels (atlasTex.GetPixels (i, j, tileWidth, tileHeight));
				texList [k].filterMode = FilterMode.Point;
				texList [k].hideFlags = HideFlags.DontSave;
				texList [k].Apply ();
				k++;
			}
		}
	}

	public Texture2D[] TexList {
		get {
			return texList;
		}
	}
}