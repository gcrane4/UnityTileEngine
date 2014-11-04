using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof(TileMap))]
public class TileMapPaletteInspector : Editor {
	int selected;
	int lastSelected;
	int rowLength;

	int tileWidth;
	int tileHeight;

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
	
	public override void OnInspectorGUI() {
		// get tilemap as target object
		var targetTilemap = target as TileMap;

		// before any user input, clean the GUI dirty flag
		GUI.changed = false;

		GUILayout.Label ("Base Settings", EditorStyles.boldLabel);
		targetTilemap.atlas = (Texture2D)EditorGUILayout.ObjectField ("Atlas Texture", targetTilemap.atlas, typeof(Texture2D), false);
		
		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Tile Size (x):");
			targetTilemap.tileWidth = EditorGUILayout.IntField (targetTilemap.tileWidth);
		GUILayout.EndHorizontal ();
		
		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Tile Size (y):");
			targetTilemap.tileHeight = EditorGUILayout.IntField (targetTilemap.tileHeight);
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
		
		if (targetTilemap.atlas != null) {
			// all tiles must be at least 2x2
			if (targetTilemap.tileWidth > 1 && targetTilemap.tileHeight > 1) {
				int numTilesX = targetTilemap.atlas.width / targetTilemap.tileWidth;
				int numTilesY = targetTilemap.atlas.height / targetTilemap.tileHeight;
				int numTiles = numTilesX * numTilesY;
				
				GUILayout.BeginHorizontal ();
				if (numTiles > 4096) {
					GUILayout.Label ("Divide into " + numTiles + " tiles?", warningStyle);
				}
				
				if (numTiles <= 4096) {
					GUILayout.Label ("Divide into " + numTiles + " tiles?");
				}
				
				if (GUILayout.Button ("Apply")) {
					// cleanup unused textures
					if (texList != null) {
						foreach (Texture2D tex in texList) 
						{
							DestroyImmediate(tex);
						}
						
						texList = null;
					}
					
					// destroy focused background texture if it exists
					if (buttonStyle.onNormal.background != null) {
						DestroyImmediate(buttonStyle.onNormal.background); 
					}

					// reset focused background size
					buttonStyle.onNormal.background = 
						new Texture2D(targetTilemap.tileWidth, targetTilemap.tileHeight);
					Color[] backColors = new Color[targetTilemap.tileWidth * targetTilemap.tileHeight];
					for (int i = 0; i < backColors.Length; i++) {
						backColors[i] = Color.black;
					}
					
					buttonStyle.onNormal.background.SetPixels(backColors);
					buttonStyle.onNormal.background.hideFlags = HideFlags.DontSave;
					buttonStyle.onNormal.background.Apply();

					// create a list that contains all tiles in the atlas 
					// divided into target tile sizes
					texList = new Texture2D[(targetTilemap.atlas.width / targetTilemap.tileWidth) * (targetTilemap.atlas.height / targetTilemap.tileHeight)];
					int k = 0;
					
					// flip array orientation by reading y-axis from top to bottom
					for (int j = targetTilemap.atlas.height - targetTilemap.tileHeight; j >= 0; j -= targetTilemap.tileHeight) {
						// continue reading x-axis left to right
						for (int i = 0; i < targetTilemap.atlas.width; i += targetTilemap.tileWidth) {
							texList [k] = new Texture2D (targetTilemap.tileWidth, targetTilemap.tileHeight);
							texList [k].filterMode = FilterMode.Point;
							texList [k].SetPixels (targetTilemap.atlas.GetPixels (i, j, targetTilemap.tileWidth, targetTilemap.tileHeight));
							texList [k].hideFlags = HideFlags.DontSave;
							texList [k].Apply ();
							k++;
						}
					}
				}
				GUILayout.EndHorizontal ();
			}			

			if (texList != null && rowLength > 0) {
				scrollPos = GUILayout.BeginScrollView (scrollPos, blankStyle);
				rowLength = (targetTilemap.atlas.width / targetTilemap.tileWidth);
				selected = GUILayout.SelectionGrid (selected, texList, rowLength, buttonStyle);
				GUILayout.EndScrollView ();
			}
		}

		// set tilemap data as dirty if the GUI is changed
		// Unity will then save any changes and clear the dirty flag
		if (GUI.changed)
			EditorUtility.SetDirty (targetTilemap);
	}

	void OnSceneGUI () {
		// get tilemap as target object
		var targetTilemap = target as TileMap;

		// only process edit mode input if tilemap is selected
		if (targetTilemap != null) {
			int controlID = GUIUtility.GetControlID (FocusType.Passive);
			if (Event.current.GetTypeForControl(controlID) == EventType.MouseDown) {
				GUIUtility.hotControl = controlID;

				// manipulate current event's mouse position in world
				Vector2 mousePosition = Event.current.mousePosition;

				Debug.Log("Mouse: " + mousePosition);

				Vector2 worldPoint = SceneView.currentDrawingSceneView.camera.ScreenToWorldPoint(mousePosition);

				Debug.Log("World: " + worldPoint);

				Vector2 cameraPosition = SceneView.currentDrawingSceneView.camera.transform.position;

				Vector2 shiftPoint = new Vector3(worldPoint.x,
                	-(worldPoint.y - (2 * cameraPosition.y)));

				Debug.Log("Shift: " + shiftPoint);

				int tileX = (int)((shiftPoint.x * 100) / targetTilemap.tileWidth);
				int tileY = (int)((shiftPoint.y * 100) / targetTilemap.tileHeight);

				if (tileX > -1 && tileX < targetTilemap.AtlasWidth() &&
				    tileY > -1 && tileY < targetTilemap.AtlasHeight()) {
					Debug.Log("Drawing Tile: (" + tileX + ", " + tileY + "): " + selected);
					GameObject newTile = targetTilemap.DrawTile(tileX, tileY, selected);
					newTile.transform.parent = targetTilemap.transform;
				}
			}
		}
	}
}
