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
    public class ScmlObject
    {
        public string Version { get; private set; }
        public string Generator { get; private set; }
        public string GeneratorVersion { get; private set; }
        public bool PixelArtMode { get; private set; }
        public IEnumerable<Folder> Folders { get { return folders; } }
        public IEnumerable<Entity> Entities { get { return entities; } }

        public ScmlObject(XmlDocument doc)
        {
            Parse(doc["spriter_data"]);
        }

        protected virtual void Parse(XmlElement element)
        {
            Version = element.GetString("element", "UNKNOWN");
            Generator = element.GetString("generator", "UNKNOWN");
            GeneratorVersion = element.GetString("generator_version", "UNKNOWN");

            string pixelArt = element.GetString("pixel_art_mode", "false");
            PixelArtMode = pixelArt == "true";

            LoadFolders(element);
            LoadEntities(element);
        }

        private void LoadFolders(XmlElement element)
        {
            var folderElems = element.GetElementsByTagName(Folder.XmlKey);
            foreach (XmlElement folderElem in folderElems)
            {
                folders.Add(new Folder(folderElem));
            }
        }

        private void LoadEntities(XmlElement element)
        {
            var entityElems = element.GetElementsByTagName(Entity.XmlKey);
            foreach (XmlElement entityElem in entityElems)
            {
                entities.Add(new Entity(entityElem, this));
            }
        }

        public Folder GetFolder(int id)
        {
            return folders.Where(folder => folder.Id == id).FirstOrDefault();
        }

        private List<Folder> folders = new List<Folder>();
        private List<Entity> entities = new List<Entity>();
    }
}
