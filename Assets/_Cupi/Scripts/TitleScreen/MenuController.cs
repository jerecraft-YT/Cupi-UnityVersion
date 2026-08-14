using UnityEngine;
using UnityEngine.EventSystems;

public class MenuController : MonoBehaviour
{
    public GameObject IntialSelected;

    private void Start()
    {
        SetIntialSelected(IntialSelected);
    }

    public void ShowMenu(GameObject Menu)
    {
        Menu.SetActive(true);
    }

    public void HideMenu(GameObject Menu) 
    {
        Menu.SetActive(false);
    }

    public void SetIntialSelected(GameObject Item)
    {
        EventSystem.current.SetSelectedGameObject(Item);
    }
}
