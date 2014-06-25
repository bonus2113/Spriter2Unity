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
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Assets.ThirdParty.Spriter2Unity.Editor.Unity
{
    public class ScmlPostProcessor : AssetPostprocessor
    {
        //HACK: Currently no known way to get the path of this script file from Unity
        const string ASSET_PATH = "Spriter2Unity/Editor/Unity/ScmlPostProcessor.cs";

        static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            //Reimport everything if the importer itself has been modified or added
            //.Union(deletedAssets).Union(movedAssets).Union(movedFromAssetPaths)
            bool shouldReimportAll = importedAssets.Where(s => s.EndsWith(ASSET_PATH)).FirstOrDefault() != null;

            //If we should reimport all SCML files, replace the passed in array with ALL scml project files
            if(shouldReimportAll)
            {
                Debug.Log("Reimporting all SCML files in project...");
                importedAssets = AssetDatabase.GetAllAssetPaths().Where(assetPath => assetPath.EndsWith(".scml")).ToArray();
            }
            
            foreach (var path in importedAssets)
            {
                if (!path.EndsWith(".scml"))
                    continue;

                ImportScml(path);
            }
        }

        static void ImportScml(string assetPath)
        {
            string folderPath = Path.GetDirectoryName(assetPath);

            //Load the SCML as XML
            var doc = new XmlDocument();
            doc.Load(assetPath);

            //Parse the SCML file
            var scml = new Spriter.ScmlObject(doc);

            //TODO: Verify that all files/folders exist
            var pb = new PrefabBuilder();
            foreach (var entity in scml.Entities)
            {
                //TODO: Settings file to customize prefab location
                var prefabPath = Path.Combine(folderPath, entity.Name + ".prefab");

                //Change to forward slash for asset database friendliness
                prefabPath = prefabPath.Replace('\\', '/');

                //Either instantiate the existing prefab or create a new one
                GameObject go;
                var prefabGo = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
                if (prefabGo == null)
                {
                    go = new GameObject();
                    prefabGo = PrefabUtility.CreatePrefab(prefabPath, go, ReplacePrefabOptions.ConnectToPrefab);
                }
                else
                {
                    go = GameObject.Instantiate(prefabGo) as GameObject;

                    var oldAnimator = go.GetComponent<Animator>();
                    if (oldAnimator) GameObject.DestroyImmediate(oldAnimator);
                }

                //Build the prefab based on the supplied entity
                pb.MakePrefab(entity, go, folderPath);

                var animator = go.AddComponent<Animator>();

                

                //Add animations to prefab object
                var anim = new AnimationBuilder();
                var allAnimClips = anim.BuildAnimationClips(go, entity, prefabPath);
                AssetDatabase.SaveAssets();

                var animatorControllerPath = Path.ChangeExtension(prefabPath, "controller");
                var oldController = (AnimatorController)AssetDatabase.LoadAssetAtPath(animatorControllerPath, typeof (AnimatorController));
                var controller = oldController;

                if (!oldController)
                {
                    controller = AnimatorController.CreateAnimatorControllerAtPath(animatorControllerPath);
                    foreach (var animationClip in allAnimClips)
                    {
                        if (animationClip)
                        {
                            AnimatorController.AddAnimationClipToController(controller, animationClip);
                        }
                    }
                }
                AnimatorController.SetAnimatorController(animator, controller);
                go.SetActive(true);
                //Update the prefab
                PrefabUtility.ReplacePrefab(go, prefabGo, ReplacePrefabOptions.ConnectToPrefab);
                
                //Add a generic avatar - because why not?
                //TODO: May need to eventually break this into a separate class
                //  ie: if we want to look for a root motion node by naming convention
                //var avatar = AvatarBuilder.BuildGenericAvatar(go, "");
                //avatar.name = go.name;
                //AssetDatabase.AddObjectToAsset(avatar, prefabPath);

                GameObject.DestroyImmediate(go);

                AssetDatabase.SaveAssets();
            }
        }
        
    }
}
