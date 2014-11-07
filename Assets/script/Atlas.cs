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
	public int RowLength {
		get {
			return atlasTex.width / tileWidth;
		}
	}

	public int ColLength {
		get {
			return atlasTex.height / tileHeight;
		}
	}

	public int LastTile {
		get {
			return (RowLength * ColLength) - 1;
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
			if (x < 0 || x > RowLength ||
			    y < 0 || y > ColLength)
				throw new UnityException("Tile requested is outside of bounds of sprite atlas.");

			// create sprite from sprite atlas texture and tile coordiantes
			return Sprite.Create(atlasTex, getTileRect (x, y), Vector2.zero);
		} 

		else throw new UnityException("Couldn't get tile from " + name);
	}

	public Sprite GetTile(int tilecode) {
		int x = tilecode % RowLength;
		int y = tilecode / RowLength;

		return GetTile (x, y);
	}

	void DivideTiles ()
	{
		// if new list textures are required, recalculate all tiles
		texList = new Texture2D[RowLength * ColLength];
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

	public Texture2D[] GetTexList() {
		if (CheckRequirements ()) {
			// if there is an existing texture list with at least one tile
			if (texList != null) {
				// check the new parameters against old texture list data
				if (texList.Length != (atlasTex.width / tileWidth) * (atlasTex.height / tileHeight) ||
				    texList[0].width != tileWidth ||
				    texList[0].height != tileHeight) {
						// generate new tile list as parameters change
						DivideTiles ();
					}
				}
			}

			// attempt to generate a new list if it hasn't been already
			if (texList == null) {
				DivideTiles ();
			}

		return texList;
	}
}