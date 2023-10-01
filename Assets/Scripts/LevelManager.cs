using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public AudioClip themeMusicClip;
    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupMusicPlayer();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void SetupMusicPlayer()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = themeMusicClip;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void LoadLevelByName(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    public void ReloadLevel()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
