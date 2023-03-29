using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Checkpoint : MonoBehaviour
{
    public int checkpointNum = 1;

    public bool active = false;

    [SerializeField] Tilemap utilityTilemap;

    public Tile activeTile;
    public Tile notActiveTile;

    Vector3Int position;


    private void Start() {
        utilityTilemap = GameObject.Find("Utility Tilemap").GetComponent<Tilemap>();
        if (GameData.Instance.checkpointNum >= checkpointNum) active = true;
        GetComponent<SpriteRenderer>().enabled = false;
        SetInitialButtonTile();
    }

    private void SetInitialButtonTile() {
        position.x = (int)Mathf.Floor(transform.position.x);
        position.y = (int)Mathf.Floor(transform.position.y);
        transform.position = position + new Vector3(0.5f, 0.5f, 0f);
        Tile currentTile = active ? activeTile : notActiveTile;
        utilityTilemap.SetTile(position, currentTile);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            Debug.Log(checkpointNum);
            Debug.Log(GameData.Instance.currentCheckpoint.checkpointNum);
            if (checkpointNum >= GameData.Instance.currentCheckpoint.checkpointNum ) {
                active = true;
                GameData.Instance.currentCheckpoint = this;
                GameData.Instance.checkpointNum = checkpointNum;
                GameData.Instance.lastCheckPointPos = transform.position;
                utilityTilemap.SetTile(position, activeTile);
                AudioManager.Instance.Play("checkpoint");
            }
        }
    }



}
