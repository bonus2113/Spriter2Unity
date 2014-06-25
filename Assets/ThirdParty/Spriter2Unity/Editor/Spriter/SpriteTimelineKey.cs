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
    public class SpriteTimelineKey : SpatialTimelineKey
    {
        new public const string XmlKey = "object";

        public File File { get; private set; }
        public Vector2 Pivot { get; private set; }
        public Color Tint { get; private set; }

        public Vector2 GetPivotOffetFromMiddle()
        {
            var mid = File.Size / 2;
            var pvt = new Vector2(
                File.Size.x * Pivot.x,
                File.Size.y * Pivot.y);

            var retVal = mid - pvt;
            return retVal;
        }

        public SpriteTimelineKey(XmlElement element, Timeline timeline)
            : base(element, timeline)
        { }

        protected override void Parse(XmlElement element, Timeline timeline)
        {
            base.Parse(element, timeline);

            var objElement = element[XmlKey];

            File = GetFile(objElement);

            Spatial = new SpatialInfo(objElement);

            Vector2 pivot;
            pivot.x = objElement.GetFloat("pivot_x", File.Pivot.x);
            pivot.y = objElement.GetFloat("pivot_y", File.Pivot.y);
            Pivot = pivot;

            Color tint = Color.white;
            tint.r = objElement.GetFloat("r", 1.0f);
            tint.g = objElement.GetFloat("g", 1.0f);
            tint.b = objElement.GetFloat("b", 1.0f);
            tint.a = objElement.GetFloat("a", 1.0f);
            Tint = tint;
        }

        File GetFile(XmlElement element)
        {
            var folderId = element.GetInt("folder", -1);
            var fileId = element.GetInt("file", -1);

            File file = null;
            var folder = Timeline.Animation.Entity.Scml.GetFolder(folderId);
            if (folder != null)
            {
                file = folder.GetFile(fileId);
                if (file == null)
                {
                    Debug.LogError(string.Format("File Not Found! folder: {0}   file: {1}", folderId, fileId));
                }
            }
            else
            {
                Debug.LogError(string.Format("Folder Not Found!  folder: {0}", folderId));
            }
            return file;
        }
    }
}
