using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof(Atlas))]
public class AtlasInspector : Editor {
	int selected;
	int lastSelected;
	int tileWidth;
	int tileHeight;

	private Texture2D[] texList;

	Color[] lastTile;
	Vector2 scrollPos = new Vector2 ();
	
	private static GUIStyle blankStyle;
	private static GUIStyle warningStyle;
	private static GUIStyle buttonStyle;

	public override void OnInspectorGUI() {
		// get tilemap as target object
		var targetAtlas = target as Atlas;

		targetAtlas.atlasTex = (Texture2D)EditorGUILayout.ObjectField ("Atlas Texture", targetAtlas.atlasTex, typeof(Texture2D), false);
		
		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Tile Size (x):");
			targetAtlas.tileWidth = EditorGUILayout.IntField (targetAtlas.tileWidth);
		GUILayout.EndHorizontal ();
		
		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Tile Size (y):");
			targetAtlas.tileHeight = EditorGUILayout.IntField (targetAtlas.tileHeight);
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
		
		if (targetAtlas != null) {
			// all tiles must be at least 2x2
			if (targetAtlas.tileWidth > 1 && targetAtlas.tileHeight > 1) {
				int numTilesX = targetAtlas.atlasTex.width / targetAtlas.tileWidth;
				int numTilesY = targetAtlas.atlasTex.height / targetAtlas.tileHeight;
				int numTiles = numTilesX * numTilesY;
				
				GUILayout.BeginHorizontal ();

				// show warning if dividing into many tiles
				if (numTiles > 4096) {
					GUILayout.Label ("Divide into " + (targetAtlas.LastTile + 1) + " tiles?", warningStyle);
				}

				// consider 4096 "a few" tiles
				if (numTiles <= 4096) {
					GUILayout.Label ("Divide into " + (targetAtlas.LastTile + 1) + " tiles?");
				}

				// apply sprite atlas texture division once the apply button is pressed
				// or if there is already a saved division
				if (GUILayout.Button ("Apply") ||
				    (targetAtlas.TexListGenerated && targetAtlas.TexList == null)) {
					// apply division to tile sprite atlas
					targetAtlas.DivideTiles();

					// destroy focused background texture if it exists
					if (buttonStyle.onNormal.background != null) {
						DestroyImmediate(buttonStyle.onNormal.background); 
					}

					// reset focused background size and texture
					buttonStyle.onNormal.background = 
						new Texture2D(targetAtlas.tileWidth, targetAtlas.tileHeight);
					Color[] backColors = new Color[targetAtlas.tileWidth * targetAtlas.tileHeight];
					for (int i = 0; i < backColors.Length; i++) {
						backColors[i] = Color.black;
					}					
					buttonStyle.onNormal.background.SetPixels(backColors);
					buttonStyle.onNormal.background.hideFlags = HideFlags.DontSave;
					buttonStyle.onNormal.background.Apply();
				}
				GUILayout.EndHorizontal ();
			}

			// if there is a valid atlas texture list
			if (targetAtlas.TexList != null) {
				// populate local texture list as needed
				if (texList == null) {
					texList = targetAtlas.TexList;
				}

				// use local copy to improve performance
				int rowLength = targetAtlas.Columns;
				if (rowLength > 0) {
					scrollPos = GUILayout.BeginScrollView (scrollPos, blankStyle);
					targetAtlas.selected = GUILayout.SelectionGrid (targetAtlas.selected, targetAtlas.TexList, rowLength, buttonStyle);
					GUILayout.EndScrollView ();
				}
			}
		}

		// set tilemap data as dirty if the GUI is changed
		// Unity will then save any changes and clear the dirty flag
		if (GUI.changed)
			EditorUtility.SetDirty (targetAtlas);
	}
}
