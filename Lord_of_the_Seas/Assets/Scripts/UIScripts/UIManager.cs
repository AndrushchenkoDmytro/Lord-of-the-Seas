using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }
    [SerializeField] private AudioClip LoseGame;
    [SerializeField] private AudioClip WinGame;

    private GameObject pausePanel;
    private Animator animator;
    bool isPause = false;
    bool canPress = true;

    [SerializeField] TextMeshProUGUI moneyValueText;
    [SerializeField] TextMeshProUGUI moneyPerSecondValueText;
    [SerializeField] TextMeshProUGUI spendMoneyValueText;
    private PlayerController playerController;

    private void Awake()
    {
        playerController = GameObject.Find("/Player/PlayerController").GetComponent<PlayerController>();
        pausePanel = GameObject.Find("PlayerUI/PausePanel");
        pausePanel.SetActive(false);
        animator = GetComponent<Animator>();
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        int money = (int)playerController.GetMoney();
        moneyValueText.text = money.ToString();

        int moneyPerSecond = (int)playerController.GetMoneyPerSecond();
        moneyPerSecondValueText.text = moneyPerSecond.ToString();
    }

    public void SpendMoney(int value)
    {
        spendMoneyValueText.text = (-value).ToString();
        animator.Play("SpendMoney");
    }

    public void ExitToMap()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void NextLevel()
    {
        Time.timeScale = 1;
        int index = SceneManager.GetActiveScene().buildIndex;
        if (index < 9)
        {
            SceneManager.LoadScene("Level" + (index+1).ToString());
        }
    }

    public void ControllGamePause()
    {
        if(canPress == true)
        {
            pausePanel.SetActive(true);
            AudioManager.instance.SetMusicVolume(0.35f);
            canPress = false;
            StartCoroutine(ÑheckIsGamePause());
        }
    }

    IEnumerator ÑheckIsGamePause()
    {
        if (isPause == false)
        {
            Time.timeScale = 0;
            animator.Play("PauseGame");
            yield return new WaitForSecondsRealtime(1);
            isPause = true;
            canPress = true;
        }
        else
        {
            animator.Play("ContinueGame");
            yield return new WaitForSecondsRealtime(1);
            isPause = false;
            canPress = true;
            Time.timeScale = 1;
            pausePanel.SetActive(false);
            AudioManager.instance.SetMusicVolume(0.85f);
        }
    }

    public void ShowWinPanel()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        if (PlayerPrefs.GetInt("Level" + (index + 1).ToString()) == 0)
        {
            PlayerPrefs.SetInt("Level" + (index + 1).ToString(),1);
        }
        playerController.StopPlayerController();
        AudioManager.instance.SetMusicVolume(0.35f);
        AudioManager.instance.PlayEffect(WinGame);
        animator.Play("WinGame");
        StartCoroutine(Wait());
    }

    public void ShowLosePanel()
    {
        playerController.StopPlayerController();
        AudioManager.instance.SetMusicVolume(0.35f);
        AudioManager.instance.PlayEffect(LoseGame);
        animator.Play("LoseGame");
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        yield return new WaitForFixedUpdate();
        Time.timeScale = 0;
    }

}
