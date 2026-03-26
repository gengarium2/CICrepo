using UnityEngine;

public class Portal : MonoBehaviour
{
    public enum DirezionePortale { Avanti, Indietro, Destra, Sinistra, Su, Giu }

    [Header("Collegamento")]
    [Tooltip("Trascina qui il portale di destinazione a cui questo è collegato")]
    public Portal linkedPortal;

    [Header("Direzione di Uscita")]
    [Tooltip("La direzione verso cui la pallina verrà sparata fuori quando esce da QUESTO portale")]
    public DirezionePortale direzioneUscita;

    public Vector3 OttieniDirezioneUscita()
    {
        switch (direzioneUscita)
        {
            case DirezionePortale.Avanti: return Vector3.forward;
            case DirezionePortale.Indietro: return Vector3.back;
            case DirezionePortale.Destra: return Vector3.right;
            case DirezionePortale.Sinistra: return Vector3.left;
            case DirezionePortale.Su: return Vector3.up;
            case DirezionePortale.Giu: return Vector3.down;
            default: return Vector3.forward;
        }
    }
}