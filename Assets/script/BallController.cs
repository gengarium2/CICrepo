using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Rigidbody))]
public class BallController : MonoBehaviour
{
    [Header("Impostazioni di Movimento")]
    public float speed = 15f; 
    private bool isMoving = false; 
    [Header("Audio Pallina")]
    public AudioSource audioPallina;
    public AudioClip suonoStop;
    public AudioClip suonoPad;
    public AudioClip suonoPortale;

    [Header("Riferimenti")]
    public GameManager gameManager; 
    private Rigidbody rb;
    private DirectionalPad Dp;
    private float ballRadius;
    private Coroutine movementCoroutine;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; 
        SphereCollider col = GetComponent<SphereCollider>();
        if (col != null)
        {
            ballRadius = col.radius * transform.localScale.x;
        }
        else
        {
            ballRadius = transform.localScale.x / 2f;
        }
    }

    void Update()
    {
        if (isMoving) return;
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) TryMove(Vector3.forward);
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) TryMove(Vector3.back);
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) TryMove(Vector3.right);
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) TryMove(Vector3.left);
        else if (Input.GetKeyDown(KeyCode.Space)) TryMove(Vector3.up);
        else if (Input.GetKeyDown(KeyCode.LeftShift)) TryMove(Vector3.down);
    }
    void TryMove(Vector3 direction, bool contaMossa = true)
    {
        if (isMoving) return;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, direction, out hit, Mathf.Infinity))
        {
            if (hit.distance <= ballRadius + 0.05f) return;

            Vector3 targetPosition = transform.position + direction * (hit.distance - ballRadius);

            if (contaMossa && gameManager != null) gameManager.AggiungiMossa();

            // --- RILEVAMENTO COSA ABBIAMO COLPITO DI PUNTA ---
            Portal portaleColpito = hit.collider.GetComponent<Portal>();
            DirectionalPad padColpito = hit.collider.GetComponent<DirectionalPad>();
            bool colpitaFine = hit.collider.CompareTag("Finish");

            // Passiamo tutto alla Coroutine
            movementCoroutine = StartCoroutine(SlideToPosition(targetPosition, portaleColpito, padColpito, colpitaFine));
        }
    }

    IEnumerator SlideToPosition(Vector3 target, Portal portaleColpito, DirectionalPad padColpito, bool colpitaFine)
    {
        isMoving = true;

        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            rb.MovePosition(Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime));
            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(target);
        isMoving = false;

        // --- LOGICA DI RISOLUZIONE (Cosa succede quando mi fermo?) ---

        // 1. È la fine del livello?
        if (colpitaFine && gameManager != null)
        {
            gameManager.Vittoria();
        }
        // 2. È un portale?
        else if (portaleColpito != null && portaleColpito.linkedPortal != null)
        {
            if (audioPallina != null && suonoPortale != null) audioPallina.PlayOneShot(suonoPortale);
            Portal uscita = portaleColpito.linkedPortal;
            Vector3 dirUscita = uscita.OttieniDirezioneUscita();
            Vector3 nuovaPos = uscita.transform.position + (dirUscita * ballRadius);
            transform.position = nuovaPos;
            rb.MovePosition(nuovaPos);
            TryMove(dirUscita, false);
        }
        // 3. È un pad direzionale sul muro?
        else if (padColpito != null)
        {
            if (audioPallina != null && suonoPad != null) audioPallina.PlayOneShot(suonoPad);
            TryMove(padColpito.OttieniVettoreDirezione());
        }
        // 4. È un muro normale?
        else
        {
            if (audioPallina != null && suonoStop != null) audioPallina.PlayOneShot(suonoStop);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish") && gameManager != null)
        {
            gameManager.Vittoria();
        }

        DirectionalPad pad = other.GetComponent<DirectionalPad>();
        if (pad != null) // Solo se ci stiamo passando sopra
        {
            if (audioPallina != null && suonoPad != null) audioPallina.PlayOneShot(suonoPad);
            if (movementCoroutine != null) StopCoroutine(movementCoroutine);
            isMoving = false;
            TryMove(pad.OttieniVettoreDirezione());
        }
    }

    IEnumerator SlideToPosition(Vector3 target, Portal portaleColpito = null)
    {
        isMoving = true;

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
            if (audioPallina != null && suonoPortale != null) audioPallina.PlayOneShot(suonoPortale);

            Portal uscita = portaleColpito.linkedPortal;
            Vector3 dirUscita = uscita.OttieniDirezioneUscita();

            Vector3 nuovaPos = uscita.transform.position + (dirUscita * ballRadius);
            transform.position = nuovaPos;
            rb.MovePosition(nuovaPos);
            TryMove(dirUscita, false);
        }
        else
        {
            if (audioPallina != null && suonoStop != null) audioPallina.PlayOneShot(suonoStop);
        }
    }
    
    public void MuoviAvanti() { TryMove(Vector3.forward); }
    public void MuoviIndietro() { TryMove(Vector3.back); }
    public void MuoviDestra() { TryMove(Vector3.right); }
    public void MuoviSinistra() { TryMove(Vector3.left); }
    public void MuoviSu() { TryMove(Vector3.up); }
    public void MuoviGiu() { TryMove(Vector3.down); }
}