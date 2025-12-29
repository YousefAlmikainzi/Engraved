using UnityEngine;
public class WolfAudio : MonoBehaviour
{
    [SerializeField] AudioClip wolfClip;
    [SerializeField] float intervalSeconds = 3.0f;
    [SerializeField] float volume = 1.0f;
    private float timer;
    private AudioSource audioSource;
    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = wolfClip;
        audioSource.volume = volume;
        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake = false;
    }
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= intervalSeconds)
        {
            audioSource.PlayOneShot(wolfClip);
            timer = 0;
        }
    }
}