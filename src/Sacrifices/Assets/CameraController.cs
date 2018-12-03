using Assets.Scripts.Models;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public float RoomWidth = 19f;
    public Position.Location Focus
    {
        set
        {
            focus = value;
            targetPosition = initialPosition + new Vector3((int)focus * RoomWidth, 0, 0);
        }
    }
    private Position.Location focus;
    private Vector3 targetPosition;
    private Vector3 initialPosition;
    private Vector3 currentVelocity = Vector3.zero;

    void Awake()
    {
        targetPosition = transform.position;
        initialPosition = transform.position;
    }

	public void Next()
    {
        if (focus != Position.Location.D)
        {
            Focus = (Position.Location)((int)focus + 1);
        }
    }

    public void Back()
    {
        if (focus != Position.Location.A)
        {
            Focus = (Position.Location)((int)focus - 1);
        }
    }
	
	void Update ()
    {
        if (transform.position != targetPosition)
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, 1f);
        }
    }
}
