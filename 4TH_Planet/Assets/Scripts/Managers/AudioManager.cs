using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private bool _isPlayingMusic = false;

    [Header("Lista de efeitos sonoros")]
    [SerializeField] private List<AudioClip> _listAudioFx;

    [Header("Lista de efeitos músicas")]
    [SerializeField] private List<AudioClip> _listAudioMusic;

    [Header("Audio Source")]
    [SerializeField] private AudioSource _audioSourceMusic;
    
    public void PlaySoundEffect(AudioSource audioSource, int indexFx)
    {
        audioSource.clip = _listAudioFx[indexFx];
        audioSource.Play();
    }

    public void PlayBackgroundMusic(int musicIndex)
    {
        StopBackgroundMusic();
        _audioSourceMusic.clip = _listAudioMusic[musicIndex];
        _audioSourceMusic.loop = true;

        _audioSourceMusic.Play();
        _isPlayingMusic = true;
    }

    private void StopBackgroundMusic()
    {
        if(_isPlayingMusic)
        {
            _audioSourceMusic.Stop();
            _isPlayingMusic = false;
        }
    }

    private void PauseAndUnpauseBackgroundMusic()
    {
        if(_isPlayingMusic)
        {
            _audioSourceMusic.Pause();
            _isPlayingMusic =false;
        }
        else
        {
            _audioSourceMusic.UnPause();
            _isPlayingMusic = true;
        }
        
    }
}
