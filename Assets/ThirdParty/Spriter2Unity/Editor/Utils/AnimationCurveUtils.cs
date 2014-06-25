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

using Assets.ThirdParty.Spriter2Unity.Editor.Spriter;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Assets.ThirdParty.Spriter2Unity.Editor.Spriter
{
    public static class AnimationCurveUtils
    {
        public const float MIN_DELTA_TIME = 0.001f;
        public const bool ENABLE_KEYFRAME_REDUCATION = true;

        public static void AddKey(this AnimationCurve curve, Keyframe keyframe, TimelineKey lastKey)
        {
            var keys = curve.keys;

            //Early out - if this is the first key on this curve just add it
            if (keys.Length == 0)
            {
                curve.AddKey(keyframe);
                return;
            }

            if (lastKey == null)
            {
                Debug.Log(string.Format("ERROR: NULL lastkey passed to AddKey when curve contains {0} keys", keys.Length));
                return;
            }

            //Get the last keyframe
            Keyframe lastKeyframe = keys[keys.Length - 1];

            //If no TimelineKey is supplied, default to Linear curve
            CurveType curveType = lastKey.CurveType;

            switch (curveType)
            {
                case CurveType.Instant:
                    lastKeyframe.outTangent = 0;
                    curve.MoveKey(keys.Length - 1, lastKeyframe);

                    keyframe.inTangent = float.PositiveInfinity;
                    curve.AddKey(keyframe);
                    break;

                case CurveType.Linear:
                    var val = (keyframe.value - lastKeyframe.value) / (keyframe.time - lastKeyframe.time);
                    lastKeyframe.outTangent = val;
                    curve.MoveKey(keys.Length - 1, lastKeyframe);

                    keyframe.inTangent = val;
                    curve.AddKey(keyframe);
                    break;

                case CurveType.Quadratic:
                    {
                        //Increase to cubic
                        var c1 = (2 * lastKey.CurveParams[0]) / 3;
                        var c2 = 1 - (2 * lastKey.CurveParams[0] + 1) / 3;

                        //Convert [0,1] into unity-acceptable tangents
                        c1 *= 3 * (keyframe.value - lastKeyframe.value) / (keyframe.time - lastKeyframe.time);
                        c2 *= 3 * (keyframe.value - lastKeyframe.value) / (keyframe.time - lastKeyframe.time);

                        //Set the out tangent for the previous frame and update
                        lastKeyframe.outTangent = c1;
                        curve.MoveKey(keys.Length - 1, lastKeyframe);

                        //Set the in tangent for the current frame and add
                        keyframe.inTangent = c2;
                        curve.AddKey(keyframe);
                        break;
                    }

                case CurveType.Cubic:
                    {
                        //Get curve parameters
                        var c1 = lastKey.CurveParams[0];
                        var c2 = 1 - lastKey.CurveParams[1];

                        //Convert [0,1] into unity-acceptable tangents
                        c1 *= 3 * (keyframe.value - lastKeyframe.value) / (keyframe.time - lastKeyframe.time);
                        c2 *= 3 * (keyframe.value - lastKeyframe.value) / (keyframe.time - lastKeyframe.time);

                        //Set the out tangent for the previous frame and update
                        lastKeyframe.outTangent = c1;
                        curve.MoveKey(keys.Length - 1, lastKeyframe);

                        //Set the in tangent for the current frame and add
                        keyframe.inTangent = c2;
                        curve.AddKey(keyframe);
                        break;
                    }

                default:
                    Debug.LogWarning("CurveType " + curveType.ToString() + " not yet supported!");
                    break;
            }
        }

        /// <summary>
        /// Add the specified key and set the in/out tangents for a linear curve
        /// </summary>
        public static void AddLinearKey(this AnimationCurve curve, Keyframe keyframe)
        {
            var keys = curve.keys;
            //Second or later keyframe - make the slopes linear
            if (keys.Length > 0)
            {
                var lastFrame = keys[keys.Length - 1];
                float slope = (keyframe.value - lastFrame.value) / (keyframe.time - lastFrame.time);
                lastFrame.outTangent = keyframe.inTangent = slope;

                //Update the last keyframe
                curve.MoveKey(keys.Length - 1, lastFrame);
            }

            //Add the new frame
            curve.AddKey(keyframe);
        }

        public static void AddKeyIfChanged(this AnimationCurve curve, Keyframe keyframe)
        {
            var keys = curve.keys;
            //If this is the first key on this curve, always add
            //NOTE: Add TWO copies of the first frame, then we adjust the last frame as we move along
            //This guarantees a minimum of two keys in each curve
            if (keys.Length == 0 || !ENABLE_KEYFRAME_REDUCATION)
            {
                curve.AddKey(keyframe);
                keyframe.time += float.Epsilon;
                curve.AddKey(keyframe);
            }
            else
            {
                //TODO: This method of keyframe reduction causes artifacts in animations that are supposed to deliberately pause
                //Find the last keyframe
                Keyframe lastKey = keys[keys.Length - 1];
                if (lastKey.time >= keyframe.time)
                    Debug.LogError("Keyframes not supplied in consecutive order!!!");

                //Grab 2 frames ago
                var last2Key = keys[keys.Length - 2];

                //If the previous 2 frames were different, add a new frame
                if (lastKey.value != last2Key.value)
                {
                    curve.AddKey(keyframe);
                }
                //The previous frame is redundant - just move it
                else
                {
                    curve.MoveKey(keys.Length - 1, keyframe);
                }
            }
        }

        /* Method Signature:        
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void SetAnimationClipSettings(AnimationClip clip, AnimationClipSettings srcClipInfo);
         */
        /// <summary>
        /// Uses reflection to call the internal (seriously, guys?!) SetAnimationClipSettings method
        /// Especially funny because the method doesn't even appear to be USED internally...
        /// </summary>
        public static void SetAnimationSettings(this AnimationClip animClip, AnimationClipSettings settings)
        {
            //Use reflection to get the internal method
            BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.NonPublic;
            MethodInfo mInfo = typeof(AnimationUtility).GetMethod("SetAnimationClipSettings", bindingFlags);
            mInfo.Invoke(null, new object[] { animClip, settings });
        }
    }
}

