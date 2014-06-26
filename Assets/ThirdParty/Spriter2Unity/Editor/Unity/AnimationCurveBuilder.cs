/*
Copyright (c) 2014 Andrew Jones, Dario Seyb
 Based on 'Spriter2Unity' python code by Malhavok

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;
using Assets.ThirdParty.Spriter2Unity.Editor.Spriter;

namespace Assets.ThirdParty.Spriter2Unity.Editor.Unity
{
    public class AnimationCurveBuilder
    {
        private class ObjectCurves
        {
            public bool IsSpriteKey;
            public AnimationCurve[] Curves;
        }

        Dictionary<string, ObjectCurves> curveCache = new Dictionary<string, ObjectCurves>();

        public void AddCurves(AnimationClip animClip)
        {
            foreach(var kvp in curveCache)
            {
                var curves = kvp.Value.Curves;
                //Position curves
                SetCurveIfNotEmpty(ref animClip, kvp.Key, typeof(Transform), "localPosition.x", curves[(int)AnimationCurveIndex.LocalPositionX]);
                SetCurveIfNotEmpty(ref animClip, kvp.Key, typeof(Transform), "localPosition.y", curves[(int)AnimationCurveIndex.LocalPositionY]);
                SetCurveIfNotEmpty(ref animClip, kvp.Key, typeof(Transform), "localPosition.z", curves[(int)AnimationCurveIndex.LocalPositionZ]);

                //Rotation curves
                SetCurveIfNotEmpty(ref animClip, kvp.Key, typeof(Transform), "localRotation.x", curves[(int)AnimationCurveIndex.LocalRotationX]);
                SetCurveIfNotEmpty(ref animClip, kvp.Key, typeof(Transform), "localRotation.y", curves[(int)AnimationCurveIndex.LocalRotationY]);
                SetCurveIfNotEmpty(ref animClip, kvp.Key, typeof(Transform), "localRotation.z", curves[(int)AnimationCurveIndex.LocalRotationZ]);
                SetCurveIfNotEmpty(ref animClip, kvp.Key, typeof(Transform), "localRotation.w", curves[(int)AnimationCurveIndex.LocalRotationW]);

                //Scale curves
                SetCurveIfNotEmpty(ref animClip, kvp.Key, typeof(Transform), "localScale.x", curves[(int)AnimationCurveIndex.LocalScaleX]);
                SetCurveIfNotEmpty(ref animClip, kvp.Key, typeof(Transform), "localScale.y", curves[(int)AnimationCurveIndex.LocalScaleY]);
                SetCurveIfNotEmpty(ref animClip, kvp.Key, typeof(Transform), "localScale.z", curves[(int)AnimationCurveIndex.LocalScaleZ]);

                //IsActive curve
                SetCurveIfNotEmpty(ref animClip, kvp.Key, typeof(GameObject), "m_IsActive", curves[(int)AnimationCurveIndex.IsActive]);

                if (kvp.Value.IsSpriteKey)
                {
                    //Color Tint curve
                    SetCurveIfNotEmpty(ref animClip, kvp.Key, typeof(SpriteRenderer), "m_Color.r", curves[(int)AnimationCurveIndex.ColorR]);
                    SetCurveIfNotEmpty(ref animClip, kvp.Key, typeof(SpriteRenderer), "m_Color.g", curves[(int)AnimationCurveIndex.ColorG]);
                    SetCurveIfNotEmpty(ref animClip, kvp.Key, typeof(SpriteRenderer), "m_Color.b", curves[(int)AnimationCurveIndex.ColorB]);
                    SetCurveIfNotEmpty(ref animClip, kvp.Key, typeof(SpriteRenderer), "m_Color.a", curves[(int)AnimationCurveIndex.ColorA]);
                    SetCurveIfNotEmpty(ref animClip, kvp.Key, typeof(SortingOrderUpdate), "SortingOrder", curves[(int)AnimationCurveIndex.ZIndex]);
                }
            }
        }

        public void SetCurveRecursive(Transform root, float time)
        {
            SetCurveRecursive(root, root, time);
        }

        private void SetCurveRecursive(Transform root, Transform current, float time)
        {
            if(root != current)
                SetCurve(root, current, time);
            foreach(Transform child in current.transform)
            {
                SetCurveRecursive(root, child, time);
            }
        }

        public void SetCurve(Transform root, Transform current, float time)
        {
            SetCurve(root, current, time, null);
        }

        public void SetCurve(Transform root, Transform current, float time, TimelineKey lastTimelineKey, int zIndex = -1)
        {
            var path = AnimationUtility.CalculateTransformPath(current, root);
            var curves = GetOrCreateAnimationCurves(path);
            UpdateTransformCurve(curves, current, time, lastTimelineKey, zIndex);
        }

        public void SetCurveActiveOnly(Transform root, Transform current, float time)
        {
            var path = AnimationUtility.CalculateTransformPath(current, root);
            var obj = GetOrCreateAnimationCurves(path);

            //IsActive curve
            float val = (current.gameObject.activeInHierarchy) ? 1.0f : 0.0f;
            obj.Curves[(int)AnimationCurveIndex.IsActive].AddKey(new Keyframe(time, val, float.PositiveInfinity, float.PositiveInfinity) { tangentMode = 0 });
        }

        private void SetCurveIfNotEmpty(ref AnimationClip clip, string path, Type component, string property, AnimationCurve curve)
        {
            if (curve.keys.Length > 0)
            {
                clip.SetCurve(path, component, property, curve);
            }
        }

        private void UpdateTransformCurve(ObjectCurves obj, Transform current, float time, TimelineKey lastTimelineKey, int zIndex = -1)
        {
            float val;
            //IsActive curve
            val = (current.gameObject.activeSelf) ? 1.0f : 0.0f;
            obj.Curves[(int)AnimationCurveIndex.IsActive].AddKey(new Keyframe(time, val, float.PositiveInfinity, float.PositiveInfinity) { tangentMode = 0 });

            //Position curves
            obj.Curves[(int)AnimationCurveIndex.LocalPositionX].AddKey(new Keyframe(time, current.localPosition.x) { tangentMode = 0 }, lastTimelineKey);
            obj.Curves[(int)AnimationCurveIndex.LocalPositionY].AddKey(new Keyframe(time, current.localPosition.y) { tangentMode = 0 }, lastTimelineKey);
            obj.Curves[(int)AnimationCurveIndex.LocalPositionZ].AddKey(new Keyframe(time, current.localPosition.z, float.PositiveInfinity, float.PositiveInfinity)); //Z value always has instant transition

            //Rotation curves
            var quat = Quaternion.Euler(current.localEulerAngles);
            obj.Curves[(int)AnimationCurveIndex.LocalRotationX].AddKey(new Keyframe(time, quat.x) { tangentMode = 0 }, lastTimelineKey);
            obj.Curves[(int)AnimationCurveIndex.LocalRotationY].AddKey(new Keyframe(time, quat.y) { tangentMode = 0 }, lastTimelineKey);
            obj.Curves[(int)AnimationCurveIndex.LocalRotationZ].AddKey(new Keyframe(time, quat.z) { tangentMode = 0 }, lastTimelineKey);
            obj.Curves[(int)AnimationCurveIndex.LocalRotationW].AddKey(new Keyframe(time, quat.w) { tangentMode = 0 }, lastTimelineKey);

            //Scale curves
            obj.Curves[(int)AnimationCurveIndex.LocalScaleX].AddKey(new Keyframe(time, current.localScale.x) { tangentMode = 0 }, lastTimelineKey);
            obj.Curves[(int)AnimationCurveIndex.LocalScaleY].AddKey(new Keyframe(time, current.localScale.y) { tangentMode = 0 }, lastTimelineKey);
            obj.Curves[(int)AnimationCurveIndex.LocalScaleZ].AddKey(new Keyframe(time, current.localScale.z) { tangentMode = 0 }, lastTimelineKey);

            //Sprite Curves
            var spriteTimelineKey = lastTimelineKey as SpriteTimelineKey;
            if (spriteTimelineKey != null)
            {
                obj.IsSpriteKey = true;
                obj.Curves[(int)AnimationCurveIndex.ColorR].AddKey(new Keyframe(time, spriteTimelineKey.Tint.r) { tangentMode = 0 }, lastTimelineKey);
                obj.Curves[(int)AnimationCurveIndex.ColorG].AddKey(new Keyframe(time, spriteTimelineKey.Tint.g) { tangentMode = 0 }, lastTimelineKey);
                obj.Curves[(int)AnimationCurveIndex.ColorB].AddKey(new Keyframe(time, spriteTimelineKey.Tint.b) { tangentMode = 0 }, lastTimelineKey);
                obj.Curves[(int)AnimationCurveIndex.ColorA].AddKey(new Keyframe(time, spriteTimelineKey.Tint.a) { tangentMode = 0 }, lastTimelineKey);
                obj.Curves[(int)AnimationCurveIndex.ZIndex].AddKey(new Keyframe(time, zIndex, float.PositiveInfinity, float.PositiveInfinity));
            }
        }

        private ObjectCurves GetOrCreateAnimationCurves(string path)
        {
            ObjectCurves objCurves;

            if (!curveCache.TryGetValue(path, out objCurves))
            {
                objCurves = new ObjectCurves();
                objCurves.Curves = new AnimationCurve[(int)AnimationCurveIndex.ENUM_COUNT]; 
                for (int i = 0; i < (int) AnimationCurveIndex.ENUM_COUNT; i++)
                {
                    objCurves.Curves[i] = new AnimationCurve();
                }
                curveCache[path] = objCurves;
            }
            return objCurves;
        }

        private enum AnimationCurveIndex
        {
            LocalPositionX,
            LocalPositionY,
            LocalPositionZ,
            LocalRotationX,
            LocalRotationY,
            LocalRotationZ,
            LocalRotationW,
            LocalScaleX,
            LocalScaleY,
            LocalScaleZ,
            IsActive,
            ColorR,
            ColorG,
            ColorB,
            ColorA,
            ZIndex,
            ENUM_COUNT,
        }
    }
}
