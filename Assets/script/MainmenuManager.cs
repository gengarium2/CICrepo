using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Pannelli UI")]
    public GameObject panelPrincipale;
    public GameObject panelLivelli;
    public GameObject panelOpzioni;
    public GameObject panelCredits;

    [Header("Opzioni")]
    public Toggle toggleFacile;
    public Toggle toggleNotturna;
    public Slider sliderVolume;

    void Start()
    {
        panelPrincipale.SetActive(true);
        panelLivelli.SetActive(false);
        panelOpzioni.SetActive(false);
        panelCredits.SetActive(false);

        if (toggleFacile != null)
        {
            bool isFacile = PlayerPrefs.GetInt("ModalitaFacile", 0) == 1;
            toggleFacile.SetIsOnWithoutNotify(isFacile);
            toggleFacile.onValueChanged.AddListener(ImpostaModalitaFacile);
        }

        if (toggleNotturna != null)
        {
            bool isNotturna = PlayerPrefs.GetInt("ModalitaNotturna", 1) == 1;
            toggleNotturna.SetIsOnWithoutNotify(isNotturna);
            toggleNotturna.onValueChanged.AddListener(ImpostaModalitaNotturna);
        }
        if (sliderVolume != null)
        {
            float volumeSalvato = PlayerPrefs.GetFloat("VolumeGlobale", 1f);
            sliderVolume.SetValueWithoutNotify(volumeSalvato);
            sliderVolume.onValueChanged.AddListener(ImpostaVolume);
        }
    }
    public void ImpostaModalitaNotturna(bool isAttiva)
    {
        PlayerPrefs.SetInt("ModalitaNotturna", isAttiva ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log("Modalità Notturna impostata su: " + isAttiva);
    }
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
    public void TornaIndietroDaLivelli()
    {
        panelLivelli.SetActive(false);
        panelPrincipale.SetActive(true);
    }
    public void ApriCredits()
    {
        panelPrincipale.SetActive(false);
        panelCredits.SetActive(true);

        // Se hai l'AudioManager, facciamo fare "click!"
        if (AudioManager.instance != null) AudioManager.instance.RiproduciClick();
    }
    public void ChiudiCredits()
    {
        panelCredits.SetActive(false);
        panelPrincipale.SetActive(true);

        if (AudioManager.instance != null) AudioManager.instance.RiproduciClick();
    }
    public void CaricaLivello(int indiceLivello)
    {
        SceneManager.LoadScene(indiceLivello);
    }
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
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        if (toggleFacile != null)
        {
            toggleFacile.isOn = false;
        }

        Debug.Log("Tutti i salvataggi sono stati resettati!");
    }

    public void ImpostaModalitaFacile(bool isAttiva)
    {
        PlayerPrefs.SetInt("ModalitaFacile", isAttiva ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log("Modalità Facile impostata su: " + isAttiva);
    }
    public void ApriLink(string url)
    {
        Application.OpenURL(url);
        Debug.Log("Sto aprendo il link: " + url);
    }
    public void ImpostaVolume(float valore)
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.CambiaVolumeGlobale(valore);
        }
        else
        {
            AudioListener.volume = valore;
            PlayerPrefs.SetFloat("VolumeGlobale", valore);
            PlayerPrefs.Save();
        }
    }
}