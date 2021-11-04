using UnityEngine;

public class ExampleObjectPoolerSpawner : PoolerBase<ExampleShape> {
  [SerializeField] private ExampleShape _shapePrefab;

  private void Start() {
    InitPool(_shapePrefab); // Initialize the pool

    var shape = Get(); // Pull from the pool
    Release(shape); // Release back to the pool
  }

  // Optionally override the setup components
  protected override void GetSetup(ExampleShape shape) {
    base.GetSetup(shape);
    shape.transform.position = Vector3.zero;
  }
}


public class ExampleShape : MonoBehaviour {}