using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundHandler : MonoBehaviour
{
    private AudioManager _audioManager;
    private void Awake()
    {
        _audioManager = FindObjectOfType<AudioManager>();
    }

    public void Play(string name)
    {
        if (!_audioManager)
            return;
        _audioManager.Play(name);
    }

}
