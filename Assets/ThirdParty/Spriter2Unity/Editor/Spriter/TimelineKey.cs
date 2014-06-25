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
    /// <summary>
    /// Base class for all timeline keys.
    /// </summary>
    public class TimelineKey : Key
    {
        /// <summary>
        /// Reference to the timeline the key is on.
        /// </summary>
        public Timeline Timeline { get; private set; }

        /// <summary>
        /// The interpolation type to use for this key.
        /// </summary>
        public CurveType CurveType { get; private set; }

        /// <summary>
        /// Parameters to use for interpolation (quadratic and qubic at the moment)
        /// </summary>
        public float[] CurveParams { get; private set; }

        public TimelineKey(XmlElement element, Timeline timeline)
            : base(element)
        {
            Parse(element, timeline);
        }

        protected virtual void Parse(XmlElement element, Timeline timeline)
        {
            string curveString = element.GetString("curve_type", "linear");
            switch (curveString)
            {
                case "instant":
                    CurveType = Spriter.CurveType.Instant;
                    break;
                case "linear":
                    CurveType = Spriter.CurveType.Linear;
                    break;
                case "quadratic":
                    CurveType = Spriter.CurveType.Quadratic;
                    break;
                case "cubic":
                    CurveType = Spriter.CurveType.Cubic;
                    break;
                case "quartic":
                    CurveType = Spriter.CurveType.Quartic;
                    break;
                case "quintic":
                    CurveType = Spriter.CurveType.Quintic;
                    break;
                default:
                    CurveType = Spriter.CurveType.INVALID;
                    break;
            }

            Timeline = timeline;

            GetCurveParams(element);
        }

        void GetCurveParams(XmlElement element)
        {
            //Get curve parameters using a bit of XPath, order using LINQ
            //XPath 1.0 doesn't support regex - should match all attributes with names matching "c[0-9]+"
            var curveParams = element.SelectNodes("@*[starts-with(name(), 'c') and string(number(substring(name(),2))) != 'NaN']")
                .OfType<XmlAttribute>()
                .OrderBy(attr => attr.Name);

            //Cast the values to float and convert to an array
            CurveParams = curveParams
                .Select(attr => float.Parse(attr.Value))
                .ToArray();
        }
    }
}
