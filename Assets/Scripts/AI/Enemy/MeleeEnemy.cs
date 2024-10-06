using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : Enemy {
    [SerializeField] private float attackDuration;

    private Rigidbody2D rb;
    private CapsuleCollider2D capsuleCollider;
    private Animator animator;

    private WaveController waveController;
    protected override void Attack() {
        if (!isServer) return;

        StartCoroutine(MeleeAttack());
    }

    protected override void Start() {

        base.Start();
        if (!isServer) return;
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();

        waveController = FindObjectOfType<WaveController>();

        SetParams();
    }

    private void SetParams() {
        LevelDifficultySO levelDifficulty = waveController.GetLevelDifficulty();
        int waveNumber = WaveController.GetWaveNumber()-1;

        MeleeEnemyParams p = levelDifficulty.meleeEnemyParams;
        MeleeEnemyCoeffs c = levelDifficulty.meleeEnemyCoeffs;

        health = (int)(p.health + waveNumber * c.healthCoeff);
        damage = (int)(p.damage + waveNumber * c.damageCoeff);
        enemyMovement.speed = p.speed + waveNumber * c.speedCoeff;
        attackRange = p.attackRange + waveNumber * c.attackRangeCoeff;
        timeToPrepareAttack = Mathf.Max(0, p.timeToPrepareAttack - waveNumber * c.timeToPrepareAttackCoeff);
        timeBetweenAttacks = Mathf.Max(0, p.timeBetweenAttacks - waveNumber * c.timeBetweenAttacksCoeff);
        attackDuration = Mathf.Max(0, p.attackDuration - waveNumber * c.attackDurationCoeff);

    }

    private IEnumerator MeleeAttack() {
        Vector2 direction = (player.transform.position - transform.position).normalized;
        Vector2 startPosition = transform.position;  
        Vector2 targetPosition = (Vector2)transform.position + direction * attackRange; 

        float distanceToPlayer = Vector2.Distance(player.transform.position, transform.position);
        RaycastHit2D hit = Physics2D.Raycast(startPosition, direction, attackRange, LayerMask.GetMask("Obstacle"));
        // если есть препятствие и оно за игроком
        if (hit) {
            if (Vector2.Distance(hit.transform.position, transform.position) > distanceToPlayer) {
                targetPosition = (Vector2)transform.position + direction * distanceToPlayer;
            }
            else {
                yield break;
            }
        }

        hit = Physics2D.Raycast(startPosition + Vector2.up * .2f + Vector2.right * .4f, direction, attackRange, LayerMask.GetMask("Obstacle"));
        if (hit) {
            if (Vector2.Distance(hit.transform.position, transform.position) > distanceToPlayer) {
                targetPosition = (Vector2)transform.position + direction * distanceToPlayer;
            }
            else {
                yield break;
            }
        }

        hit = Physics2D.Raycast(startPosition + Vector2.up * .2f + Vector2.left * .4f, direction, attackRange, LayerMask.GetMask("Obstacle"));
        if (hit) {
            if (Vector2.Distance(hit.transform.position, transform.position) > distanceToPlayer) {
                targetPosition = (Vector2)transform.position + direction * distanceToPlayer;
            }
            else {
                yield break;
            }
        }

        rb.velocity = Vector2.zero;
        enemyMovement.enabled = false;
        capsuleCollider.enabled = false; // возможно лучше использовать после timeToPrepareAttack (но это не точно)
        animator.SetBool("isRunning", false);
        RpcUpdateColor(new Color(200 / 256f, 200 / 256f, 0));

        yield return new WaitForSeconds(timeToPrepareAttack);


        float elapsedTime = 0f;
        bool playerAttacked = false;
        while (elapsedTime < attackDuration) {
            rb.MovePosition(Vector2.Lerp(startPosition, targetPosition, elapsedTime / attackDuration));

            LayerMask mask = LayerMask.GetMask("Player");
            Collider2D collider = Physics2D.OverlapBox(transform.position + Vector3.up * .2f, new Vector2(.82f, .42f), 0, mask);
            if (collider != null && !playerAttacked) {
                playerAttacked = true;
                RpcPerformAttack(collider.gameObject, damage);
            }
            elapsedTime += Time.deltaTime;
            yield return null;  
        }

        capsuleCollider.enabled = true;

        enemyMovement.enabled = true;
        animator.SetBool("isRunning", true);
        RpcUpdateColor(Color.white);
    }

    [ClientRpc]
    private void RpcPerformAttack(GameObject collider, int damage) {
        Player player = collider.GetComponent<Player>();
        player.TakeDamage(damage);
    }

    [ClientRpc]
    private void RpcUpdateColor(Color newColor) {
        spriteRenderer.color = newColor;
    }

    protected override IEnumerator AttackReload() {
        canAttack = false;
        yield return new WaitForSeconds(timeBetweenAttacks + timeToPrepareAttack + attackDuration);
        canAttack = true;
    }

}
