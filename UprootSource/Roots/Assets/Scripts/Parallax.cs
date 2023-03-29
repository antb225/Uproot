using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float lengthX, startPosX;
    private float lengthY, startPosY;
    public GameObject cam;
    public float parallaxEffectX;
    public float parallaxEffectY;

    void Start() {
        startPosX = transform.position.x;
        startPosY = transform.position.y;
        lengthX = GetComponent<SpriteRenderer>().bounds.size.x;
        lengthY = GetComponent<SpriteRenderer>().bounds.size.y;
    }

    private void FixedUpdate() {
        float tempX = (cam.transform.position.x * (1 - parallaxEffectX));
        float distX = cam.transform.position.x * parallaxEffectX;

        float tempY = (cam.transform.position.y * (1 - parallaxEffectY));
        float distY = cam.transform.position.y * parallaxEffectY;

        transform.position = new Vector3(startPosX + distX, startPosY + distY, transform.position.z);

        if (tempX > startPosX + lengthX) startPosX += lengthX;
        else if (tempX < startPosX - lengthX) startPosX -= lengthX;

        if (tempY > startPosY + lengthY) startPosY += lengthY;
        else if (tempY < startPosY - lengthY) startPosY -= lengthY;

    }
}
