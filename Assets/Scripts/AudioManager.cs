using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class AudioManager : MonoBehaviour
{
    public static AudioManager instance = null;

    public bool mute = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this);
    }

    private void Start()
    {

    }

    public void Mute()
    {
        mute = !mute;
        if (mute)
            AudioListener.volume = 0;
        else
            AudioListener.volume = 1;
    }
}
