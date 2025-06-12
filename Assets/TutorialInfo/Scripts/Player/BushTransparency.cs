using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BushTransparency : MonoBehaviour
{
    [Range(0f, 1f)] public float targetAlpha = 0.4f;
    public float fadeDuration = 0.3f;

    private List<Material> allMaterials = new List<Material>();
    private List<Color> originalColors = new List<Color>();
    private Coroutine fadeRoutine;

    private void Start()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer rend in renderers)
        {
            // Lấy một bản sao của các material để không thay đổi file gốc
            Material[] instanceMats = rend.materials;

            for (int i = 0; i < instanceMats.Length; i++)
            {
                Material mat = instanceMats[i];

                // Đảm bảo không xử lý trùng lặp nếu nhiều renderer dùng chung 1 material instance
                if (!allMaterials.Contains(mat))
                {
                    SetupURPMaterialForTransparency(mat);
                    allMaterials.Add(mat);

                    Color c = mat.HasProperty("_BaseColor") ? mat.GetColor("_BaseColor") : mat.color;
                    originalColors.Add(c);

                    // Bạn có thể không cần set alpha về 0 ở đây nếu muốn nhân vật hiện rõ lúc bắt đầu
                    // c.a = 0f; 
                    // SetMaterialAlpha(mat, c);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Giả sử player của bạn có layer là "Player" và nó va chạm với "Bush"
        if (other.gameObject.layer == LayerMask.NameToLayer("Bush")) // Thay "Player" bằng tag của player
        {
            if (fadeRoutine != null) StopCoroutine(fadeRoutine);
            fadeRoutine = StartCoroutine(FadeAlphaTo(targetAlpha));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Bush")) 
        {
            if (fadeRoutine != null) StopCoroutine(fadeRoutine);
            fadeRoutine = StartCoroutine(FadeAlphaTo(0f));
        }
    }

    private IEnumerator FadeAlphaTo(float target)
    {
        float t = 0f;
        float duration = Mathf.Max(0.01f, fadeDuration);

        List<float> startAlphas = new List<float>();
        foreach (var mat in allMaterials)
        {
            Color current = mat.HasProperty("_BaseColor") ? mat.GetColor("_BaseColor") : mat.color;
            startAlphas.Add(current.a);
        }

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            for (int i = 0; i < allMaterials.Count; i++)
            {
                Color baseColor = originalColors[i];
                float a = Mathf.Lerp(startAlphas[i], target, t);
                baseColor.a = a;
                SetMaterialAlpha(allMaterials[i], baseColor);
            }
            yield return null;
        }

        // Đảm bảo giá trị cuối cùng được đặt chính xác
        for (int i = 0; i < allMaterials.Count; i++)
        {
            Color final = originalColors[i];
            final.a = target;
            SetMaterialAlpha(allMaterials[i], final);
        }
    }

    private void SetMaterialAlpha(Material mat, Color color)
    {
        if (mat.HasProperty("_BaseColor"))
        {
            mat.SetColor("_BaseColor", color);
        }
        else
        {
            mat.color = color;
        }
    }

    private void SetupURPMaterialForTransparency(Material mat)
    {
        if (!mat.shader.name.Contains("Universal Render Pipeline"))
        {
            Debug.LogWarning($"Material '{mat.name}' is not using a URP shader. Transparency may not work.");
            return;
        }

        mat.SetFloat("_Surface", 1); 
        mat.SetFloat("_Blend", 0); 
        mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
    }
}