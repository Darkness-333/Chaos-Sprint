using Cinemachine;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LocalPlayerSettings : NetworkBehaviour
{
    [SerializeField] CinemachineVirtualCamera virtualCamera;

    private void Start() {
        StartCoroutine(FindLocalPlayerAndSetCamera());
    }

    private IEnumerator FindLocalPlayerAndSetCamera() {
         GameObject player = null;

        while (player == null) {
            if (NetworkClient.connection != null && NetworkClient.connection.identity != null) {
                player = NetworkClient.connection.identity.gameObject; // Получаем локального игрока
            }

            yield return null; 
        }
        virtualCamera.Follow = player.transform;

    }
}
