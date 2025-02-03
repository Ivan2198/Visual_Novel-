using Unity.VisualScripting;
using UnityEngine;

public class ShowWheel : MonoBehaviour
{
    [SerializeField] private GameObject wheelMenu;

    private void Update()
    {
        if (wheelMenu != null && Input.GetKeyDown(KeyCode.Tab))
        {
            wheelMenu.SetActive(true);
        }
    }
}
