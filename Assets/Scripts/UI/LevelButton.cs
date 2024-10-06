using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LevelMode {
    Simple,
    Infinitive
}

public class LevelButton : MonoBehaviour
{
    public LevelMode mode;
    public int level;
}
