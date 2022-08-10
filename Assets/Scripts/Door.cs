using UnityEngine;

public class Door : MonoBehaviour
{
    public bool showMessageOpenDoor = false;
    private void OnGUI()
    {
        if (showMessageOpenDoor)
            GUI.Box(new Rect(Screen.width / 2, Screen.height / 2, 200, 25), "Press 'E' to open/close the door");
    }
}
