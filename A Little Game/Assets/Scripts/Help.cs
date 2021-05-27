using UnityEngine;
using System.Collections;

public class Help : MonoBehaviour
{
    public GameObject helpCanvas;
    private bool isShowing = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            isShowing = !isShowing;
            helpCanvas.SetActive(isShowing);
        }
    }
}
