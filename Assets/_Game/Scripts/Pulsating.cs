using UnityEngine;

public class Pulsating : MonoBehaviour {
    public AnimationCurve xzCurve = AnimationCurve.Linear(0, 1f, 1f, 1f);
    public AnimationCurve yCurve = AnimationCurve.Linear(0, 1f, 1f, 1f);

    public float time = 0f;

    private void Awake() {
        time += Random.Range(0, 10);
    }

    void Update() {
        time += Time.deltaTime;

        var xz = xzCurve.Evaluate(time);
        var y = yCurve.Evaluate(time);
        transform.localScale = new Vector3(xz, y, xz);
    }
}
