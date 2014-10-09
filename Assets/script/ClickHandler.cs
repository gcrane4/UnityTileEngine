using UnityEngine;
using System.Collections;

public class ClickHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			Debug.Log ("Got mouse click at: " + ray);

			if (Physics2D.Raycast(ray.origin, ray.direction)) {
				int tileX = (int)(ray.origin.x * 100f) / 32;
				int tileY = (int)(ray.origin.y * 100f) / 32;
				int tilecode = (tileY * 32) + tileX;

				Debug.Log ("The mouse hit a tile: (" +
				           tileX + ", " + tileY + "): " + tilecode);
			}
		}
	}
}
