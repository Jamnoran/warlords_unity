using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DunGen.Editor
{
    [CustomEditor(typeof(GlobalProp))]
    public class GlobalPropInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            GlobalProp prop = target as GlobalProp;

            if (prop == null)
                return;

            prop.PropGroupID = EditorGUILayout.IntField("Group ID", prop.PropGroupID);

            GUILayout.BeginVertical("box");

            prop.MainPathWeight = EditorGUILayout.FloatField("Main Path", prop.MainPathWeight);
            prop.BranchPathWeight = EditorGUILayout.FloatField("Branch Path", prop.BranchPathWeight);
            prop.DepthWeightScale = EditorGUILayout.CurveField("Depth Scale", prop.DepthWeightScale, Color.white, new Rect(0, 0, 1, 1));

            GUILayout.EndVertical();

            if (GUI.changed)
                EditorUtility.SetDirty(prop);
        }
    }
}