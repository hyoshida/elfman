using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerAudio : MonoBehaviour {
    [SerializeField]
    AudioClip _leftStep;

    [SerializeField]
    AudioClip _rightStep;

    [SerializeField]
    AudioClip _jumping;

    [SerializeField]
    AudioClip _landing;

    [SerializeField]
    AudioClip _attacking;

    AudioSource _audioSource;

    // Use this for initialization
    void Start() {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayLeftStep() {
        Play(_leftStep);
    }

    public void PlayRightStep() {
        Play(_rightStep);
    }

    public void PlayJumping() {
        Play(_jumping);
    }

    public void PlayLanding() {
        Play(_landing);
    }

    public void PlayAttacking() {
        Play(_attacking);
    }

    void Play(AudioClip clip) {
        _audioSource.clip = clip;
        _audioSource.Play();
    }
}
