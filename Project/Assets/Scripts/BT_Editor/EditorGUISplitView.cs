using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class EditorGUISplitView
{

	public enum Direction {
		Horizontal,
		Vertical
	}

	Direction splitDirection;
	float splitNormalizedPosition;
	bool resize;
	public Vector2 scrollPosition;
	Rect availableRect;


	public EditorGUISplitView(Direction splitDirection) {
		splitNormalizedPosition = 0.5f;
		this.splitDirection = splitDirection;
	}

#if UNITY_EDITOR
	public Rect BeginSplitView() {
		Rect tempRect;

		if(splitDirection == Direction.Horizontal)
			tempRect = EditorGUILayout.BeginHorizontal (GUILayout.ExpandWidth(true), GUILayout.Width(500));
		else 
			tempRect = EditorGUILayout.BeginVertical (GUILayout.ExpandHeight(true));

		if (tempRect.width > 0.0f) {
			availableRect = tempRect;
		}
		if(splitDirection == Direction.Horizontal)
			scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(availableRect.width * splitNormalizedPosition));
		else
			scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(availableRect.height * splitNormalizedPosition));

		return availableRect;
	}

	public void Split() {
		GUILayout.EndScrollView();
		ResizeSplitFirstView ();
	}

	public void EndSplitView() {

		if(splitDirection == Direction.Horizontal)
			EditorGUILayout.EndHorizontal ();
		else 
			EditorGUILayout.EndVertical ();
	}

	private void ResizeSplitFirstView(){

		Rect resizeHandleRect;

		if(splitDirection == Direction.Horizontal)
			resizeHandleRect = new Rect (availableRect.width * splitNormalizedPosition, availableRect.y, 3f, availableRect.height);
		else
			resizeHandleRect = new Rect (availableRect.x,availableRect.height * splitNormalizedPosition, availableRect.width, 3f);

		Color defaultColor = GUI.color;
		GUI.color = Color.grey;
		GUI.DrawTexture(resizeHandleRect,EditorGUIUtility.whiteTexture);
		GUI.color = defaultColor;

		if (splitDirection == Direction.Horizontal) {
		}
			//EditorGUIUtility.AddCursorRect(resizeHandleRect,MouseCursor.ResizeHorizontal);
		else{}
			//EditorGUIUtility.AddCursorRect(resizeHandleRect,MouseCursor.ResizeVertical);

		if( Event.current.type == EventType.mouseDown && resizeHandleRect.Contains(Event.current.mousePosition)){
			resize = true;
		}
		if(resize){
			if (splitDirection == Direction.Horizontal) {
			}
				//splitNormalizedPosition = Event.current.mousePosition.x / availableRect.width;
			else{}
				//splitNormalizedPosition = (Event.current.mousePosition.y / availableRect.height);
		}
		if(Event.current.type == EventType.MouseUp)
			resize = false;      
	}
#endif
}

