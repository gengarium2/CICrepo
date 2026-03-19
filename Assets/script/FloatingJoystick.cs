using UnityEngine;
using UnityEngine.EventSystems;

public class FloatingJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("UI Elementi")]
    public RectTransform joystickBackground;
    public RectTransform joystickHandle;

    public Vector2 InputVector { get; private set; }
    public bool IsTouching { get; private set; }

    private RectTransform touchZone;

    private void Start()
    {
        touchZone = GetComponent<RectTransform>(); // Prende in automatico il pannello invisibile
        
        // Spegne esplicitamente ENTRAMBI i pezzi all'avvio
        joystickBackground.gameObject.SetActive(false);
        joystickHandle.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        IsTouching = true;
        joystickBackground.gameObject.SetActive(true);
        joystickHandle.gameObject.SetActive(true);
        
        // Calcola il punto esatto del tocco e sposta lì il joystick
        RectTransformUtility.ScreenPointToLocalPointInRectangle(touchZone, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
        joystickBackground.anchoredPosition = localPoint;
        
        joystickHandle.anchoredPosition = Vector2.zero;
        InputVector = Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBackground, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
        {
            float radius = joystickBackground.sizeDelta.x / 2f;
            InputVector = localPoint / radius;
            
            if (InputVector.magnitude > 1f) InputVector = InputVector.normalized;
            
            joystickHandle.anchoredPosition = InputVector * radius;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsTouching = false;
        joystickBackground.gameObject.SetActive(false);
        joystickHandle.gameObject.SetActive(false);
        InputVector = Vector2.zero;
    }
}