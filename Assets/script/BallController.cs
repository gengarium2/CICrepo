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
    public AudioClip suonoPortale;

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
        if (col != null)
        {
            // Moltiplichiamo per la scala nel caso la pallina sia stata ingrandita/rimpicciolita
            ballRadius = col.radius * transform.localScale.x;
        }
        else
        {
            ballRadius = transform.localScale.x / 2f;
        }
    }

    void Update()
    {
        // Se si sta già muovendo, ignora i nuovi input
        if (isMoving) return;

        // --- CONTROLLI ---
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) TryMove(Vector3.forward);
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) TryMove(Vector3.back);
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) TryMove(Vector3.right);
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) TryMove(Vector3.left);
        else if (Input.GetKeyDown(KeyCode.Space)) TryMove(Vector3.up);
        else if (Input.GetKeyDown(KeyCode.LeftShift)) TryMove(Vector3.down);
    }

    // MODIFICA: Aggiunto "bool contaMossa = true". Di base conta sempre la mossa, tranne se gli diciamo noi di no!
    void TryMove(Vector3 direction, bool contaMossa = true)
    {
        if (isMoving) return;
        RaycastHit hit;

        // Lanciamo il raggio dal centro della pallina
        if (Physics.Raycast(transform.position, direction, out hit, Mathf.Infinity))
        {
            if (hit.distance <= ballRadius + 0.05f)
            {
                return; // Siamo già attaccati al muro. Interrompiamo tutto.
            }

            Vector3 targetPosition = transform.position + direction * (hit.distance - ballRadius);

            // MODIFICA: Aggiungiamo la mossa SOLO se l'interruttore contaMossa è vero
            if (contaMossa && gameManager != null)
            {
                gameManager.AggiungiMossa();
            }

            Portal portaleColpito = hit.collider.GetComponent<Portal>();

            movementCoroutine = StartCoroutine(SlideToPosition(targetPosition, portaleColpito));
        }
    }

    IEnumerator SlideToPosition(Vector3 target, Portal portaleColpito = null)
    {
        isMoving = true; // Blocca gli input

        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            rb.MovePosition(newPosition);
            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(target);
        isMoving = false;

        if (portaleColpito != null && portaleColpito.linkedPortal != null)
        {
            // Abbiamo toccato un portale
            if (audioPallina != null && suonoPortale != null) audioPallina.PlayOneShot(suonoPortale);

            Portal uscita = portaleColpito.linkedPortal;
            Vector3 dirUscita = uscita.OttieniDirezioneUscita();

            Vector3 nuovaPos = uscita.transform.position + (dirUscita * ballRadius);
            transform.position = nuovaPos;
            rb.MovePosition(nuovaPos);

            // MODIFICA: Ripartenza automatica passando "false". La mossa è GRATIS!
            TryMove(dirUscita, false);
        }
        else
        {
            // Muro normale, suona il tonfo
            if (audioPallina != null && suonoStop != null) audioPallina.PlayOneShot(suonoStop);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Dp = GetComponent<DirectionalPad>();

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

            if (movementCoroutine != null)
            {
                StopCoroutine(movementCoroutine);
            }

            isMoving = false;

            // Il pad per ora conta come una mossa extra. 
            // Se in futuro vorrai che anche il pad non conti la mossa, ti basterà cambiare questa riga in: TryMove(pad.OttieniVettoreDirezione(), false);
            TryMove(pad.OttieniVettoreDirezione());
        }
    }

    public void MuoviAvanti() { TryMove(Vector3.forward); }
    public void MuoviIndietro() { TryMove(Vector3.back); }
    public void MuoviDestra() { TryMove(Vector3.right); }
    public void MuoviSinistra() { TryMove(Vector3.left); }
    public void MuoviSu() { TryMove(Vector3.up); }
    public void MuoviGiu() { TryMove(Vector3.down); }
}