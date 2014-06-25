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
    public class BoneTimelineKey : SpatialTimelineKey
    {
        new public const string XmlKey = "bone";

        public Color Tint { get; private set; }

        public BoneTimelineKey(XmlElement element, Timeline timeline)
            :base(element, timeline)
        {        }

        protected override void Parse(XmlElement element, Timeline timeline)
        {
            base.Parse(element, timeline);

            var boneElem = element["bone"];
            Spatial = new SpatialInfo(boneElem);

            Color tint = Color.white;
            tint.r = boneElem.GetFloat("r", 1.0f);
            tint.g = boneElem.GetFloat("g", 1.0f);
            tint.b = boneElem.GetFloat("b", 1.0f);
            tint.a = boneElem.GetFloat("a", 1.0f);
            Tint = tint;
        }
    }
}
