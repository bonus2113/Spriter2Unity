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

using UnityEngine;

namespace Assets.ThirdParty.Spriter2Unity.Editor.Spriter
{
    public static class PrefabUtils
    {
        // TODO: Read this from the sprite meta file.
        public const float PIXEL_SCALE = 0.01f;
        public const float Z_SPACING = 0.1f;

        public static void BakeTransforms(this Ref childRef, out Vector3 localPosition, out Vector3 localEulerAngles, out Vector3 localScale)
        {
            TimelineKey key = childRef.Referenced;

            localPosition = Vector3.zero;
            localScale = Vector3.one;
            localEulerAngles = Vector3.zero;

            var unmapped = childRef.Unmapped;
            var spatial = childRef.Referenced as SpatialTimelineKey;
            if (spatial != null)
            {
                localPosition = unmapped.Position;

                var spriteKey = key as SpriteTimelineKey;
                if (spriteKey != null)
                {
                    var sinA = Mathf.Sin(unmapped.Angle);
                    var cosA = Mathf.Cos(unmapped.Angle);

                    var pvt = spriteKey.GetPivotOffetFromMiddle();

                    pvt.x *= unmapped.Scale.x;
                    pvt.y *= unmapped.Scale.y;

                    var rotPX = pvt.x * cosA - pvt.y * sinA;
                    var rotPY = pvt.x * sinA + pvt.y * cosA;

                    localPosition.x += rotPX;
                    localPosition.y += rotPY;

                    localScale.x = unmapped.Scale.x;
                    localScale.y = unmapped.Scale.y;
                    localPosition.z = ((ObjectRef)childRef).ZIndex * -Z_SPACING;
                }

                localPosition *= PIXEL_SCALE;
                localEulerAngles = new Vector3(0, 0, unmapped.Angle_Deg);
            }
        }
    }
}