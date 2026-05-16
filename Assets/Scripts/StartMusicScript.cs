using UnityEngine;

public class StartMusicScript : MonoBehaviour
{
    private AudioClip BackgroundMusic;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource.isPlaying) return;
        else
        {
            audioSource.Play();
            DontDestroyOnLoad(audioSource);
        }
            
    }
}
