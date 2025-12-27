/*
AutoFindMaterials - an asset postprocessor for 3D model assets
it searches the project for materials first getting rid of the suffix
default separator is dot - because Blender's default auto-naming is *.001, *.002 etc

model's import settings should be set to "Import via MaterialDescription"
(consider making it a Unity Preset and set it as default in Project Settings)
"Import" action on a model asset will run this postprocessor

Made by Piotr "Gradir" Kołodziejczyk (gradir@gmail.com)
*/
using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;

public class AssetPostprocessorAssignMaterials : AssetPostprocessor
{
    readonly string[] _pathToSearch = { "Assets/Graphics" };
    const string MaterialString = "t:material ";
    const char Dot = '.';

    void OnPreprocessModel()
    {
        var importer = (ModelImporter)assetImporter;
        var existingRemaps = importer.GetExternalObjectMap();
        foreach (var kvp in existingRemaps)
            importer.RemoveRemap(kvp.Key);
    }

    void OnPreprocessMaterialDescription(MaterialDescription description, Material material, AnimationClip[] animations)
    {
        ((ModelImporter)assetImporter).AddRemap(new AssetImporter.SourceAssetIdentifier(material), GetMaterial(material.name));
    }

    Material GetMaterial(string materialNameInFbx)
    {
        var truncatedName = materialNameInFbx.Substring(0, materialNameInFbx.LastIndexOf(Dot));
        Debug.Log(truncatedName);
        var guids = AssetDatabase.FindAssets(MaterialString + truncatedName, _pathToSearch);
        for (var i = 0; i < guids.Length; i++)
        {
            var asset = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guids[i]));
            if (PrefabUtility.IsPartOfAnyPrefab(asset) || asset.name != truncatedName)
                continue;
            return asset;
        }

        return null;
    }
}
