using Mirror;
using Network;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : NetworkBehaviour {
    [SerializeField] private RectTransform heartImage;

    [SerializeField] private TextMeshProUGUI waveNumberText;
    [SerializeField] private TextMeshProUGUI inviteCodeText;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button chooseLevelButton;
    [SerializeField] private Button copyTextButton;
    [SerializeField] private Button startLevelButton;

    [SerializeField] private Button helpButton;
    [SerializeField] private Button closeHelpPanel;
    [SerializeField] private GameObject helpPanel;


    [SerializeField] private GameLevelMenuUI levelMenu;



    private Player player;
    private int playerHealth;   
    private List<RectTransform> heartList;

    private void Start() {
        Time.timeScale = 1;
        copyTextButton.onClick.AddListener(CopyText);
        helpButton.onClick.AddListener(ShowHelpPanel);
        closeHelpPanel.onClick.AddListener(ShowHelpPanel);
        inviteCodeText.text = "Код входа: " + ((NetworkManagerUTP)NetworkManager.singleton).relayJoinCode;
        print(((NetworkManagerUTP)NetworkManager.singleton).relayJoinCode);
        if (isServer) {
            pauseButton.gameObject.SetActive(true);
            chooseLevelButton.gameObject.SetActive(true);
            pauseButton.onClick.AddListener(RpcPause);
            chooseLevelButton.onClick.AddListener(ShowLevelMenu);
            startLevelButton.onClick.AddListener(StartLevel);
        }

    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.H)) {
            ShowHelpPanel();
        }
        if (!isServer) return;

        if (Input.GetKeyDown(KeyCode.E)) {
            RpcPause();
        }
        if (Input.GetKeyDown(KeyCode.F)) {
            ShowLevelMenu();
        }
        
    }

    private void ShowHelpPanel() {
        helpPanel.SetActive(!helpPanel.activeSelf);
    }

    private void StartLevel() {
        WaveController.Level = GetLevel();
        WaveController.Mode = GetMode();
        NetworkManager.singleton.ServerChangeScene(SceneManager.GetActiveScene().name);
    }

    private int GetLevel() {
        return levelMenu.GetLevel();
    }

    private LevelMode GetMode() {
        return levelMenu.GetMode();
    }

    private void CopyText() {
        GUIUtility.systemCopyBuffer = ((NetworkManagerUTP)NetworkManager.singleton).relayJoinCode;
    }

    [ClientRpc]
    private void RpcPause() {
        float timeScale = Time.timeScale;
        if (timeScale == 0) {
            timeScale = 1;
            if (isServer) {
                pauseButton.GetComponent<Image>().color = new Color(0, 200 / 256f, 1);
            }
        }
        else {
            timeScale = 0;
            if (isServer) {
                pauseButton.GetComponent<Image>().color = Color.red;
            }
        }
        Time.timeScale = timeScale;
    }

    private void ShowLevelMenu() {
        levelMenu.gameObject.SetActive(!levelMenu.gameObject.activeSelf);

    }

    public void SetPlayer(Player localPlayer) {
        player = localPlayer;

        heartList = new List<RectTransform>();
        playerHealth = player.GetHealth();
        for (int i = 0; i < playerHealth; i++) {
            RectTransform heartPosition = Instantiate(heartImage);
            heartPosition.transform.SetParent(transform, false);
            heartPosition.anchoredPosition = new Vector2(25 + 75 * i, -25);
            heartList.Add(heartPosition);
        }

        player.HealthChanged += UpdateHealthUI;
    }


    private void OnDisable() {
        if (player != null)

            player.HealthChanged -= UpdateHealthUI;
    }

    private void UpdateHealthUI(int health) {
        for (int i = 0; i < heartList.Count; i++) {
            if (i + 1 <= health) {
                heartList[i].GetComponent<Image>().color = Color.red;
            }
            else {
                heartList[i].GetComponent<Image>().color = Color.black;

            }
        }
    }



}
