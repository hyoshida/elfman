using Stage;
using UnityEngine;

public class Elevator : InteractiveObject {
    [SerializeField]
    GameObject _elevatorMenu;

    override public void Interact() {
        _elevatorMenu.SetActive(true);

        var scene = (Scene)ApplicationScene.Instance;
        scene.ChangeToMenuMode(() => _elevatorMenu.SetActive(false));
    }
}
