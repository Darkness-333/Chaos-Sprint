using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugStart : MonoBehaviour
{
    void Start()
    {
        NetworkManager.singleton.maxConnections = 10;

        NetworkManager.singleton.StartHost();
    }

}
