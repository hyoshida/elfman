namespace StageSelect {
    public class Scene : ApplicationScene {
        public void onClicktStage(int stageCode) {
            GameManager.Instance.GotoStage((uint)stageCode);
        }
    }
}