using Stage;
using UnityEngine;
using UnityEngine.UI;

public class Elevator : InteractiveObject {
    [SerializeField]
    GameObject _elevatorMenu;

    [SerializeField]
    Button _defaultMenuItem;

    override public void Interact() {
        _elevatorMenu.SetActive(true);
        _defaultMenuItem.Select();

        var scene = (Scene)ApplicationScene.Instance;
        scene.ChangeToMenuMode(() => _elevatorMenu.SetActive(false));
    }
}
