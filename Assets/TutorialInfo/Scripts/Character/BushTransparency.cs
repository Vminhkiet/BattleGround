﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BushTransparency : MonoBehaviour
{
    [Tooltip("Chọn layer của các bụi rậm.")]
    [SerializeField] private LayerMask bushLayer;

    [Tooltip("Mức alpha khi nhân vật ở trong bụi rậm.")]
    [SerializeField][Range(0, 1)] private float transparentAlpha = 0.7f;

    private List<Material> characterMeshMaterials;
    private List<Material> characterSkinnedMeshMaterials;

    private int overlappingBushCount = 0;

    void Awake()
    {
        characterSkinnedMeshMaterials = new List<Material>();
        characterMeshMaterials = new List<Material>();

        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (var rend in meshRenderers)
        {
            characterMeshMaterials.AddRange(rend.materials);
        }

        SkinnedMeshRenderer[] skinnedRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var rend in skinnedRenderers)
        {
            characterSkinnedMeshMaterials.AddRange(rend.materials);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((bushLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            overlappingBushCount++;
            UpdateCharacterAlpha();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((bushLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            overlappingBushCount--;
            if (overlappingBushCount < 0)
            {
                overlappingBushCount = 0;
            }
            UpdateCharacterAlpha();
        }
    }

    private void UpdateCharacterAlpha()
    {
        if (overlappingBushCount > 0)
        {
            SetMeshMaterialsAlpha(transparentAlpha);
            SetSkinnedMeshMaterialsAlpha(0f);
        }
        else
        {
            SetMeshMaterialsAlpha(1.0f);
            SetSkinnedMeshMaterialsAlpha(1.0f);
        }
    }

    private void SetMeshMaterialsAlpha(float alpha)
    {
        foreach (var mat in characterMeshMaterials)
        {
            if (alpha < 1.0f)
            {
                mat.SetFloat("_Surface", 1);
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            }
            else
            {
                mat.SetFloat("_Surface", 0);
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                mat.SetInt("_ZWrite", 1);
                mat.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
                mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
            }

            Color color = mat.GetColor("_BaseColor");
            color.a = alpha;
            mat.SetColor("_BaseColor", color);
        }
    }

    private void SetSkinnedMeshMaterialsAlpha(float alpha)
    {
        foreach (var mat in characterSkinnedMeshMaterials)
        {
            if (alpha < 1.0f)
            {
                mat.SetFloat("_Surface", 1);
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            }
            else
            {
                mat.SetFloat("_Surface", 0);
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                mat.SetInt("_ZWrite", 1);
                mat.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
                mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
            }

            Color color = mat.GetColor("_BaseColor");
            color.a = alpha;
            mat.SetColor("_BaseColor", color);
        }
    }
}