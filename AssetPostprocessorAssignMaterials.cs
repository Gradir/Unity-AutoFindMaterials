/*
AutoFindMaterials - an asset postprocessor for 3D model assets
it searches the project for materials first getting rid of the suffix
default separator is dot - because Blender's default auto-naming is *.001, *.002 etc

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

    public override int GetPostprocessOrder()
    {
        return 100;
    }

    void OnPreprocessMaterialDescription(MaterialDescription description, Material material, AnimationClip[] animations)
    {
        var importer = (ModelImporter)assetImporter;
        // Easy way of cleaning up previously assigned materials
        importer.materialImportMode = ModelImporterMaterialImportMode.None;
        importer.materialImportMode = ModelImporterMaterialImportMode.ImportViaMaterialDescription;
        importer.AddRemap(new AssetImporter.SourceAssetIdentifier(material), GetMaterial(material.name));
    }

    Material GetMaterial(string materialNameInFbx)
    {
        var truncatedName = materialNameInFbx.Contains(Dot)? materialNameInFbx.Substring(0, materialNameInFbx.LastIndexOf(Dot)) : materialNameInFbx;
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
