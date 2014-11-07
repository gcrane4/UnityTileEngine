using UnityEngine;
using System.Collections;

public class TileQuad : MonoBehaviour {
	/** the sprite atlas used to pull textures **/
	public Texture2D atlas;
	public int tileSizeX;
	public int tileSizeY;

	/** the quad texture only manipulatable through class methods **/
	Texture2D quadTex;

	public void AddTile(int x, int y, int tilecode) {

	}
}
