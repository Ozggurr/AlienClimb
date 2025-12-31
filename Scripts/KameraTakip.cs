using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class KameraTakip : MonoBehaviour
{
    public Transform player;
    public float distance = 5f;
    public float height = 2f;
    public float rotationSpeed = 2f;

    private float currentYaw = 0f;
    private Vector2 lastTouchPos;
    private bool isDragging = false;

    void LateUpdate()
    {
        if (player == null) return;

        // Dokunma kontrolü (mobil ve mouse destekli)
#if UNITY_EDITOR
        // Mouse ile test
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastTouchPos = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector2 delta = (Vector2)Input.mousePosition - lastTouchPos;
            currentYaw += delta.x * rotationSpeed * Time.deltaTime;
            lastTouchPos = Input.mousePosition;
        }
#else
        // Mobil dokunma
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                currentYaw += touch.deltaPosition.x * rotationSpeed * Time.deltaTime;
            }
        }
#endif

        // Kameranýn pozisyonu: oyuncuya göre döndürülmüþ offset
        Quaternion rotation = Quaternion.Euler(0f, currentYaw, 0f);
        Vector3 offset = rotation * new Vector3(0f, height, -distance);
        transform.position = player.position + offset;

        // Oyuncuya bak
        transform.LookAt(player.position + Vector3.up * 1.5f);
    }
}