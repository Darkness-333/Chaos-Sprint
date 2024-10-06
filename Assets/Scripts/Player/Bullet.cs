using Mirror;
using UnityEngine;

public class Bullet : NetworkBehaviour {
    public float speed = 10f;
    private Rigidbody2D rb;
    private int damage;

    public void SetDamage(int damage) {
        this.damage = damage;
    }

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (isServer) {

            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null) {
                enemy.TakeDamage(damage);
            }
        }
        Destroy(gameObject);
    }

}
