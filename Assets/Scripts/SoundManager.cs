using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip[] metalTonks;
    public AudioClip[] woodHits;
    public AudioClip[] squeakyToyHits;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayMetalTonk()
    {
        PlayRandomClip(metalTonks);
    }

    public void PlayWoodHit()
    {
        PlayRandomClip(woodHits);
    }

    public void PlaySqueakyToyHit()
    {
        PlayRandomClip(squeakyToyHits);
    }

    private void PlayRandomClip(AudioClip[] clips)
    {
        if (audioSource != null && clips.Length > 0)
        {
            int randomIndex = Random.Range(0, clips.Length);
            audioSource.PlayOneShot(clips[randomIndex]);
        }
    }
}
