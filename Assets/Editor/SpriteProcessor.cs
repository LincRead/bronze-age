using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class SpriteProcessor : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        TextureImporter ti = (TextureImporter)assetImporter;
        ti.textureType = TextureImporterType.Sprite;
        ti.spriteImportMode = SpriteImportMode.Multiple;
        ti.spritePixelsPerUnit = 100;
        ti.mipmapEnabled = false;
        ti.filterMode = FilterMode.Point;
    }
}