using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Pannelli UI")]
    public GameObject panelPrincipale;
    public GameObject panelLivelli;
    public GameObject panelOpzioni;

    [Header("Opzioni")]
    public Toggle toggleFacile;
    public Toggle toggleNotturna;
    public Slider sliderVolume;

    void Start()
    {
        panelPrincipale.SetActive(true);
        panelLivelli.SetActive(false);
        panelOpzioni.SetActive(false);

        // Gestione Toggle Facile
        if (toggleFacile != null)
        {
            bool isFacile = PlayerPrefs.GetInt("ModalitaFacile", 0) == 1;
            toggleFacile.SetIsOnWithoutNotify(isFacile);
            toggleFacile.onValueChanged.AddListener(ImpostaModalitaFacile);
        }

        // --- NUOVO: Gestione Toggle Modalità Notturna ---
        if (toggleNotturna != null)
        {
            // Di default impostiamo a 1 (vero), così la Skybox 1 (Nera) è quella base
            bool isNotturna = PlayerPrefs.GetInt("ModalitaNotturna", 1) == 1;
            toggleNotturna.SetIsOnWithoutNotify(isNotturna);
            toggleNotturna.onValueChanged.AddListener(ImpostaModalitaNotturna);
        }
        if (sliderVolume != null)
        {
            // Legge il volume salvato (o 100% di default)
            float volumeSalvato = PlayerPrefs.GetFloat("VolumeGlobale", 1f);

            // Imposta la grafica dello slider senza lanciare salvataggi a vuoto
            sliderVolume.SetValueWithoutNotify(volumeSalvato);

            // Collega lo scorrimento dello slider alla nostra funzione
            sliderVolume.onValueChanged.AddListener(ImpostaVolume);
        }
    }

    // (Le tue altre funzioni rimangono uguali...)

    // --- NUOVA FUNZIONE ---
    public void ImpostaModalitaNotturna(bool isAttiva)
    {
        PlayerPrefs.SetInt("ModalitaNotturna", isAttiva ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log("Modalità Notturna impostata su: " + isAttiva);
    }

    // --- MENU PRINCIPALE ---
    public void NuovoGioco()
    {
        SceneManager.LoadScene(1);
    }

    public void ApriSelezioneLivelli()
    {
        panelPrincipale.SetActive(false);
        panelLivelli.SetActive(true);
    }

    public void Esci()
    {
        Debug.Log("Uscita dal gioco!");
        Application.Quit();
    }

    // --- SELEZIONE LIVELLI ---
    public void TornaIndietroDaLivelli()
    {
        panelLivelli.SetActive(false);
        panelPrincipale.SetActive(true);
    }

    public void CaricaLivello(int indiceLivello)
    {
        SceneManager.LoadScene(indiceLivello);
    }

    // --- OPZIONI ---
    public void ApriOpzioni()
    {
        panelPrincipale.SetActive(false);
        panelOpzioni.SetActive(true);
    }

    public void TornaIndietroDaOpzioni()
    {
        panelOpzioni.SetActive(false);
        panelPrincipale.SetActive(true);
    }

    public void ResettaSalvataggi()
    {
        // Cancella TUTTI i dati salvati (stelle dei livelli e impostazioni)
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // Rimette a posto il toggle della UI spegnendolo (visto che abbiamo resettato tutto)
        if (toggleFacile != null)
        {
            toggleFacile.isOn = false;
        }

        Debug.Log("Tutti i salvataggi sono stati resettati!");
    }

    // Questa funzione verrà chiamata automaticamente quando clicchi sul Toggle
    public void ImpostaModalitaFacile(bool isAttiva)
    {
        // Se isAttiva è true salviamo 1, altrimenti salviamo 0
        PlayerPrefs.SetInt("ModalitaFacile", isAttiva ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log("Modalità Facile impostata su: " + isAttiva);
    }
    public void ImpostaVolume(float valore)
    {
        // Passa il valore all'AudioManager
        if (AudioManager.instance != null)
        {
            AudioManager.instance.CambiaVolumeGlobale(valore);
        }
        else
        {
            // Se per caso l'AudioManager non fosse pronto, lo forza di brutto
            AudioListener.volume = valore;
            PlayerPrefs.SetFloat("VolumeGlobale", valore);
            PlayerPrefs.Save();
        }
    }
}