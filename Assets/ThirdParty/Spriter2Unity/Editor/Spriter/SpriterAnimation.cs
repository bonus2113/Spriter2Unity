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
    public enum LoopType
    {
        INVALID,
        True,
        False,
        PingPong,
    }

    public static class LoopTypeUtils
    {
        public static LoopType Parse(XmlElement element)
        {
            var looping = element.GetString("looping", "true");
            switch (looping)
            {
                case "true":
                    return LoopType.True;
                case "false":
                    return LoopType.False;
                case "ping_pong":
                    return LoopType.PingPong;
            }
            return LoopType.INVALID;
        }
    }

    public class SpriterAnimation : KeyElem
    {
        public const string XmlKey = "animation";

		public Entity Entity {get;private set;}
        public string Name { get; private set; }
        public int Length_Ms { get; private set; }
        public float Length { get { return ((float)Length_Ms) / 1000; } }
        public LoopType LoopType { get; private set; }
        public int LoopTo { get; private set; }
        public IEnumerable<MainlineKey> MainlineKeys { get { return mainlineKeys; } }
        public IEnumerable<Timeline> Timelines { get { return timelines; } }

        public SpriterAnimation(XmlElement element, Entity entity)
            : base(element)
        { 
			Parse (element, entity);
		}

        public Timeline GetTimeline(int id)
        {
            return Timelines.Where(timeline => timeline.Id == id).FirstOrDefault();
        }

        protected virtual void Parse(XmlElement element, Entity entity)
		{
			Entity = entity;

            Name = element.GetString("name", "");
            Length_Ms = element.GetInt("length", -1);
            LoopType = LoopTypeUtils.Parse(element);
            LoopTo = element.GetInt("loop_to", 0);

            LoadTimelines(element);
            LoadMainline(element);
        }

        private void LoadMainline(XmlElement element)
        {
            var mainlineElem = element["mainline"];
            var mainlineKeyElems = mainlineElem.GetElementsByTagName(Key.XmlKey);
            foreach (XmlElement mainlineKeyElem in mainlineKeyElems)
            {
                mainlineKeys.Add(new MainlineKey(mainlineKeyElem, this));
            }
        }

        private void LoadTimelines(XmlElement element)
        {
            var timelineElems = element.GetElementsByTagName(Timeline.XmlKey);
            foreach(XmlElement timelineElem in timelineElems)
            {
                timelines.Add(new Timeline(timelineElem, this));
            }
        }

        private List<MainlineKey> mainlineKeys = new List<MainlineKey>();
        private List<Timeline> timelines = new List<Timeline>();
    }
}
