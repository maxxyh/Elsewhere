using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAudio : MonoBehaviour
{
    [SerializeField] private AudioClip music;
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.Instance.PlayMusicWithFade(music, 0.5f);
    }

}
