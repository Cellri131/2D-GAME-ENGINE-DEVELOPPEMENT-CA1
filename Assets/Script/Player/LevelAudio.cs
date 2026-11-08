using UnityEngine;

public class LevelAudio : MonoBehaviour
{
    [SerializeField] private AudioClip ambientSound;
    [SerializeField] private AudioClip pursuitSound;
    [SerializeField] private AudioClip collectibleSound;
    [SerializeField] private AudioClip catFightSound;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private Enemie catFight;
    [SerializeField, Min(0f)] private float proximityDistance = 5f;
    [SerializeField, Range(0f, 1f)] private float musicVolume = 0.5f;
    [SerializeField, Range(0f, 1f)] private float effectsVolume = 1f;

    private AudioSource ambientSource;
    private AudioSource pursuitSource;
    private AudioSource proximitySource;
    private AudioSource effectsSource;

    private void Awake()
    {
        ambientSource = CreateSource(ambientSound, true, musicVolume);
        pursuitSource = CreateSource(pursuitSound, true, musicVolume);
        proximitySource = CreateSource(catFightSound, true, effectsVolume);
        effectsSource = CreateSource(null, false, effectsVolume);

        if (ambientSound == null || pursuitSound == null || collectibleSound == null ||
            catFightSound == null || hitSound == null || catFight == null)
            Debug.LogError("Assigne tous les sons et l'ennemi CatFight au composant LevelAudio.", this);
    }

    private void Start()
    {
        if (ambientSound != null)
            ambientSource.Play();
    }

    private void Update()
    {
        if (catFight == null)
            return;

        if (catFight.IsActivated && pursuitSound != null && !pursuitSource.isPlaying)
            pursuitSource.Play();

        bool isNearby = catFight.isActiveAndEnabled &&
            (catFight.transform.position - transform.position).sqrMagnitude <= proximityDistance * proximityDistance;
        if (isNearby && catFightSound != null)
        {
            if (!proximitySource.isPlaying)
                proximitySource.Play();
        }
        else if (proximitySource.isPlaying)
            proximitySource.Stop();
    }

    public void PlayCollectible()
    {
        if (collectibleSound != null)
            effectsSource.PlayOneShot(collectibleSound);
    }

    public void PlayHit(bool changingScene)
    {
        if (hitSound == null)
            return;

        if (!changingScene)
        {
            effectsSource.PlayOneShot(hitSound);
            return;
        }

        GameObject soundObject = new GameObject("HitSound");
        DontDestroyOnLoad(soundObject);
        AudioSource source = soundObject.AddComponent<AudioSource>();
        source.spatialBlend = 0f;
        source.volume = effectsVolume;
        source.clip = hitSound;
        source.Play();
        Destroy(soundObject, hitSound.length);
    }

    private AudioSource CreateSource(AudioClip clip, bool loop, float volume)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;
        source.clip = clip;
        source.loop = loop;
        source.volume = volume;
        return source;
    }
}
