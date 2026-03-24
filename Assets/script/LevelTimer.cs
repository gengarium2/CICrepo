using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // Aggiunto per poter usare TextMeshPro!

public class LevelTimer : MonoBehaviour
{
    [Header("Audio Timer")]
    public AudioSource audioTimer; // Metteremo qui la clip del Tic-Tac in loop!
    public AudioClip suonoSconfitta;

    [Header("Impostazioni Timer")]
    public float tempoNormale = 30f;
    public float tempoFacile = 45f;
    
    [Header("UI e Riferimenti")]
    public Image barraTempo; 
    public TextMeshProUGUI testoContatore; // NUOVO: Il testo dei secondi
    public Image aloneRosso; // NUOVO: L'alone che lampeggerà
    public float velocitaLampeggio = 2f; // Velocità del fade in/out

    public GameObject panelSconfitta; 
    public GameObject panelVittoria; 
    public BallController ballController; 

    private float tempoRimanente;
    private float tempoTotale;
    private bool timerAttivo = true;

    void Start()
    {
        bool isFacile = PlayerPrefs.GetInt("ModalitaFacile", 0) == 1;
        
        tempoTotale = isFacile ? tempoFacile : tempoNormale;
        tempoRimanente = tempoTotale;
        
        if (panelSconfitta != null) panelSconfitta.SetActive(false);

        // Assicuriamoci che l'alone rosso sia completamente invisibile all'inizio (Alpha = 0)
        if (aloneRosso != null)
        {
            Color coloreIniziale = aloneRosso.color;
            coloreIniziale.a = 0f;
            aloneRosso.color = coloreIniziale;
        }
    }

    void Update()
    {
        if (!timerAttivo || (panelVittoria != null && panelVittoria.activeSelf))
        {
            if (audioTimer != null && audioTimer.isPlaying) audioTimer.Stop();
            return;
        }

        tempoRimanente -= Time.deltaTime;

        if (barraTempo != null)
        {
            barraTempo.fillAmount = tempoRimanente / tempoTotale;
        }

        // --- 1. AGGIORNA IL TESTO DEI SECONDI ---
        if (testoContatore != null)
        {
            // Usiamo CeilToInt così arrotonda per eccesso (es. 0.5 secondi mostra "1", non "0")
            int secondiRimasti = Mathf.CeilToInt(Mathf.Max(0, tempoRimanente));
            testoContatore.text = secondiRimasti.ToString();
        }

        // --- 2. GESTISCI L'ALONE ROSSO LAMPEGGIANTE ---
        if (tempoRimanente <= (tempoTotale * 0.25f) && tempoRimanente > 0)
        {
            if (aloneRosso != null)
            {
                float alphaLampeggio = Mathf.PingPong(Time.time * velocitaLampeggio, 1f);
                Color c = aloneRosso.color; c.a = alphaLampeggio; aloneRosso.color = c;
            }

            // Fa partire il Tic-Tac se non sta già suonando
            if (audioTimer != null && !audioTimer.isPlaying)
            {
                audioTimer.Play(); // Usa Play() normale, non OneShot, perché è in Loop
            }
        }

        // Se il tempo scade
        if (tempoRimanente <= 0)
        {
            tempoRimanente = 0;
            if (aloneRosso != null) { Color c = aloneRosso.color; c.a = 0f; aloneRosso.color = c; }

            Sconfitta();
        }
    }

    private void Sconfitta()
    {
        timerAttivo = false;

        // Ferma il Tic-Tac e suona la sconfitta!
        if (audioTimer != null)
        {
            audioTimer.Stop();
            if (suonoSconfitta != null) audioTimer.PlayOneShot(suonoSconfitta);
        }

        if (panelSconfitta != null) panelSconfitta.SetActive(true);
        if (ballController != null) ballController.enabled = false;
    }

    public void RiavviaLivello()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void TornaAlMenuPrincipale()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}