/*
Copyright (c) 2014 Andrew Jones
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
using System.Linq;
using System.Collections;
using System.Xml;
using UnityEditor;
using Assets.ThirdParty.Spriter2Unity.Editor.Spriter;
using System.Reflection;

namespace Assets.ThirdParty.Spriter2Unity.Editor.Spriter
{
    public static class AssetUtils
    {
        public static Sprite GetSpriteAtPath(string filePath, string spriteFolder)
        {
            Sprite sprite = null;

            if (string.IsNullOrEmpty(spriteFolder))
            {
                var assetPath = AssetDatabase.GetAllAssetPaths().Where(path => path.EndsWith(filePath)).FirstOrDefault();
                if (!string.IsNullOrEmpty(assetPath))
                {
                    sprite = (Sprite)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Sprite));
                }
            }
            else
            {
                var assetPath = System.IO.Path.Combine(spriteFolder, filePath);
                sprite = (Sprite)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Sprite));
            }
            return sprite;
        }
    }
}