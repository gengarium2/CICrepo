using UnityEngine;

public class UIRespiro : MonoBehaviour
{
    [Header("Impostazioni Animazione")]
    public float velocita = 2f; // Quanto velocemente pulsa
    public float scalaMinima = 0.9f; // Quanto diventa piccolo (0.9 = 90%)
    public float scalaMassima = 1.1f; // Quanto diventa grande (1.1 = 110%)

    private Vector3 scalaOriginale;

    void Start()
    {
        // Si ricorda la grandezza iniziale dell'oggetto
        scalaOriginale = transform.localScale;
    }

    void Update()
    {
        // Oscilla dolcemente tra 0 e 1
        float tempo = Mathf.PingPong(Time.time * velocita, 1f);

        // Calcola la nuova scala interpolando tra il minimo e il massimo
        float scalaAttuale = Mathf.Lerp(scalaMinima, scalaMassima, tempo);

        // Applica la grandezza
        transform.localScale = scalaOriginale * scalaAttuale;
    }
}