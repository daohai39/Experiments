using Common.Logger;
using UnityEngine;
using UnityEngine.UI;

namespace Common.UI {
  public class ExamplePopupScreen : UIScreen {
    [SerializeField] private Text messageLabel;
    [SerializeField] private Button okButton;
    public override void OnSetup() {
      okButton.onClick.AddListener(HandleOkClicked);
    }

    public override void OnPush(Data data) {
      messageLabel.text = data.Get<string>("message", "Default Example Label");
      PushFinished();
    }

    public override void OnPop() {
      PopFinished();
    }

    public override void OnFocus() {
    }

    public override void OnFocusLost() {
    }

    private void HandleOkClicked() {
      this.Log($"====Clicked OK!");
      UIManager.Instance.Pop();
    }

    private void OnDestroy() {
      okButton.onClick.RemoveAllListeners();
    }
  }
}