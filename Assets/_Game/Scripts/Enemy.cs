using Toolkit;
using Toolkit.Health;
using UnityEngine;

public class Enemy : MonoBehaviour {
    public Vector3 startPoint;
    public Vector3 endPoint;

    public float speed = 4;
    public Vector3 currentTarget;
    public bool walkingTowardsEnd;

    public float SpeedMultiplier = 1f;
    public bool RotateTowardsMovement = true;

    private void Awake() {
        startPoint = transform.TransformPoint(startPoint);
        endPoint = transform.TransformPoint(endPoint);
        transform.position = startPoint;
        currentTarget = endPoint;
        walkingTowardsEnd = true;
    }

    private void Update() {
        var newPos = Vector3.MoveTowards(transform.position, currentTarget, Time.deltaTime * speed * SpeedMultiplier);
        if(Vector3.Distance(newPos, currentTarget) < 0.01f) {
            currentTarget = walkingTowardsEnd ? startPoint : endPoint;
            walkingTowardsEnd = !walkingTowardsEnd;
        }
        transform.position = newPos;


        if(RotateTowardsMovement) {
            var targetRotation = Quaternion.LookRotation(currentTarget - newPos, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 15f);
        }
    }

    public void TakeControl(float speedMultiplier) {
        this.SpeedMultiplier = speedMultiplier;
        RotateTowardsMovement = false;
    }

    public void RevertControl() {
        SpeedMultiplier = 1f;
        RotateTowardsMovement = true;
    }

    private void OnDrawGizmos() {
        using(new GizmosUtility.MatrixScope(transform)) {
            Gizmos.DrawSphere(startPoint, 0.25f);
            Gizmos.DrawSphere(endPoint, 0.25f);
        }
    }
}
