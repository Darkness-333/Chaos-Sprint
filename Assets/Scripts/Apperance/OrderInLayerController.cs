using UnityEngine;

public class OrderInLayerController : MonoBehaviour {
    [SerializeField] private bool alwaysUpdate=false;

    private SpriteRenderer spriteRenderer;

    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * -10);

    }

    void FixedUpdate() {
        if (alwaysUpdate) {
            spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * -10);

        }
    }
}
