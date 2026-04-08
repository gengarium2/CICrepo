using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [Header("Musica")]
    public AudioSource musicaSottofondo;
    [Header("Effetti Sonori")]
    public AudioSource sfxSource;
    public AudioClip suonoClick;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        float volumeSalvato = PlayerPrefs.GetFloat("VolumeGlobale", 1f);
        AudioListener.volume = volumeSalvato;
        if (!musicaSottofondo.isPlaying)
        {
            musicaSottofondo.Play();
        }
    }
    public void CambiaVolumeGlobale(float nuovoVolume)
    {
        AudioListener.volume = nuovoVolume;
        PlayerPrefs.SetFloat("VolumeGlobale", nuovoVolume);
        PlayerPrefs.Save();
    }
    public void RiproduciClick()
    {
        if (sfxSource != null && suonoClick != null)
        {
            sfxSource.PlayOneShot(suonoClick);
        }
    }
}
