using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof(Atlas))]
public class AtlasInspector : Editor {
	int selected;
	int lastSelected;
	int tileWidth;
	int tileHeight;

	private bool divisionApplied;
	private Texture2D[] texList;

	Color[] lastTile;
	Vector2 scrollPos = new Vector2 ();
	
	private static GUIStyle blankStyle;
	private static GUIStyle warningStyle;
	private static GUIStyle buttonStyle;

	public override void OnInspectorGUI() {
		// get tilemap as target object
		var targetAtlas = target as Atlas;

		// before any user input, clean the GUI dirty flag
		GUI.changed = false;

		GUILayout.Label ("Base Settings", EditorStyles.boldLabel);
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
				if (numTiles > 4096) {
					GUILayout.Label ("Divide into " + numTiles + " tiles?", warningStyle);
				}
				
				if (numTiles <= 4096) {
					GUILayout.Label ("Divide into " + numTiles + " tiles?");
				}

				// once the apply button is clicked, or if it has been before already
				if (GUILayout.Button ("Apply")) {
					// update the division applied flag
					divisionApplied = true;

					// destroy focused background texture if it exists
					if (buttonStyle.onNormal.background != null) {
						DestroyImmediate(buttonStyle.onNormal.background); 
					}

					// reset focused background size
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

			// render the texture list once and only after division is applied
			if (divisionApplied) {
				// update the division applied flag
				divisionApplied = false;
				texList = targetAtlas.GetTexList();
			}

			if (texList != null) {
				int rowLength = targetAtlas.RowLength;
				if (texList.Length > 0) {
					scrollPos = GUILayout.BeginScrollView (scrollPos, blankStyle);
					targetAtlas.selected = GUILayout.SelectionGrid (targetAtlas.selected, texList, rowLength, buttonStyle);
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
