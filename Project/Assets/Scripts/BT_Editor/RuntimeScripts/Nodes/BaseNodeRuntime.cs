using UnityEngine;
using System.Collections;

[System.Serializable]
public abstract class BaseNodeRuntime {

	public enum Result { SUCCESS, FAILURE, RUNNING }

	public string ResultValue {
		get {
			return result;
		}
		set {
			result = value;
		}
	}

	[SerializeField]
	private string result = Result.RUNNING.ToString();

	public abstract void Tick();
}
