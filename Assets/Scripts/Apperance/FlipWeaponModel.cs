using UnityEngine;

public class FlipWeaponModel : MonoBehaviour {

    [SerializeField] private SpriteRenderer model;
    [SerializeField] private ShootPosition shootPosition;

    void Update() {

        if (Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad)>=0) {
            model.flipY = false;
            shootPosition.SetFirstPosition();
        }
        else {
            model.flipY = true;
            shootPosition.SetSecondPosition();
        }
    }
}
