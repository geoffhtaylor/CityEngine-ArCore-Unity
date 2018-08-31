//-----------------------------------------------------------------------
// <copyright file="ExamplePreprocessBuild.cs" company="Google">
//
// Copyright 2018 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace GoogleARCore.Examples.CityEngineARTemplate
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using UnityEditor;
    using UnityEditor.Build;
    using UnityEngine;

    internal class PreprocessBuild : IPreprocessBuild
    {
        private readonly List<ExampleScene> k_ExampleScenes = new List<ExampleScene>()
        {
            new ExampleScene()
            {
                ProductName = "CityEngine",
                PackageName = "com.google.ar.core.examples.unity.CityEngineAR",
                SceneGuid = "83fb41cc294e74bdea57537befa00ffc",
                IconGuid = "36944ee986d9cda4fbf5c581de1cefa3"
            },
        };

        [SuppressMessage("UnityRules.UnityStyleRules", "US1000:FieldsMustBeUpperCamelCase",
         Justification = "Overriden property.")]
        public int callbackOrder
        {
            get
            {
                return 0;
            }
        }

        public void OnPreprocessBuild(BuildTarget target, string path)
        {
            BuildTargetGroup buildTargetGroup;
            if (target == BuildTarget.Android)
            {
                buildTargetGroup = BuildTargetGroup.Android;
            }
            else if (target == BuildTarget.iOS)
            {
                buildTargetGroup = BuildTargetGroup.iOS;
            }
            else
            {
                return;
            }

            EditorBuildSettingsScene enabledBuildScene = null;
            int enabledSceneCount = 0;
            foreach (var buildScene in EditorBuildSettings.scenes)
            {
                if (!buildScene.enabled)
                {
                    continue;
                }

                enabledBuildScene = buildScene;
                enabledSceneCount++;
            }

            if (enabledSceneCount != 1)
            {
                return;
            }

            foreach (var exampleScene in k_ExampleScenes)
            {
                if (enabledBuildScene.guid.ToString() == exampleScene.SceneGuid)
                {
                    PlayerSettings.SetApplicationIdentifier(buildTargetGroup, exampleScene.PackageName);
                    PlayerSettings.productName = exampleScene.ProductName;
                    var applicationIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(
                        AssetDatabase.GUIDToAssetPath(exampleScene.IconGuid));

                    var icons = PlayerSettings.GetIconsForTargetGroup(buildTargetGroup, IconKind.Application);
                    for (int i = 0; i < icons.Length; i++)
                    {
                        icons[i] = applicationIcon;
                    }

                    PlayerSettings.SetIconsForTargetGroup(buildTargetGroup, icons, IconKind.Application);
                    break;
                }
            }
        }

        private struct ExampleScene
        {
            public string ProductName;
            public string PackageName;
            public string SceneGuid;
            public string IconGuid;
        }
    }
}
