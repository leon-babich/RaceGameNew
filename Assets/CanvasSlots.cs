using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class CanvasSlots : MonoBehaviour
{
    public Sprite imgSoundOn, imgSoundOff;
    public GameObject butSound;

    private AudioSource audioClickBut;

    static public bool IsSound { get; set; }

    string NameSoundSet = "Sound";

    Dictionary<bool, string> soundValues = new Dictionary<bool, string>
    {
        [false] = "Off",
        [true] = "On"
    };

    private void Start()
    {
        audioClickBut = GetComponent<AudioSource>();

        if (PlayerPrefs.HasKey(NameSoundSet)) {
            string isSoundStr = PlayerPrefs.GetString(NameSoundSet);

            foreach(var member in soundValues) {
                if (member.Value == isSoundStr)
                    IsSound = member.Key;
            }
        }

        setSoundSprite();
    }

    public void restartGameClick()
    {
        if(IsSound) audioClickBut.Play();

        GameController.IsLose = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void TurnLeftClick()
    {
        if (IsSound) audioClickBut.Play();
    }

    public void TurnRightClick()
    {
        if (IsSound) audioClickBut.Play();
    }

    public void switchSoundClick()
    {
        IsSound = IsSound ? false : true;
        if (IsSound) audioClickBut.Play();

        PlayerPrefs.SetString(NameSoundSet, soundValues[IsSound]);
        setSoundSprite();
    }

    void setSoundSprite()
    {
        if (IsSound) butSound.GetComponent<Image>().sprite = imgSoundOn;
        else butSound.GetComponent<Image>().sprite = imgSoundOff;
    }
}
