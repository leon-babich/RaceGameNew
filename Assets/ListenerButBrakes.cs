using UnityEngine;
using UnityEngine.EventSystems;

public class ListenerButBrakes : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        PlayerMovement.IsClickBrekes = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        PlayerMovement.IsClickBrekes = false;
    }
}
