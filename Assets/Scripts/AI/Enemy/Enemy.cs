using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : NetworkBehaviour {
    [SerializeField] protected int health;
    [SerializeField] protected int damage;
    [SerializeField] protected float attackRange;
    [SerializeField] protected float timeToPrepareAttack;
    [SerializeField] protected float timeBetweenAttacks;

    protected bool canAttack = true;

    protected GameObject player;
    protected EnemyMovement enemyMovement;
    protected SpriteRenderer spriteRenderer;

    protected virtual void Start() {
        enemyMovement = GetComponent<EnemyMovement>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (!isServer) return;

        ChooseRandomPlayer();
    }

    private void ChooseRandomPlayer() {
        List<NetworkIdentity> playerIdentities = new List<NetworkIdentity>();

        foreach (var connection in NetworkServer.connections.Values) {
            if (connection.identity.gameObject.activeSelf) {
                playerIdentities.Add(connection.identity);

            }
        }

        if (playerIdentities.Count > 0) {
            int randomIndex = Random.Range(0, playerIdentities.Count);
            print("players number: "+playerIdentities.Count + " randomIndex " + randomIndex);
            player = playerIdentities[randomIndex].gameObject;
            enemyMovement.SetPlayerTransform(player.transform);
        }
        else {
            player = null;
            Debug.Log("Active player not found");

        }
    }

    protected virtual void Update() {
        if (!isServer) return;
        if (player && !player.activeSelf) {
            ChooseRandomPlayer();
        }
        if (!player) return;
        Vector2 enemyPosition = transform.position;
        Vector2 playerPosition = player.transform.position;
        if (Vector2.Distance(playerPosition, enemyPosition) <= attackRange) {
            if (canAttack) {
                Attack();
                StartCoroutine(AttackReload());
            }
        }
    }

    protected virtual IEnumerator AttackReload() {
        canAttack = false;
        yield return new WaitForSeconds(timeBetweenAttacks+timeToPrepareAttack);
        canAttack = true;
    }

    protected abstract void Attack();

    public void TakeDamage(int damage) {
        if (!isServer) return;

        RpcUpdateColor(new Color(1, .5f, .5f));

        if (health - damage <= 0) {
            health = 0;
            EnemyDeath();
        }
        else {
            health -= damage;
        }

        StartCoroutine(ReturnColor());
    }

    [ClientRpc]
    private void RpcUpdateColor(Color newColor) {
        spriteRenderer.color = newColor;
    }

    private IEnumerator ReturnColor() {
        yield return new WaitForSeconds(.25f);
        RpcUpdateColor(Color.white);

    }

    private void EnemyDeath() {
        // animation of death
        WaveController.ReduceNumberOfEnemies();
        enemyMovement.enabled = false;
        Destroy(gameObject, .1f);

    }
}
