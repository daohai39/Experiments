namespace Common.UI {
  public class ExampleMenu {
    private static readonly UIScreen.Id ScreenId_Empty = new UIScreen.Id("Empty");
    private static readonly UIScreen.Id ScreenId_ExamplePopup = new UIScreen.Id("ExamplePopup");

    private void AddNewUIScreen() {
      UIManager.Instance.Push(ScreenId_Empty, null, "ExampleEmptyScreen");
      UIManager.Instance.Push(ScreenId_ExamplePopup, null, "ExamplePopupScreen");
    }
  }
}