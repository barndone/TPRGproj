using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PartySelectionMenu : MonoBehaviour
{

    //  List of unit IDs for loading in on game start: 
    //  IDs of:
    //      0 -> Warrior
    //      1 -> Cleric
    //      2 -> Rogue
    //      3 -> Ranger

    [SerializeField] private List<int> unitIds = new List<int>();

    [SerializeField] private List<Sprite> portraits = new List<Sprite>();

    [SerializeField] AudioSource buttonSource;
    [SerializeField] AudioClip buttonSound;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip musicIntro;
    [SerializeField] AudioClip musicEnd;

    [SerializeField] GameObject portraitOne;
    [SerializeField] Image portraitOneImage;
    [SerializeField] GameObject portraitTwo;
    [SerializeField] Image portraitTwoImage;
    [SerializeField] GameObject portraitThree;
    [SerializeField] Image portraitThreeImage;

    bool shouldExit = false;

    private void Awake()
    {
        StartCoroutine(PlayMusicIntro(musicIntro));
    }

    //  implementation for our ready button
    public void Ready()
    {
        buttonSource.PlayOneShot(buttonSound);
        SaveSystem.instance.SaveParty(unitIds);
        StartCoroutine(DelayedLoad(musicEnd, 2));
    }

    //  clear the current party
    public void Clear()
    {
        buttonSource.PlayOneShot(buttonSound);
        unitIds.Clear();
    }

    public void AddWarrior()
    {
        buttonSource.PlayOneShot(buttonSound);

        unitIds.Add(0);
        RezieUnitIds();
    }

    public void AddCleric()
    {
        buttonSource.PlayOneShot(buttonSound);

        unitIds.Add(1);
        RezieUnitIds();
    }

    public void AddRogue()
    {
        buttonSource.PlayOneShot(buttonSound);

        unitIds.Add(2);
        RezieUnitIds();
    }

    public void AddRanger()
    {
        buttonSource.PlayOneShot(buttonSound);

        unitIds.Add(3);
        RezieUnitIds();
    }

    
    private void RezieUnitIds()
    {
        //  If the amount of values within unitIds is greater than 3
        if (unitIds.Count > 3)
        {
            //  trim the excess (starting from 3, remove Count - 3 values)
            //  i.e., if there are 4 entries, remove 1 value starting at index of 3
            unitIds.RemoveRange(3, unitIds.Count - 3);
        }
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

    //  Play the intro to the scene track before starting to play the loopable part of the track
    IEnumerator PlayMusicIntro(AudioClip intro)
    {
        audioSource.PlayOneShot(intro);

        yield return new WaitForSecondsRealtime(intro.length);

        audioSource.Play();
    }

    void LateUpdate()
    {
        if (unitIds.Count >= 1)
        {
            portraitOne.SetActive(true);
            portraitOneImage.sprite = portraits[unitIds[0]];
        }
        if (unitIds.Count >= 2)
        {
            portraitTwo.SetActive(true);
            portraitTwoImage.sprite = portraits[unitIds[1]];
        }
        if (unitIds.Count == 3)
        {
            portraitThree.SetActive(true);
            portraitThreeImage.sprite = portraits[unitIds[2]];
        }
        else if (unitIds.Count == 0)
        {
            portraitOne.SetActive(false);
            portraitTwo.SetActive(false);
            portraitThree.SetActive(false);
        }
    }

}
