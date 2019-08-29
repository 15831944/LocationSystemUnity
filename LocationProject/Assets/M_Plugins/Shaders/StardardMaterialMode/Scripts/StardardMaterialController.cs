using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace StardardShader
{
    /// <summary>
    /// StardardMaterial控制脚本
    /// </summary>
    public class StardardMaterialController : MonoBehaviour
    {
        public bool isEditorRecover = true;//编辑器状态下是否自动恢复材质
        public List<StardardMaterialEntity> matEntitys;//材质信息,包括材质备份
        private bool isCanGetMaterials = true;

        //public Color color;

        // Use this for initialization
        void Start()
        {
            AnewGetMaterials();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public static StardardMaterialController Add(GameObject o)
        {
            StardardMaterialController controller = o.AddMissingComponent<StardardMaterialController>();
            controller.AnewGetMaterials();
            return controller;
        }

        private void OnDisable()
        {
#if UNITY_EDITOR
            if (isEditorRecover)
            {
                RecoverMaterials();
            }

#endif
        }

        [ContextMenu("SetMatsTransparent")]
        public void SetMatsTransparent()
        {
            Color color = new Color(0.5f, 0.5f, 0.5f, 0.3f);
            SetRenderingMode(RenderingMode.Transparent, color, true);
        }

        [ContextMenu("SetMatsOpaque")]
        public void SetMatsOpaque()
        {
            SetRenderingMode(RenderingMode.Opaque);
        }

        [ContextMenu("SetMatsCutout")]
        public void SetMatsCutout()
        {
            SetRenderingMode(RenderingMode.Cutout);
        }

        [ContextMenu("SetMatsFade")]
        public void SetMatsFade()
        {
            SetRenderingMode(RenderingMode.Fade);
        }

        //public void SetMatsTransparent(float aphaT)
        //{
        //    Color colorT = new Color(0.2f, 0.2f, 0.2f, aphaT);
        //    SetRenderingMode(RenderingMode.Transparent, colorT, true);
        //}

        public void SetMatsTransparent(Color colorT, bool isClearMainTexture = true)
        {
            SetRenderingMode(RenderingMode.Transparent, colorT, isClearMainTexture);
        }

        /// <summary>
        /// 设置材质列表
        /// </summary>
        /// <param name="valueT"></param>

        public void SetRenderingMode(RenderingMode mode)
        {
            if (matEntitys == null) return;
            foreach (StardardMaterialEntity entity in matEntitys)
            {
                Material mat = entity.mat;
                if (mat.shader.name == "Standard")
                {
                    //Debug.Log(mat.shader.name);
                    //Debug.Log(mat.renderQueue);
                    StardardShaderMatHelper.SetMaterialRenderingMode(mat, mode);
                    //mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, valueT);
                }
            }
        }

        /// <summary>
        /// 设置材质列表
        /// </summary>
        /// <param name="valueT"></param>

        public void SetRenderingMode(RenderingMode mode, float valueT = 0.5F)
        {
            if (matEntitys == null) return;
            foreach (StardardMaterialEntity entity in matEntitys)
            {
                Material mat = entity.mat;
                if (mat.shader.name == "Standard")
                {
                    //Debug.Log(mat.shader.name);
                    //Debug.Log(mat.renderQueue);
                    StardardShaderMatHelper.SetMaterialRenderingMode(mat, mode);
                    mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, valueT);
                }
            }
        }

        /// <summary>
        /// 设置材质列表
        /// </summary>
        /// <param name="valueT"></param>

        public void SetRenderingMode(RenderingMode mode, Color colorT, bool isClearMainTexture)
        {
            if (matEntitys == null) return;
            foreach (StardardMaterialEntity entity in matEntitys)
            {
                Material mat = entity.mat;
                if (mat.shader.name == "Standard")
                {
                    //Debug.Log(mat.shader.name);
                    //Debug.Log(mat.renderQueue);
                    StardardShaderMatHelper.SetMaterialRenderingMode(mat, mode);
                    mat.color = colorT;
                    if (isClearMainTexture)
                    {
                        ClearMainTexture(mat);
                    }
                }
            }
        }

        /// <summary>
        /// 清除主贴图
        /// </summary>
        public void ClearMainTexture(Material mat)
        {
            mat.mainTexture = null;
        }

        //[ContextMenu("ShowMatsInfo")]
        //public void ShowMatsInfo()
        //{
        //    foreach (StardardMaterialEntity entity in matEntitys)
        //    {
        //        Material mat = entity.mat;
        //        if (mat.shader.name == "Standard")
        //        {
        //            //Debug.Log(mat.shader.name);
        //            Debug.Log(mat.renderQueue);
        //        }
        //    }
        //}


        /// <summary>
        /// 恢复材质的到原本状态
        /// </summary>
        [ContextMenu("RecoverMaterials")]
        public void RecoverMaterials()
        {            
            //if (matEntitys == null) return;
            if (matEntitys != null)
            {
                foreach (StardardMaterialEntity entity in matEntitys)
                {
                    entity.Recover();
                }
            }

        }

        /// <summary>
        /// 获取指定文件夹下的所有材质，备份材质
        /// </summary>
        [ContextMenu("GetMaterials")]
        public void GetMaterials()
        {
            if (isCanGetMaterials == false) return;
            isCanGetMaterials = false;
            if (matEntitys == null)
            {
                matEntitys = new List<StardardMaterialEntity>();
            }
            Renderer[] renders = gameObject.GetComponentsInChildren<Renderer>(true);

            foreach (Renderer r in renders)
            {
                foreach (Material m in r.sharedMaterials)
                {
                    StardardMaterialEntity entity = matEntitys.Find((item) => item.mat == m);
                    if (entity == null)
                    {
                        if (m == null||m.name=="Default-Material")
                        {
                            continue;
                        }
                        StardardMaterialEntity entityT = new StardardMaterialEntity(m);
                        matEntitys.Add(entityT);
                    }
                }
            }

            //SkinnedMeshRenderer[] skinnedMeshRenderers = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);

            //foreach (Renderer r in skinnedMeshRenderers)
            //{
            //    foreach (Material m in r.sharedMaterials)
            //    {
            //        StardardMaterialEntity entity = matEntitys.Find((item) => item.mat == m);
            //        if (entity == null)
            //        {
            //            if (m == null)
            //            {
            //                continue;
            //            }
            //            StardardMaterialEntity entityT = new StardardMaterialEntity(m);
            //            matEntitys.Add(entityT);
            //        }
            //    }
            //}

        }

        /// <summary>
        /// 重新,获取指定文件夹下的所有材质，并备份材质
        /// </summary>
        [ContextMenu("REGetMaterials")]
        public void AnewGetMaterials()
        {
            RecoverMaterials();//先恢复材质原始颜色
            isCanGetMaterials = true;
            if (matEntitys == null)
            {
                matEntitys = new List<StardardMaterialEntity>();
            }
            else
            {
                matEntitys.Clear();
            }
            GetMaterials();
        }
    }
}
