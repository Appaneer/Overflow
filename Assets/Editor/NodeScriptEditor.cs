using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Node))]
public class NodeScriptEditor : Editor {

	private bool isPowerUpToggle;
	private Node _node;
	private int selected = 0;

	void OnEnable(){
		_node = (Node)target;
	}

	public override void OnInspectorGUI(){
		GUILayout.BeginHorizontal ();
		GUILayout.Label("Value", GUILayout.Width(70));
		_node.value = EditorGUILayout.IntField (_node.value);
		GUILayout.EndHorizontal ();
		GUILayout.Space (5);
		GUILayout.BeginHorizontal ();
		GUILayout.Label ("is power up?", GUILayout.Width(70));
		isPowerUpToggle = EditorGUILayout.Toggle (isPowerUpToggle);
		GUILayout.EndHorizontal ();

		if(isPowerUpToggle){
			GUILayout.Label("My Power Up:", GUILayout.Width(100));
			selected = EditorGUILayout.Popup ("Label", selected, new string[] { "rowEliminator", "bomb", "coin" });
			switch (selected) {
			case 0:
				_node.myPowerUp = powerUp.rowEliminator;
				break;
			case 1:
				_node.myPowerUp = powerUp.bomb;
				break;
			case 2:
				_node.myPowerUp = powerUp.coin;
				break;
			}
		}
	}
}
