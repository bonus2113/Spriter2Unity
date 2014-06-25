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
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Assets.ThirdParty.Spriter2Unity.Editor.Spriter
{
    public class ObjectType
    {
        public string Name { get; private set; }

        public static readonly ObjectType INVALID = new ObjectType("INVALID");
        public static readonly ObjectType Point = new ObjectType("Point");
        public static readonly ObjectType Box = new ObjectType("Box");
        public static readonly ObjectType Sprite = new ObjectType("Sprite");
        public static readonly ObjectType Sound = new ObjectType("Sound");
        public static readonly ObjectType Entity = new ObjectType("Entity");
        public static readonly ObjectType Variable = new ObjectType("Variable");

        private ObjectType(string name)
        {
            Name = name;
        }
        public static ObjectType Parse(XmlElement element)
        {
            ObjectType value;
            string objectType = element.GetString("object_type", "sprite");
            switch (objectType)
            {
                case "point":
                    value = ObjectType.Point;
                    break;
                case "box":
                    value = ObjectType.Box;
                    break;
                case "sprite":
                    value = ObjectType.Sprite;
                    break;
                case "sound":
                    value = ObjectType.Sound;
                    break;
                case "entity":
                    value = ObjectType.Entity;
                    break;
                case "variable":
                    value = ObjectType.Variable;
                    break;
                default:
                    value = ObjectType.INVALID;
                    break;
            }

            return value;
        }
    }
}
