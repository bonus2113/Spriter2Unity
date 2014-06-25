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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Assets.ThirdParty.Spriter2Unity.Editor.Spriter
{
    public class SpatialInfo
    {
        public Vector2 Position { get; private set; }
        public Vector2 Scale { get; private set; }
        public float Angle_Deg { get; private set; }
        public float Angle
        {
            get
            {
                return Mathf.Deg2Rad * Angle_Deg;
            }
        }
        public SpinDirection Spin { get; private set; }

        public SpatialInfo(XmlElement element)
        {
            Parse(element);
        }

        private SpatialInfo()
        { }

        public SpatialInfo Unmap(SpatialInfo parent)
        {
            if (parent == null)
                return this;

			var unmapped = new SpatialInfo();

			unmapped.Position = new Vector2(
				Position.x * parent.Scale.x,
				Position.y * parent.Scale.y);
			unmapped.Scale = new Vector2(
				Scale.x * parent.Scale.x,
				Scale.y * parent.Scale.y);
            unmapped.Angle_Deg = Angle_Deg;
            unmapped.Spin = Spin;

            return unmapped;
        }

        protected virtual void Parse(XmlElement element)
        {
            Vector2 position;
            position.x = element.GetFloat("x", 0.0f);
            position.y = element.GetFloat("y", 0.0f);
            Position = position;

            Vector2 scale = Vector2.one;
            scale.x = element.GetFloat("scale_x", 1.0f);
            scale.y = element.GetFloat("scale_y", 1.0f);
            Scale = scale;

            Angle_Deg = element.GetFloat("angle", 0.0f);

            int spinVal = element.GetInt("spin", 1);
            Spin = (spinVal == -1) ? SpinDirection.Clockwise : SpinDirection.CounterClockwise;
        }
    }
}
