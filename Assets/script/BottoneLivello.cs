using UnityEngine;
using UnityEngine.UI;

public class BottoneLivello : MonoBehaviour
{
    [Header("Impostazioni Livello")]
    public int indiceLivello;

    [Header("UI Stelle")]
    public Image[] immaginiStelle;
    public Color coloreStellaVuota = Color.gray;
    public Color coloreStellaPiena = Color.yellow;

    private Button bottone; // Riferimento al componente cliccabile

    void Awake()
    {
        // Prende in automatico il componente Button attaccato a questo oggetto
        bottone = GetComponent<Button>();
    }

    void OnEnable()
    {
        AggiornaStelleEStato();
    }

    public void AggiornaStelleEStato()
    {
        // Recupera il livello massimo sbloccato (se non esiste, di default è 1)
        int livelloMassimoSbloccato = PlayerPrefs.GetInt("LivelliSbloccati", 1);

        if (indiceLivello <= livelloMassimoSbloccato)
        {
            // --- LIVELLO SBLOCCATO ---
            bottone.interactable = true; // Rende il bottone cliccabile

            int stelleSalvate = PlayerPrefs.GetInt("Livello_" + indiceLivello + "_Stelle", 0);
            
            for (int i = 0; i < immaginiStelle.Length; i++)
            {
                immaginiStelle[i].gameObject.SetActive(true); // Mostra le stelle
                immaginiStelle[i].color = (i < stelleSalvate) ? coloreStellaPiena : coloreStellaVuota;
            }
        }
        else
        {
            // --- LIVELLO BLOCCATO ---
            bottone.interactable = false; // Rende il bottone grigio e non cliccabile
            
            // Nasconde le stelle se il livello è ancora bloccato
            for (int i = 0; i < immaginiStelle.Length; i++)
            {
                immaginiStelle[i].gameObject.SetActive(false);
            }
        }
    }
}