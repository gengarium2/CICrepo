using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    [Header("Riferimenti")]
    public FloatingJoystick joystick; // Trascina qui la tua TouchZoneCamera
    
    [Header("Impostazioni")]
    public float velocitaRotazione = 120f;
    public float velocitaRitorno = 5f;

    private float startRotX;
    private float startRotY;
    private float currentRotX;
    private float currentRotY;

    void Start()
    {
        // Memorizza i famosi 45 gradi iniziali
        startRotX = transform.eulerAngles.y; 
        startRotY = transform.eulerAngles.x;
        
        currentRotX = startRotX;
        currentRotY = startRotY;
    }

    void Update()
    {
        if (joystick.IsTouching)
        {
            // Se tieni premuto, orbita la telecamera in base a come muovi il dito
            currentRotX += joystick.InputVector.x * velocitaRotazione * Time.deltaTime;
            currentRotY -= joystick.InputVector.y * velocitaRotazione * Time.deltaTime; // - per invertire l'asse

            // Limita la rotazione in verticale per non far finire la visuale sotto il pavimento (modifica questi valori se serve)
            currentRotY = Mathf.Clamp(currentRotY, 10f, 85f); 
        }
        else
        {
            // Se rilasci il dito, la telecamera "scivola" magicamente alla posizione originale
            currentRotX = Mathf.Lerp(currentRotX, startRotX, velocitaRitorno * Time.deltaTime);
            currentRotY = Mathf.Lerp(currentRotY, startRotY, velocitaRitorno * Time.deltaTime);
        }

        // Applica la rotazione matematica al Perno
        transform.rotation = Quaternion.Euler(currentRotY, currentRotX, 0);
    }
}