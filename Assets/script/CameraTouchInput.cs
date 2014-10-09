using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Camera))]
public class CameraTouchInput : MonoBehaviour {

    private Touch touch;

	// Update is called once per frame
	void Update () {
        if (Input.touchCount > 0) {
			touch = Input.GetTouch(0);
			
			camera.transform.Translate(-(touch.deltaPosition / 100f));        
			
			// zoom out with two touches
			if (Input.touchCount == 2) {
				// limit zoom maximum
				if (camera.orthographicSize < 5) {
					camera.orthographicSize += Time.deltaTime;
				}
			}
			
			// zoom in with three touches
			if (Input.touchCount == 3) {
				// limit zoom minimum
				if (camera.orthographicSize > .5) {
					camera.orthographicSize -= Time.deltaTime;
				}
			}
		}
	}
}
