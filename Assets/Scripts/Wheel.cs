using UnityEngine;
using System.Collections;
[RequireComponent(typeof(HingeJoint2D))]
public class Wheel : ConstructionSegment
{
    Rigidbody2D rb;
    float force = 400;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float motor = Input.GetAxis("Horizontal");
        rb.angularVelocity = -motor * force;
    }
}
