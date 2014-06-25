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

using System.Xml;
using UnityEngine;

namespace Assets.ThirdParty.Spriter2Unity.Editor.Spriter
{
    public static class XmlUtils
    {
        public static bool TryGetString(this XmlNode node, string key, out string value)
        {
            value = default(string);
            var attr = node.Attributes[key];
            bool parsed = false;
            if (attr != null)
            {
                parsed = true;
                value = attr.Value;
            }

            return parsed;
        }

        public static bool TryGetInt(this XmlNode node, string key, out int value)
        {
            value = default(int);
            var attr = node.Attributes[key];
            bool parsed = false;
            if (attr != null)
            {
                parsed = int.TryParse(attr.Value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out value);
            }

            return parsed;
        }

        public static bool TryGetFloat(this XmlNode node, string key, out float value)
        {
            value = default(float);
            var attr = node.Attributes[key];
            bool parsed = false;
            if (attr != null)
            {
                parsed = float.TryParse(attr.Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out value);
            }

            return parsed;
        }

        public static bool TryGetVector2(this XmlNode node, out Vector2 value)
        {
            value = default(Vector2);
            float tmp;
            bool parsed = true;
            if (TryGetFloat(node, "x", out tmp)) value.x = tmp;
            else parsed = false;
            if (TryGetFloat(node, "y", out tmp)) value.y = tmp;
            else parsed = false;

            return parsed;
        }

        public static bool TryGetVector3(this XmlNode node, out Vector3 value)
        {
            value = default(Vector3);
            float tmp;
            bool parsed = true;
            if (TryGetFloat(node, "x", out tmp)) value.x = tmp;
            else parsed = false;
            if (TryGetFloat(node, "y", out tmp)) value.y = tmp;
            else parsed = false;
            if (TryGetFloat(node, "z", out tmp)) value.z = tmp;
            else parsed = false;

            return parsed;
        }

        public static string GetString(this XmlNode node, string key, string defaultVal)
        {
            string value = defaultVal;
            var attr = node.Attributes[key];
            if (attr != null)
            {
                value = attr.Value;
            }
            return value;
        }

        public static int GetInt(this XmlNode node, string key, int defaultVal)
        {
            int value = defaultVal;
            int tmp;
            if (TryGetInt(node, key, out tmp)) value = tmp;

            return value;
        }

        public static float GetFloat(this XmlNode node, string key, float defaultVal)
        {
            var value = defaultVal;
            float tmp;
            if (TryGetFloat(node, key, out tmp)) value = tmp;

            return value;
        }

        public static Vector2 GetVector2(this XmlNode node, Vector2 defaultVal)
        {
            var value = defaultVal;
            float tmp;
            if (TryGetFloat(node, "x", out tmp)) value.x = tmp;
            if (TryGetFloat(node, "y", out tmp)) value.y = tmp;

            return value;
        }

        public static Vector3 GetVector3(this XmlNode node)
        {
            var value = default(Vector3);
            float tmp;
            if (TryGetFloat(node, "x", out tmp)) value.x = tmp;
            if (TryGetFloat(node, "y", out tmp)) value.y = tmp;
            if (TryGetFloat(node, "z", out tmp)) value.z = tmp;

            return value;
        }
    }
}

