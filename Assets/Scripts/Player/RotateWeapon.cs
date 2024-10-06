using Mirror;
using UnityEngine;

public class RotateWeapon : NetworkBehaviour {


    void Update() {
        if (!isLocalPlayer) return;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3 direction = mousePosition - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        CmdRotateWeapon(angle); // Отправляем угол вращения на сервер

    }

    [Command]
    void CmdRotateWeapon(float angle) {
        RpcRotateWeapon(angle); // Передаем вращение всем клиентам
    }

    [ClientRpc]
    void RpcRotateWeapon(float angle) {
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}

