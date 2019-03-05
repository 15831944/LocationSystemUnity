using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StardardShader
{
    /// <summary>
    /// StardardShader相关帮助类
    /// </summary>
    public class StardardShaderMatHelper : MonoBehaviour
    {

        //public Material mat;

        void Start()
        {
            //SetMaterialRenderingMode(mat, RenderingMode.Fade);
        }





        //设置材质的渲染模式
        public static void SetMaterialRenderingMode(Material material, RenderingMode renderingMode)
        {
            switch (renderingMode)
            {
                case RenderingMode.Opaque:
                    material.SetFloat("_Mode", 0);
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = -1;
                    break;
                case RenderingMode.Cutout:
                    material.SetFloat("_Mode", 1);
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.EnableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 2450;
                    break;
                case RenderingMode.Fade:
                    material.SetFloat("_Mode", 2);
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 3000;
                    break;
                case RenderingMode.Transparent:
                    material.SetFloat("_Mode", 3);
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 3000;
                    break;
            }
        }

        /// <summary>
        /// 获取标准材质实体
        /// </summary>
        public static StardardMaterialEntity GetStardardMaterialBackups(Material mat)
        {
            StardardMaterialEntity entity = new StardardMaterialEntity(mat);
            return entity;
        }

        ///// <summary>
        ///// 获取标准材质实体
        ///// </summary>
        //public static StardardMaterialEntity GetStardardMaterialEntity(Material material)
        //{
        //    StardardMaterialEntity entity = new StardardMaterialEntity();
        //    if (material.renderQueue == 2000)
        //    {
        //        entity.mode = RenderingMode.Opaque;
        //    }
        //    else if (material.renderQueue == 2450)
        //    {
        //        entity.mode = RenderingMode.Cutout;
        //    }
        //    else if (material.renderQueue == 3000)
        //    {
        //        int n = material.GetInt("_SrcBlend");
        //        if (n == (int)UnityEngine.Rendering.BlendMode.SrcAlpha)
        //        {
        //            entity.mode = RenderingMode.Fade;
        //        }
        //        else if (n == (int)UnityEngine.Rendering.BlendMode.One)
        //        {
        //            entity.mode = RenderingMode.Transparent;
        //        }

        //    }

        //    entity.color = material.color;

        //    Debug.Log(string.Format("mode:{0},color:{1}", entity.mode, entity.color));

        //    return entity;
        //}

        ///// <summary>
        ///// 设置材质的相关信息，通过材质实体类
        ///// </summary>
        //public static void SetMaterialByEntity(Material mat, StardardMaterialEntity entityT)
        //{
        //    SetMaterialRenderingMode(mat, entityT.mode);
        //    mat.color = entityT.color;
        //}
    }
    public enum RenderingMode
    {
        Opaque,
        Cutout,
        Fade,
        Transparent
    }
    //public class StardardMaterialEntity
    //{
    //    public RenderingMode mode;//模式
    //    public Color color;//透明度值
    //}

    /// <summary>
    /// 对应材质备份实体
    /// </summary>
    [Serializable]
    public class StardardMaterialEntity
    {
        public Material mat;//材质
        [SerializeField]
        private Material matBackups;//材质备份，不可修改

        public StardardMaterialEntity(Material m)
        {
            mat = m;
            matBackups = new Material(m);
        }

        /// <summary>
        /// 恢复原有材质
        /// </summary>
        public void Recover()
        {
            if (matBackups != null)
            {
                mat.CopyPropertiesFromMaterial(matBackups);
            }
        }
    }
}
