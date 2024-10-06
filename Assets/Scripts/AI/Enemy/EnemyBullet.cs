using Mirror;
using UnityEngine;

public class EnemyBullet : NetworkBehaviour {
    public float speed = 10f;
    public int damage;

    private Rigidbody2D rb;
    public void SetDamage(int damage) {
        this.damage = damage;
    }

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;
        Destroy(gameObject, 20);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(!isServer) return;
        Player player = collision.GetComponent<Player>();
        if (player != null) {
            //player.TakeDamage(damage);
            //Destroy(gameObject);
            RpcPerformAttack(player, damage);
        }
    }

    [ClientRpc]
    private void RpcPerformAttack(Player player, int damage) {
        print("ranged enemy attacked player");
        player.TakeDamage(damage);
        Destroy(gameObject);

    }

}