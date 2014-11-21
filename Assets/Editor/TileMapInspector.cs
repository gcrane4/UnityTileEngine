using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof(TileMap))]
public class TileMapInspector : Editor {
	void OnSceneGUI () {
		// get tilemap as target object
		var targetTilemap = target as TileMap;

		// only process edit mode input if tilemap is selected
		if (targetTilemap != null) {
			int controlID = GUIUtility.GetControlID (FocusType.Passive);
			if (Event.current.GetTypeForControl(controlID) == EventType.MouseDown) {
				GUIUtility.hotControl = controlID;

				// manipulate current event's mouse position on screen
				Vector2 mousePosition = Event.current.mousePosition;

				// flip y-axis mouse position information
				mousePosition.y = SceneView.currentDrawingSceneView.camera.pixelHeight - mousePosition.y;

				// translate screen click to world point
				Vector2 worldPoint = SceneView.currentDrawingSceneView.camera.ScreenToWorldPoint(mousePosition);

				// shift world point using tilemap transform position
				worldPoint.x -= targetTilemap.transform.position.x;
				worldPoint.y -= targetTilemap.transform.position.y;

				// get x and y in tiles
				Atlas atlas = targetTilemap.GetComponent<Atlas>();
				int tileX = Mathf.FloorToInt((worldPoint.x * 100) / atlas.tileWidth);
				int tileY = Mathf.FloorToInt((worldPoint.y * 100) / atlas.tileHeight);

				Debug.Log("Tile: " + tileX + ", " + tileY);

				// use TileMap method to draw tile to TileMap
				if (targetTilemap.DrawTile(tileX, tileY, atlas.selected) == null) {
					// switch control back to the Editor if clicking outside of TileMap
					GUIUtility.hotControl = 0;
				}
			}
		}
	}
}
