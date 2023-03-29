using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class GameData : MonoBehaviour {

    public static GameData Instance;

    public GameObject player;
    public Checkpoint currentCheckpoint;
    public int checkpointNum;
    public Vector2 lastCheckPointPos;

    public Tilemap groundTilemap;
    public Tilemap rootTilemap;
    public Tilemap utilityTilemap;

    

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        }
        else {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        GetVariables();
    }

    public void GetVariables() {
        player = GameObject.Find("Player");
        groundTilemap = GameObject.Find("Ground Tilemap").GetComponent<Tilemap>();
        rootTilemap = GameObject.Find("Root Tilemap").GetComponent<Tilemap>();
        utilityTilemap = GameObject.Find("Utility Tilemap").GetComponent<Tilemap>();

    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
            GetVariables();
        }


        if (Input.GetKeyDown(KeyCode.F)) {
            player = GameObject.Find("Player");
            groundTilemap = GameObject.Find("Ground Tilemap").GetComponent<Tilemap>();
            rootTilemap = GameObject.Find("Root Tilemap").GetComponent<Tilemap>();
            utilityTilemap = GameObject.Find("Utility Tilemap").GetComponent<Tilemap>();
        }


    }



}