using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Tester), true)]
public class TesterEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		Tester tester = (Tester)target;
		if (GUILayout.Button("Run Test"))
		{
			tester.RunTest();
		}
	}
}