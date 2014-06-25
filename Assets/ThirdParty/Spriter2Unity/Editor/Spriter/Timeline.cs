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

namespace Assets.ThirdParty.Spriter2Unity.Editor.Spriter
{
    public class Timeline : KeyElem
    {
        public const string XmlKey = "timeline";

		public SpriterAnimation Animation { get; private set; }
        public string Name { get; private set; }
        public ObjectType ObjectType { get; private set; }
        public IEnumerable<TimelineKey> Keys { get{return keys;} }

        public Timeline(XmlElement element, SpriterAnimation animation)
            :base(element)
        {		
			Parse (element, animation);
		}

        protected virtual void Parse(XmlElement element, SpriterAnimation animation)
		{
			Animation = animation;
            Name = element.GetString("name", "");

            ObjectType = ObjectType.Parse(element);

            var children = element.GetElementsByTagName(TimelineKey.XmlKey);
            foreach (XmlElement childElement in children)
            {
                keys.Add(GetKey(childElement));
            }
        }

        public TimelineKey GetKey(int id)
        {
            return Keys.Where(key => key.Id == id).FirstOrDefault();
        }

        private TimelineKey GetKey(XmlElement element)
        {
            //Check if key is sprite or bone
            var bone = element[BoneTimelineKey.XmlKey];
            if(bone != null)
            {
                return new BoneTimelineKey(element, this);
            }
            else
            {
                var obj = element[SpriteTimelineKey.XmlKey];
                if(obj != null)
                {
                    var objType = ObjectType.Parse(obj);
                    if (objType == ObjectType.Sprite)
                    {
                        return new SpriteTimelineKey(element, this);
                    }
                }
            }
            return null;
        }
        private List<TimelineKey> keys = new List<TimelineKey>();
    }
}
