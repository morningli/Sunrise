using UnityEngine;
using System.Collections;

public class MenuTopPanel : MonoBehaviour 
{
    GameObject RightBar;
    void Awake()
    {
		RightBar = Util.FindChild("RightBar", gameObject);
		EventTriggerListener.Get(Util.FindChild("BackButton", gameObject)).onClick = OnBackButtonClick;
		EventTriggerListener.Get(Util.FindChild("BarButton", gameObject)).onClick = OnBarButtonClick;
		EventTriggerListener.Get(Util.FindChild("HeroButton", gameObject)).onClick = OnHeroButtonClick;
    }

    private void OnBarButtonClick(GameObject go)
    {
        //RightBar.SetActive(!RightBar.activeSelf);
    }

    private void OnBackButtonClick(GameObject go)
    {
        //PageManager.Instance.ShowPage(UIMainMenuPage.Instance, PageManager.AnimationType.LeftToRight);
    }

    private void OnHeroButtonClick(GameObject go)
    {
        //PageManager.Instance.ShowPage(UIBagPage.Instance);
    }

}
