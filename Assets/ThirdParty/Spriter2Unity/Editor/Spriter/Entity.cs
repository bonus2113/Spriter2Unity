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
    public class Entity : KeyElem
    {
        public const string XmlKey = "entity";

		public ScmlObject Scml { get; private set;}
        public string Name { get; private set; }
        public IEnumerable<SpriterAnimation> Animations { get { return animations; } }

        public Entity(XmlElement element, ScmlObject scml)
            : base(element)
        { 
			Parse (element, scml);
		}

        protected virtual void Parse(XmlElement element, ScmlObject scml)
		{
			Scml = scml;

            Name = element.GetString("name", "");

            LoadAnimations(element);
        }

        private void LoadAnimations(XmlElement element)
        {
            var animElements = element.GetElementsByTagName(SpriterAnimation.XmlKey);
            foreach (XmlElement animElement in animElements)
            {
                animations.Add(new SpriterAnimation(animElement, this));
            }
        }

        private List<SpriterAnimation> animations = new List<SpriterAnimation>();
    }
}
