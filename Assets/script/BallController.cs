using UnityEngine;
using System.Collections;

// Questo assicura che ci sia sempre un Rigidbody sulla pallina
[RequireComponent(typeof(Rigidbody))] 
public class BallController : MonoBehaviour
{
    [Header("Impostazioni di Movimento")]
    public float speed = 15f; // Velocità di scivolamento
    private bool isMoving = false; // Controlla se la pallina è già in viaggio
    [Header("Audio Pallina")]
    public AudioSource audioPallina;
    public AudioClip suonoStop; // Quando sbatte al muro
    public AudioClip suonoPad;
    [Header("Riferimenti")]
    public GameManager gameManager; // Trascina qui il GameManager dall'Inspector

    private Rigidbody rb;
    private DirectionalPad Dp;
    private float ballRadius;
    private Coroutine movementCoroutine;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // Assicuriamoci che sia kinematico da script

        // Calcoliamo il raggio esatto della pallina (prendendo lo SphereCollider)
        SphereCollider col = GetComponent<SphereCollider>();
        if (col != null) {
            // Moltiplichiamo per la scala nel caso la pallina sia stata ingrandita/rimpicciolita
            ballRadius = col.radius * transform.localScale.x; 
        } else {
            ballRadius = transform.localScale.x / 2f;
        }
    }

    void Update()
    {
        // Se si sta già muovendo, ignora i nuovi input
        if (isMoving) return;

        // --- CONTROLLI (Solo GetKeyDown per evitare che tenendo premuto spammi input) ---
        // Avanti e Indietro (Asse Z)
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) TryMove(Vector3.forward);
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) TryMove(Vector3.back);
        
        // Destra e Sinistra (Asse X)
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) TryMove(Vector3.right);
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) TryMove(Vector3.left);
        // Alto e Basso (Asse Y)
        else if (Input.GetKeyDown(KeyCode.Space)) TryMove(Vector3.up);
        else if (Input.GetKeyDown(KeyCode.LeftShift)) TryMove(Vector3.down);
    }

    void TryMove(Vector3 direction)
    {
        if (isMoving) return;
        RaycastHit hit;
        
        // Lanciamo il raggio dal centro della pallina
        if (Physics.Raycast(transform.position, direction, out hit, Mathf.Infinity))
        {
            // CONTROLLO FONDAMENTALE: Se la distanza dal muro è minore o uguale al raggio della pallina 
            // (più un minuscolo margine d'errore di 0.05f), significa che SIAMO GIA' ATTACCATI AL MURO.
            if (hit.distance <= ballRadius + 0.05f)
            {
                return; // Interrompiamo tutto. Niente movimento e NIENTE mossa aggiunta!
            }

            // Calcoliamo la destinazione usando la distanza netta meno il raggio
            // Questo evita il glitch di entrare dentro il muro
            Vector3 targetPosition = transform.position + direction * (hit.distance - ballRadius);

            // Dato che ci stiamo effettivamente muovendo, aggiungiamo la mossa
            if (gameManager != null)
            {
                gameManager.AggiungiMossa();
            }

            // Avvia lo scivolamento
        movementCoroutine = StartCoroutine(SlideToPosition(targetPosition));
        }
    }

    IEnumerator SlideToPosition(Vector3 target)
    {
        isMoving = true; // Blocca gli input

        // Finché non siamo vicinissimi al target...
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            // Calcoliamo il prossimo passo
            Vector3 newPosition = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            
            // Usiamo MovePosition del Rigidbody. E' il modo corretto per muovere
            // oggetti cinematici senza "rompere" la fisica di Unity e compenetrare i muri.
            rb.MovePosition(newPosition);

            // Aspettiamo il prossimo frame FISICO (non grafico), indispensabile col Rigidbody
            yield return new WaitForFixedUpdate(); 
        }

        // Fissiamo la pallina esattamente sul punto di arrivo per correggere eventuali millimetri di scarto
        rb.MovePosition(target);
        isMoving = false;
        if (audioPallina != null && suonoStop != null) audioPallina.PlayOneShot(suonoStop);
    }

void OnTriggerEnter(Collider other)
    {
        Dp=GetComponent<DirectionalPad>();
        // 1. Rilevamento della vittoria
        if (other.CompareTag("Finish") && gameManager != null)
        {
            gameManager.Vittoria();
        }

        // 2. Rilevamento del Pad Direzionale
        DirectionalPad pad = other.GetComponent<DirectionalPad>();
        if (pad != null)
        {
            if (audioPallina != null && suonoPad != null) audioPallina.PlayOneShot(suonoPad);
            // Ferma la pallina all'istante (interrompe la Coroutine)
            if (movementCoroutine != null)
            {
                StopCoroutine(movementCoroutine);
            }

            // Diciamo allo script che la pallina non si sta più muovendo...
            isMoving = false; 

            // ...e lanciamo IMMEDIATAMENTE un nuovo movimento nella direzione del pad!
            TryMove(pad.OttieniVettoreDirezione());
        }
    }
    public void MuoviAvanti()   { TryMove(Vector3.forward); }
    public void MuoviIndietro() { TryMove(Vector3.back); }
    public void MuoviDestra()   { TryMove(Vector3.right); }
    public void MuoviSinistra() { TryMove(Vector3.left); }
    public void MuoviSu()       { TryMove(Vector3.up); }
    public void MuoviGiu()      { TryMove(Vector3.down); }
}
