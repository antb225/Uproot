using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCheckpointManager : MonoBehaviour
{
    private void Awake() {
        GameData.Instance.GetVariables();
    }
}
