using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyParams {
    public int health;
    public int damage;
    public float speed;
    public float attackRange;
    public float timeToPrepareAttack;
    public float timeBetweenAttacks;
}

[System.Serializable]
public class MeleeEnemyParams:EnemyParams {
    public float attackDuration;
}

[System.Serializable]
public class RangedEnemyParams : EnemyParams {
    public float bulletSpeed;
    public float distanceFromPlayer;
}


[System.Serializable]
public class EnemyCoeffs {
    public float healthCoeff;
    public float damageCoeff;
    public float speedCoeff;
    public float attackRangeCoeff;
    public float timeToPrepareAttackCoeff;
    public float timeBetweenAttacksCoeff;
}

[System.Serializable]
public class MeleeEnemyCoeffs : EnemyCoeffs{
    public float attackDurationCoeff;
}

[System.Serializable]
public class RangedEnemyCoeffs : EnemyCoeffs {
    public float bulletSpeedCoeff;
    public float distanceFromPlayerCoeff;
}

[CreateAssetMenu(menuName = "LevelDifficultySO")]
public class LevelDifficultySO : ScriptableObject {

    [Header("MeleeEnemy Settings")]
    public MeleeEnemyParams meleeEnemyParams;
    public MeleeEnemyCoeffs meleeEnemyCoeffs;

    [Header("RangedEnemy Settings")]
    public RangedEnemyParams rangedEnemyParams;
    public RangedEnemyCoeffs rangedEnemyCoeffs;

    [Header("Wave Settings")]
    public int numberOfEnemies;
    public float numberOfEnemiesCoeff;
    public float meleeEnemyChance;
    public float rangedEnemyChance;


}
