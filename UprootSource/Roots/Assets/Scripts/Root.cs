using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Root : MonoBehaviour
{

    //[SerializeField] float pushForce = 10f;

    [SerializeField] int maxLength;
    public int currLength;

    public Vector3Int position = Vector3Int.zero;
    public List<RootStem> rootStems;
    public Tilemap rootTilemap;
    public Tilemap groundTilemap;
    public Tilemap utilityTilemap;

    [SerializeField] Tile rootTile;
    [SerializeField] Tile rootTileFlower;
    [SerializeField] RootTiles rootTiles;
    [SerializeField] RuleTile stone;
    [SerializeField] Tile blinkBlockOn;

    public LayerMask pushLayerMask;

    [SerializeField] NumberDisplay numDisplay;

    List<Pushable> rootPushables;

    [SerializeField] Tilemap testMap;

    public bool preExtended;

    public float growSoundCooldown = 0.1f;
    private void Start() {

        rootTilemap = GameData.Instance.rootTilemap;
        groundTilemap = GameData.Instance.groundTilemap;
        utilityTilemap = GameData.Instance.utilityTilemap;

        numDisplay.display = maxLength - currLength;
        if(rootStems == null) rootStems = new List<RootStem>();

        GetComponent<SpriteRenderer>().enabled = false;
        SetInitialRootTile();
        rootPushables = new List<Pushable>();

        if (preExtended) {
            SetAllTiles();
        }

    }

    void SetAllTiles() {
        foreach (var stem in rootStems) {
            rootTilemap.SetTile(stem.position, stem.currentTile);
        }
        transform.position = rootTilemap.GetCellCenterLocal(rootStems[rootStems.Count - 1].position);
        numDisplay.display = 0;
    }

    private void SetInitialRootTile() {
        position.x = (int)Mathf.Floor(transform.position.x);
        position.y = (int)Mathf.Floor(transform.position.y);
        transform.position = position + new Vector3(0.5f, 0.5f, 0f);
        rootTilemap.SetTile(position, rootTileFlower);
    }

    public void GrowStem(Vector3 mouseInput) {
     
        Vector3Int hoverCell = rootTilemap.LocalToCell(mouseInput); //The Cell Mouse is Hovering Over
        Vector3Int currCell = rootTilemap.LocalToCell(transform.position);//The Cell root is in

        if (currCell != hoverCell) { //initCell != hoverCell && 
            //Check if hovercell is adjacent to the current cell, if so grow that direction
            if ((new Vector3Int(currCell.x, currCell.y + 1, 0)) == hoverCell) {
                StemBranch(Direction.Up);
            }
            if ((new Vector3Int(currCell.x, currCell.y - 1, 0)) == hoverCell) {
                StemBranch(Direction.Down);
            }
            if ((new Vector3Int(currCell.x + 1, currCell.y, 0)) == hoverCell) {
                StemBranch(Direction.Right);
            }
            if ((new Vector3Int(currCell.x - 1, currCell.y, 0)) == hoverCell) {
                StemBranch(Direction.Left);
            }
        }
    }


    float playTime = 0;
    public void StemBranch(Direction direction) {
        Vector3Int newRootPos = this.position;
        RootStem lastStem = null;

        if (rootStems.Count - 1 >= 0) {
            lastStem = rootStems[rootStems.Count - 1]; //Get the Last Stem
            if (IsOppositeDirection(direction, lastStem.direction)) {
                //Remove Last Stem if Opposite Directions
                rootStems.Remove(lastStem);
                rootTilemap.SetTile(lastStem.position, null);
                UpdateNewLastTile();
                //Update New Root Game Object Position
                transform.position += GetDirectionOffset(direction);
                currLength--;
                numDisplay.display = maxLength - currLength;
                return;
            }
            newRootPos = lastStem.position; //Position to Create New Root
        }

        newRootPos += GetDirectionOffset(direction);

        if (CanGrowConditions(newRootPos, direction)) {
            if(Time.time - playTime > growSoundCooldown) {
                AudioManager.Instance.PlayOne("grow");
                playTime = Time.time;
            }
            
            transform.position = rootTilemap.GetCellCenterWorld(newRootPos);
            RootStem newStem = new RootStem(newRootPos, direction);
            rootStems.Add(newStem);

            Tile newTile = rootTiles.SelectTile(direction); //Find and set new tile
            newStem.currentTile = newTile;
            rootTilemap.SetTile(newRootPos, newTile);

            if (lastStem != null) { //If there was a previous stem, update its Tile
                Tile lastTile = rootTiles.SelectTile(lastStem.direction, direction);
                lastStem.currentTile = lastTile;
                rootTilemap.SetTile(lastStem.position, lastTile);
            }
            currLength++;
            numDisplay.display = maxLength - currLength;
        }
    }
    public bool CanGrowConditions(Vector3Int newRootPos, Direction direction) {
        if (currLength >= maxLength) return false;
        if (rootTilemap.GetTile(newRootPos) != null) return false;
        if (groundTilemap.GetTile(newRootPos) == stone) return false;
        if (utilityTilemap.GetTile(newRootPos) == blinkBlockOn) return false;

        Vector3 offset = GetDirectionOffset(direction);
        Vector3 baseStem = rootTilemap.GetCellCenterWorld(newRootPos) - offset;
        GetPushables(baseStem, offset, null);
        if (rootPushables.Count > 0 && !EnoughRoomForPushables()) return false;
        foreach (var pushable in rootPushables) {
            pushable.Clear();
        }
        rootPushables.Clear();
       
        return true;
    }

    private bool EnoughRoomForPushables() {
        List<Pushable> tested = new List<Pushable>();

        foreach (var pushable in rootPushables) {
            pushable.transform.position += pushable.pushDistance; //move them to check
            tested.Add(pushable);
            if (groundTilemap.GetTile(groundTilemap.WorldToCell(pushable.transform.position)) != null ||
               rootTilemap.GetTile(rootTilemap.WorldToCell(pushable.transform.position)) != null ||
               utilityTilemap.GetTile(utilityTilemap.WorldToCell(pushable.transform.position)) == blinkBlockOn) {     
                foreach (var movedPushable in tested) {       
                    movedPushable.transform.position -= movedPushable.pushDistance; //move em back cause it aint valid
                    movedPushable.Clear();
                }
                rootPushables.Clear();
                return false;
            }
        }
        return true;
    }


    private void GetPushables(Vector3 origin, Vector3 offset, GameObject self) {
        RaycastHit2D[] pushables;
        Vector3 normOffset = offset.normalized;


        Vector2 boxSize = new Vector2(1f, 1f);
        if (self != null) {
            BoxCollider2D box = self.GetComponent<BoxCollider2D>();
            boxSize = box.size;
            origin += (Vector3)box.offset;
        }

        Vector2 halfVactor = normOffset;
        halfVactor.x = halfVactor.x == 0f ? 1f : 0.5f;
        halfVactor.y = halfVactor.y == 0f ? 1f : 0.5f;

        Vector2 actuallSize = Vector2.Scale(halfVactor, boxSize);
        Vector2 size = actuallSize * 0.90f; //scale down a little

        float actualWidth = actuallSize.x > actuallSize.y ? actuallSize.y : actuallSize.x;
        float width = size.x > size.y ? size.y : size.x;

        float distance = Mathf.Abs(Vector2.Dot(offset, boxSize)) + actualWidth - width/2;

        pushables = Physics2D.BoxCastAll(origin, size, 0, offset.normalized, distance, pushLayerMask);

        if (pushables.Length > 0) {

            foreach (var pushable in pushables) {
                Pushable pushie = pushable.transform.GetComponent<Pushable>();
                if (pushie != null && !pushie.beenPushed && pushable.collider.GetType() == typeof(BoxCollider2D)) {

                    Vector3 pointToOrigin = origin - (Vector3)pushable.point;
                    pointToOrigin = new Vector3(Mathf.Abs(pointToOrigin.x), Mathf.Abs(pointToOrigin.y));

                    Vector3 totalDistance = offset.normalized * (distance + (width / 2));
                    totalDistance = new Vector3(Mathf.Abs(totalDistance.x), Mathf.Abs(totalDistance.y));

                    Vector3 pushOffset = totalDistance - pointToOrigin;
                    pushOffset = Vector3.Scale(pushOffset, offset.normalized);

                    pushie.beenPushed = true;
                    pushie.pushDistance = pushOffset;
                    rootPushables.Add(pushie);
                    GetPushables(pushable.transform.position, pushOffset, pushable.transform.gameObject);
                    //pushable.transform.position += pushOffset;
                    pushie.beenPushed = false;
                    return;
                }
            }
        }
    }

    private void UpdateNewLastTile() {
        if (rootStems.Count - 1 >= 0) { //Update New Last Tile Sprite
            RootStem lastRoot = rootStems[rootStems.Count - 1];
            Tile endTile = rootTiles.SelectTile(lastRoot.direction);
            lastRoot.currentTile = endTile;
            rootTilemap.SetTile(lastRoot.position, endTile);
        }
    }




    private Vector3Int GetDirectionOffset(Direction direction) {
        switch (direction) {
            case Direction.Up:
                return new Vector3Int(0, 1, 0);
            case Direction.Down:
                return new Vector3Int(0, -1, 0);
            case Direction.Left:
                return new Vector3Int(-1, 0, 0);
            case Direction.Right:
                return new Vector3Int(1, 0, 0);
        }
        Debug.LogError("WARNING WATCH OUT HERE!");
        return Vector3Int.zero;
    }



    private bool IsOppositeDirection(Direction dir1, Direction dir2) {
        switch (dir1) {
            case Direction.Up:
                if (dir2 == Direction.Down) return true;
                break;
            case Direction.Down:
                if (dir2 == Direction.Up) return true;
                break;
            case Direction.Right:
                if (dir2 == Direction.Left) return true;
                break;
            case Direction.Left:
                if (dir2 == Direction.Right) return true;
                break;
        }
        return false;
    }
}
[System.Serializable]
public class RootStem {
    public Vector3Int position;
    public Direction direction;
    public Tile currentTile;

    public RootStem(Vector3Int position, Direction direction) {
        this.position = position;
        this.direction = direction;
    }
}
[System.Serializable]
public class RootTiles {
    public Tile hori;
    public Tile vert;

    public Tile up;
    public Tile down;
    public Tile left;
    public Tile right;

    public Tile URLD; //read as "Up-Right or Left-Down"
    public Tile ULRD;
    public Tile DRLU;
    public Tile DLRU;




    public Tile SelectTile(Direction currDir, Direction nextDir) {
        if (currDir == nextDir) {
            if (currDir == Direction.Up || currDir == Direction.Down) {
                return vert;
            }
            else if (currDir == Direction.Left || currDir == Direction.Right) {
                return hori;
            }
        }
        if (currDir == Direction.Up) {
            if (nextDir == Direction.Right) return URLD; //UR
            if (nextDir == Direction.Left) return ULRD;  //UL
        }
        if (currDir == Direction.Down) {
            if (nextDir == Direction.Right) return DRLU; //DR
            if (nextDir == Direction.Left) return DLRU;  //DL
        }
        if (currDir == Direction.Left) {
            if (nextDir == Direction.Down) return URLD; //LD
            if (nextDir == Direction.Up) return DRLU;   //LU
        }
        if (currDir == Direction.Right) {
            if (nextDir == Direction.Down) return ULRD; //RD
            if (nextDir == Direction.Up) return DLRU;   //RU
        }
        Debug.LogError("NO VALID TILE");
        return null;
    }
    public Tile SelectTile(Direction currDir) { //assuming no next root
        switch (currDir) {
            case Direction.Up: return up;
            case Direction.Down: return down;
            case Direction.Left: return left;
            case Direction.Right: return right;
        }
        Debug.LogError("NO VALID TILE");
        return null;
    }


}
public enum Direction {
    Up,
    Down,
    Left,
    Right,
    None
}
