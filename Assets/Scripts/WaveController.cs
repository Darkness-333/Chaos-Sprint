using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveController : NetworkBehaviour {
    [SerializeField] private GameObject startHintText;
    [SerializeField] private GameObject winText;

    [SerializeField] private GameObject meleeEnemy;
    [SerializeField] private GameObject rangedEnemy;

    [SerializeField] private int level;
    [SerializeField] private List<LevelDifficultySO> levelDifficultyList;

    public static LevelMode Mode { get; set; }
    public static int Level { get; set; }

    private static int numberOfEnemies;

    private static int waveNumber;

    private float horizontalBorders = 12;
    private float verticalBorders = 8;

    private LevelDifficultySO levelDifficulty;

    private bool gameStarted = false;

    public static int GetWaveNumber() {
        return waveNumber;
    }

    public LevelDifficultySO GetLevelDifficulty() {
        return levelDifficultyList[level - 1];
    }

    public static void ReduceNumberOfEnemies() {
        numberOfEnemies--;
    }

    private void Start() {
        if (!isServer) return;
        startHintText.SetActive(true);
        if(Level != 0) { 
            level = Level;
        }
        levelDifficulty = GetLevelDifficulty();
        Mode=LevelMode.Simple;
        numberOfEnemies = 0;
        waveNumber = 0;
        //if (!gameStarted) return;
        //numberOfEnemies = (int)(levelDifficulty.numberOfEnemies + (waveNumber - 1) * levelDifficulty.numberOfEnemiesCoeff);
        //StartCoroutine(SpawnEnemies());
    }

    private void Update() {
        if (!isServer) return;

        if (Input.GetKeyDown(KeyCode.Space)) {
            gameStarted = true;
            startHintText.SetActive(false);
        }

        if (numberOfEnemies == 0 && gameStarted) {
            if (Mode==LevelMode.Simple && waveNumber == 10) {
                winText.SetActive(true);
                return;
            }
            waveNumber++;
            RpcUpdateWaveNumber(waveNumber);
            numberOfEnemies = (int)(levelDifficulty.numberOfEnemies + (waveNumber - 1) * levelDifficulty.numberOfEnemiesCoeff);
            StartCoroutine(SpawnEnemies());
        }
    }

    [ClientRpc]
    private void RpcUpdateWaveNumber(int newWaveNumber) {
        GameObject.Find("WaveNumberText").GetComponent<TextMeshProUGUI>().text = "Волна " + newWaveNumber;
    }

    private IEnumerator SpawnEnemies() {
        yield return new WaitForSeconds(1);
        for (int i = 0; i < numberOfEnemies; i++) {
            Vector2 position = new Vector2();
            if (Random.value < .5f) {
                position.x = horizontalBorders;
                if (Random.value < .5f) {
                    position.x = -horizontalBorders;
                }
                position.y = Random.Range(-verticalBorders, verticalBorders);
            }
            else {
                position.y = verticalBorders;
                if (Random.value < .5f) {
                    position.y = -verticalBorders;
                }
                position.x = Random.Range(-horizontalBorders, horizontalBorders);
            }

            // Суммируем шансы
            float totalChance = levelDifficulty.meleeEnemyChance + levelDifficulty.rangedEnemyChance;
            float randomValue = Random.Range(0f, totalChance);

            // Определяем, какой враг был выбран
            if (randomValue <= levelDifficulty.meleeEnemyChance) {
                GameObject enemy = Instantiate(meleeEnemy, position, Quaternion.identity);
                NetworkServer.Spawn(enemy);
            }
            else {
                GameObject enemy = Instantiate(rangedEnemy, position, Quaternion.identity);
                NetworkServer.Spawn(enemy);
            }

        }
    }

}
