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

    [Header("UI Stella Extra")]
    public GameObject stellaExtraMenu; // NUOVO: La stella gigante arancione sul bottone

    private Button bottone;

    void Awake()
    {
        bottone = GetComponent<Button>();
    }

    void OnEnable()
    {
        AggiornaStelleEStato();
    }

    public void AggiornaStelleEStato()
    {
        int livelloMassimoSbloccato = PlayerPrefs.GetInt("LivelliSbloccati", 1);

        if (indiceLivello <= livelloMassimoSbloccato)
        {
            // --- LIVELLO SBLOCCATO ---
            bottone.interactable = true;
            int stelleSalvate = PlayerPrefs.GetInt("Livello_" + indiceLivello + "_Stelle", 0);

            if (stelleSalvate == 4)
            {
                // Mostra la stella arancione e nasconde le altre
                if (stellaExtraMenu != null) stellaExtraMenu.SetActive(true);
                for (int i = 0; i < immaginiStelle.Length; i++) immaginiStelle[i].gameObject.SetActive(false);
            }
            else
            {
                // Mostra le classiche e nasconde quella arancione
                if (stellaExtraMenu != null) stellaExtraMenu.SetActive(false);
                for (int i = 0; i < immaginiStelle.Length; i++)
                {
                    immaginiStelle[i].gameObject.SetActive(true);
                    immaginiStelle[i].color = (i < stelleSalvate) ? coloreStellaPiena : coloreStellaVuota;
                }
            }
        }
        else
        {
            // --- LIVELLO BLOCCATO ---
            bottone.interactable = false;
            if (stellaExtraMenu != null) stellaExtraMenu.SetActive(false);
            for (int i = 0; i < immaginiStelle.Length; i++) immaginiStelle[i].gameObject.SetActive(false);
        }
    }
}