using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Television : MonoBehaviour
{
    [Header("Videos disponibles")]
    public VideoClip[] videoClips;

    [Header("Componentes necesarios")]
    public VideoPlayer videoPlayer;
    public AudioSource audioSource;

    void Start()
    {
        if (videoClips.Length == 0 || videoPlayer == null || audioSource == null)
        {
            Debug.LogWarning("Faltan componentes o videos.");
            return;
        }

        // Elegir un video aleatorio
        int index = Random.Range(0, videoClips.Length);
        VideoClip selectedVideo = videoClips[index];

        // Configurar el VideoPlayer para usar AudioSource
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.SetTargetAudioSource(0, audioSource);

        // Asignar el video y reproducir
        videoPlayer.clip = selectedVideo;
        videoPlayer.Play();
    }
}
