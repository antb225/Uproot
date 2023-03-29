using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BlinkBlock : MonoBehaviour
{
    public bool active;

    public Vector3Int position = Vector3Int.zero;

    [SerializeField] public Tile onTile;
    [SerializeField] public Tile offTile;

    [SerializeField] Tilemap utilityTilemap;


    private void Start() {
        utilityTilemap = GameData.Instance.utilityTilemap;
        GetComponent<SpriteRenderer>().enabled = false;
        SetInitialBlinkBlockTile();
    }

    private void SetInitialBlinkBlockTile() {
        position.x = (int)Mathf.Floor(transform.position.x);
        position.y = (int)Mathf.Floor(transform.position.y);
        transform.position = position + new Vector3(0.5f, 0.5f, 0f);
        Tile blickTile = active ? onTile : offTile;
        utilityTilemap.SetTile(position, blickTile);
    }

}
