using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Questo crea un "Singleton", un riferimento globale unico
    public static AudioManager instance;

    [Header("Musica")]
    public AudioSource musicaSottofondo;
    [Header("Effetti Sonori")]
    public AudioSource sfxSource; // Sorgente per i suoni globali
    public AudioClip suonoClick;

    void Awake()
    {
        // Se non c'è ancora un AudioManager nel gioco, questo diventa quello ufficiale
        if (instance == null)
        {
            instance = this;
            // IL SEGRETO: Dice a Unity di NON distruggere questo oggetto quando cambi livello
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Se torni al Main Menu, la scena cercherà di creare un NUOVO AudioManager.
            // Noi lo distruggiamo subito per evitare cloni e far suonare due musiche sovrapposte!
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Carica il volume salvato in precedenza (o 1, cioè 100%, se è la prima volta)
        float volumeSalvato = PlayerPrefs.GetFloat("VolumeGlobale", 1f);
        AudioListener.volume = volumeSalvato;

        // Fa partire la musica se non sta già suonando
        if (!musicaSottofondo.isPlaying)
        {
            musicaSottofondo.Play();
        }
    }

    // Funzione chiamata dallo Slider delle opzioni
    public void CambiaVolumeGlobale(float nuovoVolume)
    {
        AudioListener.volume = nuovoVolume; // Cambia il volume di TUTTO il gioco
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
