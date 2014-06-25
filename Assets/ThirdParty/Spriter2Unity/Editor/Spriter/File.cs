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
using UnityEditor;

namespace Assets.ThirdParty.Spriter2Unity.Editor.Spriter
{
    public enum FileType
    {
        INVALID_TYPE,
        Image,
        SoundEffect,
        AtlasImage,
        Entity,
    }

    public class File : KeyElem
    {
        public const string XmlKey = "file";

        public File(XmlElement element, Folder folder)
            :base(element)
        {
            Parse(element, folder);
        }

        public Folder Folder { get; private set; }
        public FileType FileType { get; private set; }
        public string Name { get; private set; }
        public Vector2 Pivot { get; private set; }
        public Vector2 Size { get; private set; }
        public Vector2 Offset { get; private set; }
        public Vector2 OriginalSize { get; private set; }

        protected virtual void Parse(XmlElement element, Folder folder)
        {
            Folder = folder;

            var type = element.GetString("type", "image");
            switch(type)
            {
                case "image":
                    FileType = FileType.Image;
                    break;
                case "atlas_image":
                    FileType = FileType.AtlasImage;
                    break;
                case "sound_effect":
                    FileType = FileType.SoundEffect;
                    break;
                case "entity":
                    FileType = FileType.Entity;
                    break;
                default:
                    FileType = FileType.INVALID_TYPE;
                    break;
            }

            Name = element.GetString("name", "");

            Vector2 pivot;
            pivot.x = element.GetFloat("pivot_x", 0.0f);
            pivot.y = element.GetFloat("pivot_y", 0.0f);
            Pivot = pivot;

            Vector2 size;
            size.x = element.GetInt("width", 0);
            size.y = element.GetInt("height", 0);
            Size = size;

            Vector2 offset;
            offset.x = element.GetInt("offset_x", 0);
            offset.y = element.GetInt("offset_y", 0);
            Offset = offset;

            Vector2 originalSize;
            originalSize.x = element.GetInt("original_width", 0);
            originalSize.y = element.GetInt("original_height", 0);
            OriginalSize = originalSize;
        }

#if UNITY_EDITOR
        private string FolderName
        {
            get
            {
                return Name.Substring(0, Name.IndexOf('/') - 1);
            }
        }

        private string ImageName
        {
            get
            {
                return Name.Substring(Name.IndexOf('/')+1);
            }
        }

        public Vector2 GetPivotOffetFromMiddle()
        {
            var mid = Size / 2;
            var pvt = new Vector2(
                Size.x * Pivot.x,
                Size.y * Pivot.y);

            return mid - pvt;
        }
#endif
    }
}
