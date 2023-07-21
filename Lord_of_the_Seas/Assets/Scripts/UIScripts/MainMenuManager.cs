using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] Transform levels;
    [SerializeField] Animator mainMenuAnimator;
    [SerializeField] Animator MapAnimator;

    [SerializeField] Image musicImage;
    [SerializeField] Image effectImage;

    [SerializeField] Sprite[] icons;
    [SerializeField] AudioClip audioClip;

    public void Awake()
    {
        Transform temp;
        for (int i = 1; i < levels.childCount; i++)
        {
            temp = levels.transform.GetChild(i);
            if (PlayerPrefs.GetInt("Level" + (i + 1).ToString()) == 0)
            {
                temp.GetChild(0).gameObject.SetActive(false);
                temp.GetComponent<Button>().enabled = false;
            }
            else
            {
                temp.GetChild(1).gameObject.SetActive(false);
            }
        }
    }

    public void OffEffects()
    {
        AudioManager.instance.PlayEffect(audioClip);
        if (AudioManager.instance.MuteEffectSource() == false)
        {
            effectImage.sprite = icons[3];
        }
        else
        {
            effectImage.sprite = icons[2];
        }
    }

    public void OffMusic()
    {
        AudioManager.instance.PlayEffect(audioClip);
        if (AudioManager.instance.MuteMusicSource() == false)
        {
            musicImage.sprite = icons[1];
        }
        else
        {
            musicImage.sprite = icons[0];
        }
    }

    public void MoveToMap()
    {
        AudioManager.instance.PlayEffect(audioClip);
        mainMenuAnimator.Play("ToMap");
        MapAnimator.Play("Map");
    }

    public void MoveToMainMenu()
    {
        AudioManager.instance.PlayEffect(audioClip);
        mainMenuAnimator.Play("ToMainMenu");
        MapAnimator.Play("MainMenu");
    }

    public void ExitFromGame()
    {
        AudioManager.instance.PlayEffect(audioClip);
        Application.Quit();
    }

    public void OpenLevel_1()
    {
        SceneManager.LoadScene("Level1");
    }

    public void OpenLevel_2()
    {
        SceneManager.LoadScene("Level2");
    }

    public void OpenLevel_3()
    {
        SceneManager.LoadScene("Level3");
    }

    public void OpenLevel_4()
    {
        SceneManager.LoadScene("Level4");
    }

    public void OpenLevel_5()
    {
        SceneManager.LoadScene("Level5");
    }

    public void OpenLevel_6()
    {
        SceneManager.LoadScene("Level6");
    }

    public void OpenLevel_7()
    {
        SceneManager.LoadScene("Level7");
    }

    public void OpenLevel_8()
    {
        SceneManager.LoadScene("Level8");
    }

    public void OpenLevel_9()
    {
        SceneManager.LoadScene("Level9");
    }

}
