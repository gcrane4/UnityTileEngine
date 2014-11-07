using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof(TileMap))]
public class TileMapPaletteInspector : Editor {
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

				// translate screen click to world point
				Vector2 worldPoint = SceneView.currentDrawingSceneView.camera.ScreenToWorldPoint(mousePosition);

				// get the camera position for offset
				Vector2 cameraPosition = SceneView.currentDrawingSceneView.camera.transform.position;

				// shift the mouse click point using camera position
				Vector2 shiftPoint = new Vector3(worldPoint.x,
                	-(worldPoint.y - (2 * cameraPosition.y)));

				// get x and y in tiles
				Atlas atlas = targetTilemap.GetComponent<Atlas>();
				int tileX = (int)((shiftPoint.x * 100) / atlas.tileWidth);
				int tileY = (int)((shiftPoint.y * 100) / atlas.tileHeight);

				// use TileMap method to draw tile to TileMap
				if (targetTilemap.DrawTile(tileX, tileY, atlas.selected) == null) {
					// switch control back to the Editor if clicking outside of TileMap
					GUIUtility.hotControl = 0;
				}
			}
		}
	}
}
