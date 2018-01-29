using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clamping : MonoBehaviour {
    public GameObject Startjoint;
    public GameObject Endjoint;
    public bool initialized = false;

    public void Init(GameObject a, GameObject b) {
        Startjoint = a;
        Endjoint = b;
        initialized = true;

        UpdateClamping(Startjoint.transform.position, Endjoint.transform.position);
    }

    public void UpdateClamping(Vector2 a, Vector2 b)
    {
        Vector2 dir = b - a;
        Vector2 pos = a + dir / 2;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        transform.eulerAngles = new Vector3(0, 0, angle);
        transform.position = pos;
        transform.localScale = new Vector3(dir.magnitude, 1, 1);
    }
}
