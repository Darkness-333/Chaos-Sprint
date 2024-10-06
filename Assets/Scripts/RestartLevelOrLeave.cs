using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartLevelOrLeave : NetworkBehaviour
{

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if(NetworkServer.active && NetworkClient.isConnected)
            {
                print("stop host");
                NetworkManager.singleton.StopHost();
            }
            else if (NetworkClient.isConnected) {
                print("stop client");

                NetworkManager.singleton.StopClient();
            }
        }

        if (!isServer) return;

        if (Input.GetKeyDown(KeyCode.R)) {
            bool canRestart = true;
            foreach (var connection in NetworkServer.connections.Values) {
                if (connection.identity.gameObject.activeSelf) {
                    canRestart = false; 
                    break;
                }
            }
            if (canRestart) {

                // Меняем сцену (перезапускаем её)
                NetworkManager.singleton.ServerChangeScene(SceneManager.GetActiveScene().name);
            }
        }
    }

}
