using UnityEngine;
using UnityEditor;
using System.Collections;

public class TileMapPalette : EditorWindow {
	Texture2D myTex2D;
	int tileSizeX;
	int tileSizeY;
	int selected;
	int lastSelected;
	Color[] lastTile;
	Vector2 scrollPos = new Vector2 ();
	
	private Texture2D[] texList;
	private static GUIStyle blankStyle;
	private static GUIStyle warningStyle;
	private static GUIStyle buttonStyle;

	[MenuItem ("TileMap/Atlas Palette")]
	public static void ShowWindow() {
		EditorWindow.GetWindow (typeof(TileMapPalette));
	}

	void OnGUI() {
		GUILayout.Label ("Base Settings", EditorStyles.boldLabel);
		myTex2D = (Texture2D)EditorGUILayout.ObjectField ("Atlas Texture", myTex2D, typeof(Texture2D), false);

		GUILayout.BeginHorizontal ();
			GUILayout.Label ("Tile Size (x):");
			tileSizeX = EditorGUILayout.IntField (tileSizeX);
		GUILayout.EndHorizontal ();

		GUILayout.BeginHorizontal ();
			GUILayout.Label ("Tile Size (y):");
			tileSizeY = EditorGUILayout.IntField (tileSizeY);
		GUILayout.EndHorizontal ();	

		// only define blankStyle once
		if (blankStyle == null) {
			blankStyle = new GUIStyle();
		}

		// only define warningStyle once
		if (warningStyle == null) {
			warningStyle = new GUIStyle();
			warningStyle.fontStyle = FontStyle.Bold;
		}

		// only define buttonStyle once
		if (buttonStyle == null) {
			buttonStyle = new GUIStyle();
			buttonStyle.stretchHeight = false;
			buttonStyle.stretchWidth = false;
			buttonStyle.padding = new RectOffset(2, 2, 2, 2);
		}

		if (myTex2D != null) {
			// all tiles must be at least 2x2
			if (tileSizeX > 1 && tileSizeY > 1) {
				int numTilesX = myTex2D.width / tileSizeX;
				int numTilesY = myTex2D.height / tileSizeY;
				int numTiles = numTilesX * numTilesY;

				GUILayout.BeginHorizontal ();
				if (numTiles > 4096) {
					GUILayout.Label ("Divide into " + numTiles + " tiles?", warningStyle);
				}

				if (numTiles <= 4096) {
					GUILayout.Label ("Divide into " + numTiles + " tiles?");
				}

				if (GUILayout.Button ("Apply")) {
					// resize elements
					// reset focused background size
					buttonStyle.onNormal.background = new Texture2D(tileSizeX, tileSizeY);
					Color[] backColors = new Color[tileSizeX * tileSizeY];
					for (int i = 0; i < backColors.Length; i++) {
						backColors[i] = Color.black;
					}

					buttonStyle.onNormal.background.SetPixels(backColors);
					buttonStyle.onNormal.background.Apply();

					texList = new Texture2D[(myTex2D.width / tileSizeX) * (myTex2D.height / tileSizeY)];
					int k = 0;

					// flip array orientation by reading y-axis from top to bottom
					for (int j = myTex2D.height - tileSizeY; j >= 0; j -= tileSizeY) {
						// continue reading x-axis left to right
						for (int i = 0; i < myTex2D.width; i += tileSizeX) {
							texList [k] = new Texture2D (tileSizeX, tileSizeY);
							texList [k].filterMode = FilterMode.Point;
							texList [k].SetPixels (myTex2D.GetPixels (i, j, tileSizeX, tileSizeY));
							texList [k].Apply ();
							k++;
						}
					}
				}
				GUILayout.EndHorizontal ();
			}			

			// expand the selection grid as the screen is resized
			if (texList != null) {
				scrollPos = GUILayout.BeginScrollView (scrollPos, blankStyle);
				selected = GUILayout.SelectionGrid (selected, texList, (myTex2D.width / tileSizeX), buttonStyle);
				GUILayout.EndScrollView ();
			}
		}
	}
}
