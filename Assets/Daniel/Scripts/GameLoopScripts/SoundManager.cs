using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPoolManager : MonoBehaviour, IProcess
{
    [System.Serializable]
    public class SoundPool
    {
        public Queue<AudioSource> soundSources = new Queue<AudioSource>();
    }

    public List<AudioClip> audioClips; // Lista de AudioClips
    [SerializeField] private int poolSize = 10;

    private Dictionary<string, SoundPool> poolDictionary = new Dictionary<string, SoundPool>();
    private Dictionary<string, AudioClip> clipDictionary = new Dictionary<string, AudioClip>();

    public static SoundPoolManager Instance { get; private set; }
    public bool IsCompleted { get; private set; } = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void ExecuteProcess(System.Action onComplete)
    {
        StartCoroutine(InitializePool(onComplete));
    }

    private IEnumerator InitializePool(System.Action onComplete)
    {
        foreach (var clip in audioClips)
        {
            string clipName = clip.name;
            clipDictionary[clipName] = clip;
            SoundPool pool = new SoundPool();

            for (int i = 0; i < poolSize; i++)
            {
                AudioSource audioSource = CreateAudioSource(clip);
                pool.soundSources.Enqueue(audioSource);
                yield return null;
            }

            poolDictionary[clipName] = pool;
        }

        IsCompleted = true;
        onComplete?.Invoke();
    }

    private AudioSource CreateAudioSource(AudioClip clip)
    {
        GameObject soundObject = new GameObject($"Sound_{clip.name}");
        //soundObject.transform.SetParent(transform);
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();

        // Configuración del AudioSource
        audioSource.clip = clip;
        audioSource.playOnAwake = true;
        audioSource.spatialBlend = 1.0f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.minDistance = 1f;
        audioSource.maxDistance = 70f;
        audioSource.gameObject.SetActive(false);

        return audioSource;
    }

    public AudioSource PlaySound(string soundName, GameObject parentObject)
    {
        if (!poolDictionary.ContainsKey(soundName))
        {
            Debug.LogError($"No se encontró el sonido: {soundName}");
            return null;
        }

        SoundPool pool = poolDictionary[soundName];

        if (pool.soundSources.Count == 0)
        {
            Debug.LogWarning($"No hay fuentes disponibles para {soundName}. Creando una nueva...");
            pool.soundSources.Enqueue(CreateAudioSource(clipDictionary[soundName]));
        }

        AudioSource audioSource = pool.soundSources.Dequeue();
        audioSource.transform.SetParent(parentObject.transform);
        audioSource.transform.localPosition = Vector3.zero;
        audioSource.gameObject.SetActive(true);
        audioSource.enabled = true;

        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        audioSource.Play();

        return audioSource;
    }


    public void ReturnToPool(string soundName, AudioSource audioSource)
    {
        if (poolDictionary.ContainsKey(soundName))
        {
            audioSource.Stop();
            audioSource.gameObject.SetActive(false);
            audioSource.transform.SetParent(transform);
            poolDictionary[soundName].soundSources.Enqueue(audioSource);
        }
        else
        {
            Debug.LogError($"El sonido {soundName} no está en la pool.");
        }
    }

}
