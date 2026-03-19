using UnityEngine;
using TMPro; // Serve per usare i testi della UI
using UnityEngine.SceneManagement; // Serve per caricare i livelli
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject panelVittoria;
    public GameObject panelPausa;
    public TextMeshProUGUI testoMosseInGame;
    public TextMeshProUGUI testoMosseFinali;
    [Header("Sistema a Stelle")]
    public int mossePerTreStelle = 5;
    public int mossePerDueStelle = 10;
    [Header("UI Stelle Vittoria")]
    public Image[] stelleVittoria; // Qui trascinerai le 3 immagini dal PanelVittoria
    public Color coloreStellaVuota = Color.gray;
    public Color coloreStellaPiena = Color.yellow;

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
        testoMosseInGame.text = "Mosse: " + mosse;
    }

public void Vittoria()
    {
        if (giocoFinito) return;
        
        giocoFinito = true;
        panelVittoria.SetActive(true);
        testoMosseFinali.text = "Completato in " + mosse + " mosse!";

        int stelleOttenute = 1;
        if (mosse <= mossePerTreStelle) stelleOttenute = 3;
        else if (mosse <= mossePerDueStelle) stelleOttenute = 2;

        for (int i = 0; i < stelleVittoria.Length; i++)
        {
            stelleVittoria[i].color = (i < stelleOttenute) ? coloreStellaPiena : coloreStellaVuota;
        }

        int livelloCorrente = SceneManager.GetActiveScene().buildIndex;
        int recordStelle = PlayerPrefs.GetInt("Livello_" + livelloCorrente + "_Stelle", 0);

        if (stelleOttenute > recordStelle)
        {
            PlayerPrefs.SetInt("Livello_" + livelloCorrente + "_Stelle", stelleOttenute);
        }

        // --- NUOVO PEZZO: SBLOCCO DEL LIVELLO SUCCESSIVO ---
        int prossimoLivello = livelloCorrente + 1;
        int livelloMassimoSbloccato = PlayerPrefs.GetInt("LivelliSbloccati", 1);

        // Se il prossimo livello è maggiore di quello che avevamo già sbloccato in passato, aggiorniamo il record!
        if (prossimoLivello > livelloMassimoSbloccato)
        {
            PlayerPrefs.SetInt("LivelliSbloccati", prossimoLivello);
            Debug.Log("Sbloccato il livello: " + prossimoLivello);
        }
        // ----------------------------------------------------

        PlayerPrefs.Save(); // Salva sia le stelle che lo sblocco in un colpo solo
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