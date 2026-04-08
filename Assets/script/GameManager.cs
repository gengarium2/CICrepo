using UnityEngine;
using TMPro; // Serve per usare i testi della UI
using UnityEngine.SceneManagement; // Serve per caricare i livelli
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioManagerLivello;
    public AudioClip suonoVittoria;
    [Header("UI Elements")]
    public GameObject panelVittoria;
    public GameObject panelPausa;
    public TextMeshProUGUI testoMosseInGame;
    public TextMeshProUGUI testoMosseFinali;
    [Header("Sistema a Stelle")]
    public int mossePerTreStelle = 5;
    public int mossePerDueStelle = 10;
    public int mossePerStellaExtra = 8;
    [Header("UI Stelle Vittoria")]
    public Image[] stelleVittoria;
    public GameObject[] testiRequisitiStelle;
    public Color coloreStellaVuota = Color.gray;
    public Color coloreStellaPiena = Color.yellow;
    [Header("UI Stella Extra")]
    public GameObject stellaExtraVittoria;
    public string lineaDiDialogoExtra = "perfection achieved!";
    private int mosse = 0;
    private bool inPausa = false;
    private bool giocoFinito = false;

    void Start()
    {
        AggiornaTestoMosse();
    }

    void Update()
    {
        // Se premi ESC (o P) apri/chiudi la pausa
        if (Input.GetKeyDown(KeyCode.Escape) && !giocoFinito)
        {
            TogglePausa();
        }
    }
public void RiavviaLivello()
    {
        // FONDAMENTALE: Riporta il tempo alla normalità prima di ricaricare la scena,
        // altrimenti il gioco ricaricherà il livello ma rimarrà bloccato in pausa (Time.timeScale = 0)!
        Time.timeScale = 1; 
        
        // Ricarica la scena attualmente attiva (cioè il livello in cui ti trovi ora)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
    }
    public void AggiungiMossa()
    {
        if (giocoFinito) return;
        mosse++;
        AggiornaTestoMosse();
    }

    private void AggiornaTestoMosse()
    {
        testoMosseInGame.text = "Moves: " + mosse;
    }

public void Vittoria()
    {
        if (giocoFinito) return;
        if (audioManagerLivello != null && suonoVittoria != null)
        {
            audioManagerLivello.PlayOneShot(suonoVittoria);
        }
        giocoFinito = true;
        panelVittoria.SetActive(true);
        testoMosseFinali.text = "Solved in " + mosse + " moves!";

        int stelleOttenute = 1;
        if (mosse <= mossePerStellaExtra) stelleOttenute = 4;
        else if (mosse <= mossePerTreStelle) stelleOttenute = 3;
        else if (mosse <= mossePerDueStelle) stelleOttenute = 2;

        if (stelleOttenute == 4)
        {
            for (int i = 0; i < stelleVittoria.Length; i++) stelleVittoria[i].gameObject.SetActive(false);
            for (int i = 0; i < testiRequisitiStelle.Length; i++)
            {
                if (testiRequisitiStelle[i] != null) testiRequisitiStelle[i].SetActive(false);
            }

            if (stellaExtraVittoria != null) stellaExtraVittoria.SetActive(true);
        }
        else
        {
            // Accende le classiche e spegne l'arancione
            if (stellaExtraVittoria != null) stellaExtraVittoria.SetActive(false);
            for (int i = 0; i < stelleVittoria.Length; i++)
            {
                stelleVittoria[i].gameObject.SetActive(true);
                stelleVittoria[i].color = (i < stelleOttenute) ? coloreStellaPiena : coloreStellaVuota;
            }

            testoMosseFinali.text = "Solved in " + mosse + " moves!";
        }

        // --- SALVATAGGIO E SBLOCCO ---
        int livelloCorrente = SceneManager.GetActiveScene().buildIndex;
        int recordStelle = PlayerPrefs.GetInt("Livello_" + livelloCorrente + "_Stelle", 0);

        if (stelleOttenute > recordStelle)
        {
            PlayerPrefs.SetInt("Livello_" + livelloCorrente + "_Stelle", stelleOttenute);
        }

        int prossimoLivello = livelloCorrente + 1;
        int livelloMassimoSbloccato = PlayerPrefs.GetInt("LivelliSbloccati", 1);
        if (prossimoLivello > livelloMassimoSbloccato)
        {
            PlayerPrefs.SetInt("LivelliSbloccati", prossimoLivello);
        }

        PlayerPrefs.Save();
    }

    public void TogglePausa()
    {
        inPausa = !inPausa;
        panelPausa.SetActive(inPausa);
        
        // Ferma il tempo se in pausa (così la pallina non si muove)
        Time.timeScale = inPausa ? 0 : 1; 
    }

    public void CaricaProssimoLivello()
    {
        Time.timeScale = 1; // Ripristina il tempo prima di cambiare scena
        // Carica la scena successiva basandosi sull'indice nelle Build Settings
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); 
    }
    public void TornaAlMenuPrincipale()
    {
        // FONDAMENTALE: Riporta il tempo alla normalità prima di cambiare scena, 
        // altrimenti il menu principale rimarrà "congelato" in pausa!
        Time.timeScale = 1; 
        
        // Carica la scena all'indice 0 (il MainMenu)
        SceneManager.LoadScene(0); 
    }
}