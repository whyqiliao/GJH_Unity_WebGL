using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Lovatto.OrbitCamera;

[CustomEditor(typeof(bl_CameraOrbit))]
public class bl_CameraOrbitEditor : Editor
{
    bl_CameraOrbit script;

    private void OnEnable()
    {
        script = (bl_CameraOrbit)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUI.BeginChangeCheck();
        GUILayout.BeginVertical("box");

        GUILayout.BeginVertical(EditorStyles.helpBox);
        script.Target = EditorGUILayout.ObjectField("Target", script.Target, typeof(Transform), true) as Transform;
        script.TargetOffset = EditorGUILayout.Vector3Field("Target Offset", script.TargetOffset);
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box");
        script.executeInEditMode = EditorGUILayout.ToggleLeft("Execute In Edit Mode", script.executeInEditMode, EditorStyles.toolbarButton);
        GUILayout.Space(2);
        script.isForMobile = EditorGUILayout.ToggleLeft("Is for Mobile", script.isForMobile, EditorStyles.toolbarButton);
        GUILayout.EndVertical();

        GUILayout.BeginVertical(EditorStyles.helpBox);
        script.editorValues["input"] = DrawHeader("Input", script.editorValues["input"]);
        if (script.editorValues["input"])
        {
            script.RequiredInput = EditorGUILayout.ToggleLeft("Require Input To Rotate", script.RequiredInput, EditorStyles.toolbarButton);
            if (script.RequiredInput)
            {
                GUILayout.Space(2);
                script.RotateInputKey = (CameraMouseInputType)EditorGUILayout.EnumPopup("Rotate Input Key", script.RotateInputKey, EditorStyles.toolbarPopup);
                script.LockCursorOnRotate = EditorGUILayout.ToggleLeft("Lock Cursor On Rotate", script.LockCursorOnRotate, EditorStyles.toolbarButton);
                GUILayout.Space(2);
                script.InputMultiplier = EditorGUILayout.Slider("Input Sensitivity", script.InputMultiplier, 0.001f, 0.07f);
                script.InputLerp = EditorGUILayout.Slider("Input Smooth", script.InputLerp, 0.1f, 15);
            }
            script.useKeys = EditorGUILayout.ToggleLeft("Use Keyboard Axis", script.useKeys, EditorStyles.toolbarButton);
        }
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box");
        script.editorValues["move"] = DrawHeader("Movement", script.editorValues["move"]);
        if (script.editorValues["move"])
        {
            script.MovementType = (CameraMovementType)EditorGUILayout.EnumPopup("Movement Type", script.MovementType, EditorStyles.toolbarPopup);
            GUILayout.Space(2);
            script.SpeedAxis.x = EditorGUILayout.Slider("Horizontal Speed", script.SpeedAxis.x, 1f, 200);
            script.SpeedAxis.y = EditorGUILayout.Slider("Vertical Speed", script.SpeedAxis.y, 1f, 200);
            if (script.MovementType != CameraMovementType.Normal)
            {
                script.LerpSpeed = EditorGUILayout.Slider("Rotation Smooth", script.LerpSpeed, -90, 90);
            }
            script.OutInputSpeed = EditorGUILayout.Slider("Inertia Speed", script.OutInputSpeed, 1f, 100);

            script.AutoRotate = EditorGUILayout.ToggleLeft("Auto Rotation", script.AutoRotate, EditorStyles.toolbarButton);
            GUILayout.Space(2);
            if (script.AutoRotate)
            {
                script.AutoRotationType = (CameraAutoRotationType)EditorGUILayout.EnumPopup("Auto Rotation Side", script.AutoRotationType, EditorStyles.toolbarPopup);
                GUILayout.Space(2);
                script.AutoRotSpeed = EditorGUILayout.Slider("Auto Rotation Speed", script.AutoRotSpeed, 0, 20);
            }
            script.ClampHorizontal = EditorGUILayout.ToggleLeft("Clamp Horizontal Rotation", script.ClampHorizontal, EditorStyles.toolbarButton);
            GUILayout.Space(2);
            if (script.ClampHorizontal)
            {
                EditorGUILayout.MinMaxSlider(string.Format("X Clamp ({0:00}|{1:00})",script.XLimitClamp.x,script.XLimitClamp.y), ref script.XLimitClamp.x, ref script.XLimitClamp.y, 0, 360);
            }
            script.ClampVertical = EditorGUILayout.ToggleLeft("Clamp Vertical Rotation", script.ClampVertical, EditorStyles.toolbarButton);
            GUILayout.Space(2);
            if (script.ClampVertical)
            {
                EditorGUILayout.MinMaxSlider(string.Format("Y Clamp ({0:00}|{1:00})", script.YLimitClamp.x, script.YLimitClamp.y), ref script.YLimitClamp.x, ref script.YLimitClamp.y, -180, 180);
            }
        }
        GUILayout.EndVertical();

        GUILayout.BeginVertical(EditorStyles.helpBox);
        script.editorValues["zoom"] = DrawHeader("Zoom", script.editorValues["zoom"]);
        if (script.editorValues["zoom"])
        {
            script.Distance = EditorGUILayout.FloatField("Default Distance", script.Distance);
            EditorGUILayout.MinMaxSlider(string.Format("Distance Clamp ({0:00}|{1:00})",script.DistanceClamp.x,script.DistanceClamp.y), ref script.DistanceClamp.x, ref script.DistanceClamp.y, 0, 50);
            script.ScrollSensitivity = EditorGUILayout.Slider("Zoom Sensitivity", script.ScrollSensitivity, 1, 10);
            script.ZoomSpeed = EditorGUILayout.Slider("Zoom Speed", script.ZoomSpeed, 1, 25);
            script.useZoomOutEffect = EditorGUILayout.ToggleLeft("Use Zoom Out Effect", script.useZoomOutEffect, EditorStyles.toolbarButton);
            GUILayout.Space(2);
            if (script.useZoomOutEffect)
            {
                script.TouchZoomAmount = EditorGUILayout.Slider("Zoom Out Amount", script.TouchZoomAmount, -90, 90);
            }
            script.useStartZoomEffect = EditorGUILayout.ToggleLeft("Use Start Zoom Effect", script.useStartZoomEffect, EditorStyles.toolbarButton);
            GUILayout.Space(2);
            if (script.useStartZoomEffect)
            {
                script.StartFov = EditorGUILayout.Slider("Start Field Of View (Zoom)", script.StartFov, 5, 179);
                script.DelayStartFoV = EditorGUILayout.Slider("Start Delay", script.DelayStartFoV, 0.0f, 7);
                script.FovLerp = EditorGUILayout.Slider("Zoom In Speed", script.FovLerp, 0.1f, 15);
            }
        }
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box");
        script.editorValues["collision"] = DrawHeader("Collision", script.editorValues["collision"]);
        if (script.editorValues["collision"])
        {
            script.DetectCollision = EditorGUILayout.ToggleLeft("Detect Collisions", script.DetectCollision, EditorStyles.toolbarButton);
            GUILayout.Space(2);
            if (script.DetectCollision)
            {
                script.DetectCollisionLayers = EditorGUILayout.LayerField("Collision Layers", script.DetectCollisionLayers);
                script.TeleporOnHit = EditorGUILayout.ToggleLeft("Teleport", script.TeleporOnHit, EditorStyles.toolbarButton);
                GUILayout.Space(2);
                script.CollisionRadius = EditorGUILayout.Slider("Collision Radius", script.CollisionRadius, 0.01f, 4);
            }
        }
        GUILayout.EndVertical();

        GUILayout.BeginVertical(EditorStyles.helpBox);
        script.editorValues["fade"] = DrawHeader("Fade", script.editorValues["fade"]);
        if (script.editorValues["fade"])
        {
            script.FadeOnStart = EditorGUILayout.ToggleLeft("Camera Fade On Start", script.FadeOnStart, EditorStyles.toolbarButton);
            GUILayout.Space(2);
            if (script.FadeOnStart)
            {
                script.FadeSpeed = EditorGUILayout.Slider("Fade Speed", script.FadeSpeed, 0.01f, 5);
                script.FadeColor = EditorGUILayout.ColorField("Fade Color", script.FadeColor);
            }
        }
        GUILayout.EndVertical();

        GUILayout.EndVertical();
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }
    }

    bool DrawHeader(string text, bool value)
    {
        if (GUILayout.Button(text,EditorStyles.toolbarDropDown))
        {
            value = !value;
        }
        GUILayout.Space(2);
        return value;
    }
}