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
    public enum CurveType
    {
        INVALID,
        Instant,
        Linear,
        Quadratic,
        Cubic,
        Quartic,
        Quintic,
    }

    public enum SpinDirection{
        Clockwise = -1,
        CounterClockwise = 1
    }

    public class MainlineKey : Key
    {
        public IEnumerable<Ref> Refs { get { return refs; } }
        public IEnumerable<BoneRef> BoneRefs { get { return refs.OfType<BoneRef>(); } }
        public IEnumerable<ObjectRef> ObjectRefs { get { return refs.OfType<ObjectRef>(); } }

        public MainlineKey(XmlElement element, SpriterAnimation animation)
            : base(element)
        {
            Parse(element, animation);
        }
        
        protected virtual void Parse(XmlElement element, SpriterAnimation animation)
        {
            //Get elements
            //TODO: Ensure proper ordering of elements to prevent dependency errors
            var children = element.ChildNodes;
            foreach(XmlNode child in children)
            {
                XmlElement childElement = child as XmlElement;
                if(childElement != null)
                {
                    switch(childElement.Name)
                    {
                        case BoneRef.XmlKey:
                            refs.Add(new BoneRef(childElement, animation, this));
                            break;
                        case ObjectRef.XmlKey:
                            refs.Add(new ObjectRef(childElement, animation, this));
                            break;
                    }
                }
            }
        }

        public BoneRef GetBoneRef(int id)
        {
            return refs.Where(bone => bone.Id == id).OfType<BoneRef>().FirstOrDefault();
        }

        public ObjectRef GetObjectRef(int id)
        {
            return refs.Where(obj => obj.Id == id).OfType<ObjectRef>().FirstOrDefault();
        }

        public IEnumerable<Ref> GetChildren(Ref parent)
        {
            return refs.Where(obj => obj.Parent == parent);
        }

        private List<Ref> refs = new List<Ref>();
    }
}
