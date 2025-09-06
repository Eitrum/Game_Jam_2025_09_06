using Toolkit;
using UnityEngine;

public class Enemy : MonoBehaviour {
    public Vector3 startPoint;
    public Vector3 endPoint;

    public float speed = 4;
    public Vector3 currentTarget;
    public bool walkingTowardsEnd;

    private void Awake() {
        startPoint = transform.TransformPoint(startPoint);
        endPoint = transform.TransformPoint(endPoint);
        transform.position = startPoint;
        currentTarget = endPoint;
        walkingTowardsEnd = true;
    }

    private void Update() {
        var newPos = Vector3.MoveTowards(transform.position, currentTarget, Time.deltaTime * speed);
        if(Vector3.Distance(newPos, currentTarget) < 0.01f) {
            currentTarget = walkingTowardsEnd ? startPoint : endPoint;
            walkingTowardsEnd = !walkingTowardsEnd;
        }
        transform.position = newPos;
    }

    private void OnDrawGizmos() {
        using(new GizmosUtility.MatrixScope(transform)) {
            Gizmos.DrawSphere(startPoint, 0.25f);
            Gizmos.DrawSphere(endPoint, 0.25f);
        }
    }
}
