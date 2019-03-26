using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR;

[RequireComponent(typeof(SteamVR_LaserPointer_mjs))]
public class VR_UI_Input : MonoBehaviour
{
    private SteamVR_LaserPointer_mjs laserPointer;
    private SteamVR_Action_Boolean interactUI = SteamVR_Input.GetBooleanAction("Interact UI");

    private void OnEnable()
    {
        laserPointer = GetComponent<SteamVR_LaserPointer_mjs>();
    }

    private void HandleTriggerClicked(object sender, PointerEventArgs e)
    {
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            ExecuteEvents.Execute(EventSystem.current.currentSelectedGameObject, new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
        }
    }

    private void HandlePointerIn(object sender, PointerEventArgs e)
    {
        var button = e.target.GetComponent<Button>();
        if (button != null)
        {
            button.Select();
            Debug.Log("HandlePointerIn", e.target.gameObject);
        }
    }

    private void HandlePointerOut(object sender, PointerEventArgs e)
    {

        var button = e.target.GetComponent<Button>();
        if (button != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            Debug.Log("HandlePointerOut", e.target.gameObject);
        }
    }
}