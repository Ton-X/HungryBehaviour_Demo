using UnityEngine;
using System.Collections;

[System.Serializable]
public abstract class BaseBehaviourNode : BaseNodeRuntime {

	public GameObject go;
	public BehaviourNodeEditor editorNode;

	private int logicResult = (int) Result.RUNNING;

	public override void Tick ()
	{
		logicResult = DoLogic();

		if (logicResult == ((int) Result.SUCCESS)) {
			ResultValue = Result.SUCCESS.ToString ();
		} else if (logicResult == ((int) Result.FAILURE)) {
			ResultValue = Result.FAILURE.ToString ();
		} else {
			ResultValue = Result.RUNNING.ToString ();
		}
	}

	public virtual void Start(){
	}

	public abstract int DoLogic ();

}
