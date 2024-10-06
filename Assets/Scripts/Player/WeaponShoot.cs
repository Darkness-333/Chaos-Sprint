using Mirror;
using System.Collections;
using UnityEngine;

public class WeaponShoot : NetworkBehaviour {
    [SerializeField] private SpriteRenderer model;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private float reloadTime;
    [SerializeField] private int damage;

    private bool canShoot = true;
    void Update() {
        if (!isLocalPlayer) return;

        if (Input.GetMouseButtonDown(0) && canShoot) {
            Shoot();
            StartCoroutine(Reload());
        }
    }

    private void Shoot() {
        // �������� ������� ���� ��� ���������� �������� �����
        if (!isServer) {
            Bullet localBullet = Instantiate(bulletPrefab, shootPoint.position, transform.rotation);
            localBullet.SetDamage(damage);
        }

        // �������� ������� � ��������
        CmdShoot(shootPoint.position, transform.rotation, NetworkClient.localPlayer);
    }

    [Command]
    private void CmdShoot(Vector3 shootPosition, Quaternion shootRotation, NetworkIdentity excludeClient) {

        // ������� ���� �� �������
        Bullet bullet = Instantiate(bulletPrefab, shootPosition, shootRotation);
        bullet.SetDamage(damage);
        NetworkServer.Spawn(bullet.gameObject);
        RpcDestroyOnLocalPlayer(bullet.gameObject, excludeClient);
        
    }

    [ClientRpc]
    void RpcDestroyOnLocalPlayer(GameObject bullet, NetworkIdentity excludeClient) {
        if (isServer) return;
        // ���������� ���� �� �������, ������� � ������
        if (isLocalPlayer && connectionToClient == excludeClient.connectionToClient) {
            Destroy(bullet);
        }
    }

    // working variant
    //[Command]
    //private void CmdShoot(Vector3 shootPosition, Quaternion shootRotation) {
    //    Bullet bullet = Instantiate(bulletPrefab, shootPosition, shootRotation);
    //    bullet.SetDamage(damage);
    //    NetworkServer.Spawn(bullet.gameObject);

    //}

    private IEnumerator Reload() {
        canShoot = false;
        yield return new WaitForSeconds(reloadTime);
        canShoot = true;
    }
}
