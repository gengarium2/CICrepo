using UnityEngine;

public class SkyboxManager : MonoBehaviour
{
    [Header("Materiali Skybox")]
    public Material skyboxNotte;  // Skybox 1 (Nera/Default)
    public Material skyboxGiorno; // Skybox 2 (Bianca)

    void Start()
    {
        // Legge l'opzione (1 = Notte, 0 = Giorno). Default è 1.
        bool isNotturna = PlayerPrefs.GetInt("ModalitaNotturna", 1) == 1;

        // Cambia la skybox globale del gioco
        RenderSettings.skybox = isNotturna ? skyboxNotte : skyboxGiorno;

        // Aggiorna l'illuminazione ambientale per farla combaciare con il nuovo cielo
        DynamicGI.UpdateEnvironment(); 
    }
}
