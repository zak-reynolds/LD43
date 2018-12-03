using UnityEngine;

public class PlebMover : MonoBehaviour {

    public Vector3 TargetPosition {
        set {
            targetPosition = value;
            gameObject.GetComponentInChildren<SpriteRenderer>().flipX = targetPosition.x < transform.position.x;
        }
    }
    private Vector3 targetPosition;
    private Vector3 currentVelocity = Vector3.zero;

    void Awake()
    {
        targetPosition = transform.position;
    }

	void Update () {
		if (transform.position != targetPosition)
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, 1f);
        }
	}
}
