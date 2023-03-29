using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(BoxCollider2D))]
public class Button : MonoBehaviour
{

    public bool isPressed;
    bool lastPressed;
    [SerializeField] public Direction direction;

    public Vector3Int position = Vector3Int.zero;

    [SerializeField] public GameObject negative;
    [SerializeField] public GameObject positive;

    List<BlinkBlock> negativeBlocks;
    List<BlinkBlock> positiveBlocks;


    [SerializeField] Tilemap rootTilemap;
    [SerializeField] Tilemap groundTilemap;
    [SerializeField] Tilemap utilityTilemap;

    public ButtonTiles buttonTiles;

    BoxCollider2D myCollider;

    List<Collider2D> collisions;

    void OnTriggerEnter2D(Collider2D col) {
        collisions.Add(col);
    }
    void OnTriggerExit2D(Collider2D col) {
        collisions.Remove(col);
    }
     private void Update() {
        if(collisions.Count > 0) isPressed = true;
        else if (rootTilemap.GetTile(position) != null) isPressed = true;
        else isPressed = false;
        if(lastPressed != isPressed) {
            AudioManager.Instance.Play("button");
            utilityTilemap.SetTile(position, GetButtonTile());
            if (isPressed) {
                foreach (var block in positiveBlocks) {
                    utilityTilemap.SetTile(block.position, block.onTile);
                }
                foreach (var block in negativeBlocks) {
                    utilityTilemap.SetTile(block.position, block.offTile);
                }
            }
            else {
                foreach (var block in positiveBlocks) {
                    utilityTilemap.SetTile(block.position, block.offTile);
                }
                foreach (var block in negativeBlocks) {
                    utilityTilemap.SetTile(block.position, block.onTile);
                }
            }
        }
        lastPressed = isPressed;
    }

    private void Start() {
        rootTilemap = GameData.Instance.rootTilemap;
        groundTilemap = GameData.Instance.groundTilemap;
        utilityTilemap = GameData.Instance.utilityTilemap;
        collisions = new List<Collider2D>();
        myCollider = GetComponent<BoxCollider2D>();
        GetComponent<SpriteRenderer>().enabled = false;
        SetInitialButtonTile();
        InitBlocks();
    }

    private void InitBlocks() {
        negativeBlocks = new List<BlinkBlock>();
        positiveBlocks = new List<BlinkBlock>();
        foreach (Transform child in negative.transform) {
            BlinkBlock block = child.GetComponent<BlinkBlock>();
            if(block != null) negativeBlocks.Add(block);
        }
        foreach (Transform child in positive.transform) {
            BlinkBlock block = child.GetComponent<BlinkBlock>();
            if (block != null) positiveBlocks.Add(block);
        }
    }

    private void SetInitialButtonTile() {
        position.x = (int)Mathf.Floor(transform.position.x);
        position.y = (int)Mathf.Floor(transform.position.y);
        transform.position = position + new Vector3(0.5f, 0.5f, 0f);
        utilityTilemap.SetTile(position, GetButtonTile());
    }


    Tile GetButtonTile() {
        switch (direction) {
            case Direction.Up: return isPressed ? buttonTiles.up_pressed : buttonTiles.up;
            case Direction.Down: return isPressed ? buttonTiles.down_pressed : buttonTiles.down;
            case Direction.Left: return isPressed ? buttonTiles.left_pressed : buttonTiles.left;
            case Direction.Right: return isPressed ? buttonTiles.right_pressed : buttonTiles.right;
        }
        return buttonTiles.up;
    }
}
[System.Serializable]
public class ButtonTiles {
    public Tile up;
    public Tile down;
    public Tile left;
    public Tile right;

    public Tile up_pressed;
    public Tile down_pressed;
    public Tile left_pressed;
    public Tile right_pressed;


}
