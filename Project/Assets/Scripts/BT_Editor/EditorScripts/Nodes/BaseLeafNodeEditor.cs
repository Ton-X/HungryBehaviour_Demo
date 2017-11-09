using System;

public abstract class BaseLeafNodeEditor : BaseNodeEditor
{
	public string nodeName;

#if UNITY_EDITOR
	public override void Draw() {
		base.Draw ();
	}
#endif
	public override void DrawConnections () {
	}

	public override void NodeDeleted(BaseNodeEditor node) {
	}

}

