using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ShootPosition : MonoBehaviour {
    [SerializeField] private Vector3 firstPosition;
    [SerializeField] private Vector3 secondPosition;

    public void SetFirstPosition() {
        transform.localPosition = firstPosition;
    }

    public void SetSecondPosition() {
        transform.localPosition = secondPosition;
    }
}
