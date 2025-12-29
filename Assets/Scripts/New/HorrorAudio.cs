using UnityEngine;
using UnityEngine.Audio;

public class HorrorAudio : MonoBehaviour
{
    [SerializeField] AudioClip horrorClip;
    [SerializeField] float intervalSeconds = 3.0f;
    [SerializeField] float volume = 1.0f;

    private float timer;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = horrorClip;
        audioSource.volume = volume;
        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake = false;

        audioSource.PlayOneShot(horrorClip);
    }
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= intervalSeconds)
        {
            audioSource.PlayOneShot(horrorClip);
            timer = 0;
        }
    }
}
