using UnityEngine;

public class BillboardSprite : MonoBehaviour
{
	private Camera _cam;
	void Start()
	{
		_cam = Camera.main;
	}
	void LateUpdate()
	{
		transform.LookAt(_cam.transform);
		transform.Rotate(0, 180, 0);
	}
}