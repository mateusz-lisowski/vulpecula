using UnityEngine;


public class CameraController : MonoBehaviour
{
	public LayerMask hiddenLayers;

    public Transform target;
    public PolygonCollider2D boundary;

	public float xMargin = 1f; // Distance in the x axis the player can move before the camera follows.
	public float yMargin = 1f; // Distance in the y axis the player can move before the camera follows.
	public float xSmooth = 8f; // How smoothly the camera catches up with it's target movement in the x axis.
	public float ySmooth = 8f; // How smoothly the camera catches up with it's target movement in the y axis.

	private void OnValidate()
	{
		Camera camera = transform.GetComponent<Camera>();

		camera.cullingMask = ~hiddenLayers;
	}

	private void Update()
	{
		trackPlayer();
		keepWithinBoundary();
	}

	private bool checkXMargin()
	{
		// Returns true if the distance between the camera and the player in the x axis is greater than the x margin.
		return Mathf.Abs(transform.position.x - target.position.x) > xMargin;
	}
	private bool checkYMargin()
	{
		// Returns true if the distance between the camera and the player in the y axis is greater than the y margin.
		return Mathf.Abs(transform.position.y - target.position.y) > yMargin;
	}
	private void trackPlayer()
	{
		float targetX = transform.position.x;
		float targetY = transform.position.y;

		if (checkXMargin())
		{
			targetX = Mathf.Lerp(transform.position.x, target.position.x, xSmooth * Time.deltaTime);
		}

		if (checkYMargin())
		{
			targetY = Mathf.Lerp(transform.position.y, target.position.y, ySmooth * Time.deltaTime);
		}

		transform.position = new Vector3(targetX, targetY, transform.position.z);
	}

	private static Bounds orthographicBounds(Camera camera)
	{
		float screenAspect = (float)Screen.width / (float)Screen.height;
		float cameraHeight = camera.orthographicSize * 2;
		Bounds bounds = new Bounds(
			camera.transform.position,
			new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
		return bounds;
	}
	private bool keepPointWithinBoundary(Vector2 point)
	{
		Vector2 closest = boundary.ClosestPoint(point);

		if (closest == point)
			return true;

		if (Mathf.Abs(closest.x - point.x) < Mathf.Abs(closest.y - point.y))
			closest.x = point.x;
		else
			closest.y = point.y;

		transform.position = new Vector3(
			transform.position.x - (point.x - closest.x), 
			transform.position.y - (point.y - closest.y),
			transform.position.z);

		return false;
	}
	private void keepWithinBoundary()
	{
		for (int i = 0; i < 20; ++i)
		{
			Bounds bounds = orthographicBounds(Camera.main);

			if (!keepPointWithinBoundary(new Vector2(bounds.min.x, bounds.min.y)))
				continue;
			if (!keepPointWithinBoundary(new Vector2(bounds.max.x, bounds.min.y)))
				continue;
			if (!keepPointWithinBoundary(new Vector2(bounds.min.x, bounds.max.y)))
				continue;
			if (!keepPointWithinBoundary(new Vector2(bounds.max.x, bounds.max.y)))
				continue;

			return;
		}
	}
}
