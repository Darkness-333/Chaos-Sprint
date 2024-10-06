using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GameLevelMenuUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI gameModeText;
    [SerializeField] private Button closeButton;

    private int level=1;
    private LevelMode mode=LevelMode.Simple;

    private void Start() {
        closeButton.onClick.AddListener(CloseLevelMenu);
    }

    private void CloseLevelMenu() {
        gameObject.SetActive(false);
    }

    public int GetLevel() {
        return level;
    }

    public LevelMode GetMode() {
        return mode;
    }

    public void ChangeLevel(LevelButton levelButton) {
        level = levelButton.level;
        mode = levelButton.mode;
        levelText.SetText("Выбранный уровень: "+level.ToString());
        if(levelButton.mode == LevelMode.Infinitive) {
            gameModeText.SetText("Выбранный режим: Бесконечный");
        }
        else {
            gameModeText.SetText("Выбранный режим: Обычный");
        }
    }

}
