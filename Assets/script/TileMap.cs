using UnityEngine;
using System.Collections;

//TODO: REFACTOR!!! very much needed, approaching end of basic iteration
public class TileMap : MonoBehaviour {
	private bool writeOn = false;

	private float guiScaleX;
	private float guiScaleY;

	private int tileSelect = 0;

    /** the tile atlas, a texture containing one or more tile images evenly spaced **/
    public Texture2D atlas;

	private int[,] map;
	private GameObject[,] tileSprites;

	// TODO: dynamic GUI bounds for layered input
	private static Rect[] guiBounds = new Rect[] { new Rect(0f, .9004525f, .7175141f, .0995475f),
												   new Rect(0f, .4751131f, .240113f, .5248869f)};	

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
		if (tilecode == -1) 
		{
			// disable this tile
			tileSprites[y, x].renderer.enabled = false;
			return tileSprites[y, x];
		} else if (tileSprites[y, x] != null) {
			tileSprites[y, x].renderer.enabled = true;
		}

		if (tilecode >= (AtlasWidth() * AtlasHeight())) {
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
		map = new int[AtlasHeight(), AtlasWidth()];
		// TODO: on a mostly empty map, don't use so much memory space
		// although these ARE pointers per C#
		tileSprites = new GameObject[AtlasHeight(), AtlasWidth()];

		// print all atlas tiles in order
		int tc = 0;
		for (int y = 0; y < AtlasHeight(); y++)
		{
			for (int x = 0; x < AtlasWidth(); x++) 
			{
				map[y, x] = tc++;
			}
		}

        // turn off texture filtering
        atlas.filterMode = FilterMode.Point;

		// render all tiles in map
		DrawRegion (0, 0, AtlasWidth(), AtlasHeight());
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

	void Update() {
		if (Screen.orientation == ScreenOrientation.Landscape) {
			guiScaleX = (Screen.width / 12.0f) / 100;
			guiScaleY = (Screen.height / 6.0f) / 100;
		} 

		if (Screen.orientation == ScreenOrientation.Portrait) {
			guiScaleX = (Screen.width / 6.0f) / 100;
			guiScaleY = (Screen.height / 12.0f) / 100;
		}

		// handle Input mouse clicks
		if (Input.GetMouseButtonDown (0)) {
			bool guiHit = false;

			// check for a collision with the GUI layer, if clicked outside of GUI rect
			Vector3 mouseViewPosition = Camera.main.ScreenToViewportPoint (Input.mousePosition);

			foreach (Rect bound in guiBounds) {
				// check to see if GUI was clicked first
				if (bound.Contains(mouseViewPosition)) {
					Debug.Log("got GUI click at: (" + mouseViewPosition.x +
					          ", " + mouseViewPosition.y + ")");
					guiHit = true;
					break;
				}
			}

			// after all bounds are checked, check for input collision on TileMap
			if (!guiHit) {
				Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

				// cast a 2D ray to check for collision with TileMap objects
				RaycastHit2D rayHit = Physics2D.Raycast(
					mouseWorldPosition, Vector2.zero);
				
				// show click when an object is clicked
				if (rayHit.collider != null) {
					Debug.Log ("object clicked: " + rayHit.collider.name);
					Debug.Log ("mouse at: (" + mouseWorldPosition.x + 
					           ", " + mouseWorldPosition.y + ")");

					int tileX = ((int)(mouseWorldPosition.x * 100)) / 32;
					int tileY = ((int)(mouseWorldPosition.y * 100)) / 32;

					Debug.Log ("tile coord: (" + tileX + ", " + tileY + ")");

					// update tile to selected tile on map at mouse position
					DrawTile(tileX, tileY, tileSelect);
				}
			}
		}

	}


	// TODO: scall small text up on smaller screens
	void OnGUI() {
		// refresh tile texture each frame
		// WARN: this may be expensive for large tiles
		Texture2D tileTex = new Texture2D(tileWidth, tileHeight);
		Rect tileRect = getTileRect(tileSelect);
		tileTex.SetPixels(
			atlas.GetPixels((int)tileRect.x, (int)tileRect.y,
		                (int)tileRect.width, (int)tileRect.height));
		tileTex.Apply ();
		GUI.DrawTexture(new Rect(10f * guiScaleX, 200f * guiScaleY, 128f * guiScaleX, 128f * guiScaleX), tileTex);

		// toggle write mode with toggle switch
		writeOn = GUI.Toggle (new Rect (10f * guiScaleX, 130f * guiScaleY, 200f * guiScaleX, 60f * guiScaleY), writeOn, "Write Mode");

		// increase and loop around atlas tiles
		if (GUI.Button (new Rect (230f * guiScaleX, 20f * guiScaleY, 200f * guiScaleX, 100f * guiScaleY), "Next Tile")) {
			// at the last tile, go back to tile 0
			if (tileSelect == (AtlasWidth() * AtlasHeight()) - 1) {
				tileSelect = 0;
			} else {
				tileSelect++;
			}
		}

		// decrease and loop around atlas tiles
		if (GUI.Button (new Rect (10f * guiScaleX, 20f * guiScaleY, 200f * guiScaleX, 100f * guiScaleY), "Prev Tile")) {
			// at the first tile, go to the last tile in atlas
			if (tileSelect == 0) {
				tileSelect = (AtlasWidth() * AtlasHeight()) - 1;
			} else {
				tileSelect--;
			}
		} 
	}
}