using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    [SerializeField] AudioSource buttonSource;
    [SerializeField] AudioClip buttonSound;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip menuMusic;
    [SerializeField] AudioClip menuMusicEnd;

    private bool shouldExit = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();    
    }

    public void Play()
    {
        buttonSource.PlayOneShot(buttonSound);
        StartCoroutine(DelayedLoad(menuMusicEnd, 1));
    }

    public void Quit()
    {
        buttonSource.PlayOneShot(buttonSound);
        StartCoroutine(DelayedQuit(menuMusicEnd));
    }

    //Delayed load for a scene with parameters for an audio clip, and a scene index
    IEnumerator DelayedLoad(AudioClip clip, int sceneIndex)
    {
        if (!shouldExit)
        {

            shouldExit = true;

            audioSource.Stop();
            audioSource.PlayOneShot(clip);

            yield return new WaitForSecondsRealtime(clip.length);

            shouldExit = false;
            SceneManager.LoadScene(sceneIndex);
            

        }
    }

    IEnumerator DelayedQuit(AudioClip clip)
    {
        if (!shouldExit)
        {
            shouldExit = true;

            audioSource.Stop();
            audioSource.PlayOneShot(clip);

            yield return new WaitForSecondsRealtime(clip.length);

            shouldExit = false;
            Application.Quit();
        }
    }
}
