using UnityEngine;
using UnityEngine.EventSystems;

public class ListenerButAccelerator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        PlayerMovement.IsClickAccelerator = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        PlayerMovement.IsClickAccelerator = false;
    }
}
