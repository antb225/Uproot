using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Pushable : MonoBehaviour
{
    public bool beenPushed;
    public Vector3 pushDistance;
    public BoxCollider2D boxCollider;
    public CircleCollider2D noGoZone;
    public Vector2 size;

    private void Start() {
        boxCollider = GetComponent<BoxCollider2D>();
        size = boxCollider.size;
    }

    public void Clear() {
        beenPushed = false;
        pushDistance = Vector3.zero;
    }

}
