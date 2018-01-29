using UnityEngine;
using System.Collections;

public class Suspension : Clamping
{
    void LateUpdate()
    {
        if (!initialized) return;
        UpdateClamping(Startjoint.transform.position, Endjoint.transform.position);
    }
}
