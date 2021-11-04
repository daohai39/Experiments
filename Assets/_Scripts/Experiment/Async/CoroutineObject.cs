using System;
using System.Threading;

namespace Experiment.Async {
  public class CoroutineObject {
  //   public async UnitTask<int> BarAsync() {
  //     try {
  //       var x = await FooSync();
  //       return x * 2;
  //     } catch (Exception ex) when (!(ex is OperationCanceledException)) { //should separate OperationCanceledException from other exception
  //       DialogService.ShowReturnToTitleAsyns().Forget();
  //       //by rethrowing with OperationCanceledException
  //       //you can expected to display results similar to Observable.Empty/Never.
  //       throw new OperationCanceledException(); 
  //     }
  //   }
  //
  //   async UniTask TrippleClick(CancellationToken token) {
  //     // Acquiring a Hadnler at th e start is more efficent than doing OnClick/token passing each time
  //     using (var handler = button.GetAsyncClickEventHandler(token)) {
  //       await button.OnClickAysnc();
  //       await button.OnClickAysnc();
  //       await button.OnClickAysnc();
  //       //when using await for an event, it's best to pass CancellationToken since there's a risk of infinite standby
  //     }
  //   }
  }
}