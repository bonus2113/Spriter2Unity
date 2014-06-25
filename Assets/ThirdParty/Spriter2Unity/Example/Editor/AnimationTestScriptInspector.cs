using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

[CustomEditor(typeof(AnimationTestScript))]
public class AnimationTestScriptInspector : Editor 
{
    public override void OnInspectorGUI()
    {
        AnimationTestScript myTarget = (AnimationTestScript)target;

        EditorGUILayout.LabelField("Set a trigger:");

        foreach (AnimationTestScript.TriggerType trigger in Enum.GetValues(typeof(AnimationTestScript.TriggerType)))
        {
            if (GUILayout.Button(trigger.ToString()))
            {
                myTarget.SetTrigger(trigger);
            }
        }

        EditorGUILayout.Space();
        myTarget.Speed = EditorGUILayout.Slider("Movement Speed", myTarget.Speed, 0, 10);
    }
}
