using UnityEngine;
using Stage;

public class ElevatorMenu : MonoBehaviour {
    public void OnSave() {
        Scene instance = (Scene)ApplicationScene.Instance;
        instance.Save();
    }
}