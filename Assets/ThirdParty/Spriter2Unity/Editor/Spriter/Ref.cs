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
using System.Xml;
using UnityEngine;

namespace Assets.ThirdParty.Spriter2Unity.Editor.Spriter
{
    public class Ref : KeyElem
    {
        public MainlineKey MainlineKey { get; private set; }
        protected SpatialInfo unmapped;
        public TimelineKey Referenced { get; private set; }
        public BoneRef Parent { get; private set; }
        public string RelativePath
        {
            get
            {
                if(Parent == null)
                {
                    return Referenced.Timeline.Name;
                }
                else
                {
                    return Parent.RelativePath + "/" + Referenced.Timeline.Name;
                }
            }
        }

        public SpatialInfo Unmapped
        {
            get
            {
                if (unmapped == null)
                {
                    unmapped = ComputeUnmapped();
                }
                return unmapped;
            }
        }

        private SpatialInfo ComputeUnmapped()
        {
            SpatialInfo spatialInfo = null;
            var spatial = Referenced as SpatialTimelineKey;
            if (spatial != null)
            {
                spatialInfo = spatial.Spatial;
            }
            else
            {
                Debug.LogError("Non-Spatial Ref type!!");
            }

            if (Parent != null)
			{
				spatialInfo = spatialInfo.Unmap (Parent.Unmapped);
            }

            return spatialInfo;
        }

        public Ref(XmlElement element, SpriterAnimation animation, MainlineKey parentKey)
            :base(element)
        {
            MainlineKey = parentKey;
            Parse(element, animation, parentKey);
        }

        private void Parse(XmlElement element, SpriterAnimation animation, MainlineKey parentKey)
        {
            Referenced = GetTimelineKey(element, animation);

            int parentId = element.GetInt("parent", -1);
            if(parentId >= 0)
            {
                Parent = parentKey.GetBoneRef(parentId);
            }
        }

        protected TimelineKey GetTimelineKey(XmlElement element, SpriterAnimation animation)
        {
            int timeline = element.GetInt("timeline", 0);
            int key = element.GetInt("key", 0);

            var timelineObj = animation.GetTimeline(timeline);
            if (timelineObj == null)
            {
                Debug.LogError(String.Format(
                    System.Globalization.CultureInfo.InvariantCulture,
                    "Unable to find timeline {0} in animation {1}",
                    timeline,
                    animation.Id));
            }
            return timelineObj.GetKey(key);
        }
    }
}
