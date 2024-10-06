using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;


public class RangedEnemy : Enemy {
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float distanceFromPlayer;

    [SerializeField] private EnemyBullet enemyBulletPrefab;

    private Rigidbody2D rb;
    private Animator animator;

    private WaveController waveController;
    protected override void Attack() {
        if (!isServer) return;

        StartCoroutine(RangedAttack());

    }

    protected override void Start() {

        base.Start();
        if (!isServer) return;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        waveController = FindObjectOfType<WaveController>();

        SetParameters();
    }


    private void SetParameters() {
        LevelDifficultySO levelDifficulty = waveController.GetLevelDifficulty();
        int waveNumber = WaveController.GetWaveNumber() - 1;

        RangedEnemyParams p = levelDifficulty.rangedEnemyParams;
        RangedEnemyCoeffs c = levelDifficulty.rangedEnemyCoeffs;

        health = (int)(p.health + waveNumber * c.healthCoeff);
        damage = (int)(p.damage + waveNumber * c.damageCoeff);
        enemyMovement.speed = p.speed + waveNumber * c.speedCoeff;
        attackRange = p.attackRange + waveNumber * c.attackRangeCoeff;
        timeToPrepareAttack = Mathf.Max(0, p.timeToPrepareAttack - waveNumber * c.timeToPrepareAttackCoeff);
        timeBetweenAttacks = Mathf.Max(0, p.timeBetweenAttacks - waveNumber * c.timeBetweenAttacksCoeff);
        bulletSpeed = p.bulletSpeed + waveNumber * c.bulletSpeedCoeff;
        distanceFromPlayer = p.distanceFromPlayer + waveNumber * c.distanceFromPlayerCoeff;
    }

    protected override void Update() {
        if (!isServer) return;
        base.Update();
        if (!player) return;

        Vector2 enemyPosition = transform.position;
        Vector2 playerPosition = player.transform.position;
        Vector2 directionToEnemy = (enemyPosition - playerPosition).normalized;
        if (Vector2.Distance(playerPosition, enemyPosition) <= distanceFromPlayer && !enemyMovement.targetChanged) {
            enemyMovement.targetPosition = playerPosition + directionToEnemy * 1.5f * attackRange;

            StartCoroutine(ReturnTarget());
        }
    }

    private IEnumerator ReturnTarget() {
        enemyMovement.targetChanged = true;
        yield return new WaitForSeconds(1);
        enemyMovement.targetChanged = false;

    }

    private IEnumerator RangedAttack() {
        Vector3 direction = player.transform.position - transform.position;

        if (direction.x < 0) {
            RpcUpdateFlip(true);
        }
        else {
            RpcUpdateFlip(false);

        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        rb.velocity = Vector2.zero;
        enemyMovement.enabled = false;
        animator.SetBool("isRunning", false);
        RpcUpdateColor(new Color(200 / 256f, 200 / 256f, 0));


        yield return new WaitForSeconds(timeToPrepareAttack);


        enemyMovement.enabled = true;
        animator.SetBool("isRunning", true);
        RpcUpdateColor(Color.white);


        EnemyBullet enemyBullet = Instantiate(enemyBulletPrefab, transform.position, rotation);
        enemyBullet.speed = bulletSpeed;
        enemyBullet.SetDamage(damage);
        NetworkServer.Spawn(enemyBullet.gameObject);

    }

    [ClientRpc]
    private void RpcUpdateFlip(bool flip) {
        if (spriteRenderer) {
            spriteRenderer.flipX = flip;
        }
    }

    [ClientRpc]
    private void RpcUpdateColor(Color newColor) {
        if (spriteRenderer) {
            spriteRenderer.color = newColor;

        }
    }
}
