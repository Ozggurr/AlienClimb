using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpindAndBob : MonoBehaviour
{
    public float spinSpeed = 90f, bobAmp = 0.25f, bobSpeed = 2f;
    Vector3 startPos;
    void Start() { startPos = transform.position; }
    void Update()
    {
        transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f, Space.World);
        transform.position = startPos + Vector3.up * Mathf.Sin(Time.time * bobSpeed) * bobAmp;
    }
}
