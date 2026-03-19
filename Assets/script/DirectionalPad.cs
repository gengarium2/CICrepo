using UnityEngine;

public class DirectionalPad : MonoBehaviour
{
    public enum DirezionePad { Avanti, Indietro, Destra, Sinistra, Su, Giu }
    public enum AssePad { pavimento, MuroAvanti, MuroDestra }

    [Header("In che direzione lancia la pallina?")]
    public DirezionePad direzione;
        [Header("asse pad")]
    public AssePad asse;

    // Questa funzione traduce la scelta nel menu a tendina in un vettore reale
    public Vector3 OttieniVettoreDirezione()
    {
        switch (direzione)
        {
            case DirezionePad.Avanti: return Vector3.forward;
            case DirezionePad.Indietro: return Vector3.back;
            case DirezionePad.Destra: return Vector3.right;
            case DirezionePad.Sinistra: return Vector3.left;
            case DirezionePad.Su: return Vector3.up;
            case DirezionePad.Giu: return Vector3.down;
            default: return Vector3.forward;
        }
    }
}
