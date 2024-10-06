using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : NetworkBehaviour {
    [SerializeField] private int health;
    private SpriteRenderer spriteRenderer;

    public event Action<int> HealthChanged;

    public override void OnStartLocalPlayer() {
        base.OnStartLocalPlayer();

        var healthUI = FindObjectOfType<GameUI>();
        if (healthUI != null) {
            healthUI.SetPlayer(this);
        }
    }
    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if(!isLocalPlayer) {
            spriteRenderer.color = new Color(200 / 256f, 1, 200 / 256f);
        }
    }

    public int GetHealth() {
        return health;
    }

    public void TakeDamage(int damage) {
        spriteRenderer.color = new Color(1, .5f, .5f);
        print("Player health before damage " + health);

        if (health - damage <= 0) {
            health = 0;
            HealthChanged?.Invoke(health);
            PlayerDeath();
            return;
        }
        else {
            health -= damage;
            HealthChanged?.Invoke(health);
        }
        StartCoroutine(ReturnColor());
    }

    private void PlayerDeath() {
        gameObject.SetActive(false);
    }

    private IEnumerator ReturnColor() {
        yield return new WaitForSeconds(.25f);
        spriteRenderer.color = Color.white;

    }

}
