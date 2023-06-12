using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BlacksmithTools
{
    class BodyPartController : MonoBehaviour
    {
        /*
        override bones array needs to be here, implement at a later date when making gore mod

        */

        public List<VisEquipment.PlayerModel> originalModels = new List<VisEquipment.PlayerModel>();

        public VisEquipment viseq;

        public void FullUpdate()
        {
            UpdateBodyModel();
        }

        public void UpdateBodyModel()
        {
            if(BodypartSystem.bodypartSettingsAsBones.Keys.Count != BodypartSystem.bodypartSettings.Keys.Count)
            {
                BodypartSystem.PartCfgToBoneindexes();
                BodypartSystem.CleanupCfgs();
            }

            List<int> boneIndexes = new List<int>();
            
            foreach (int hash in Util.GetEquippedHashes(viseq))
            {
                foreach (string itemName in BodypartSystem.bodypartSettingsAsBones.Keys)
                {
                    //gotta take into account autistic RRR armor names, method in util
                    if (itemName.GetStableHashCode() == hash || Util.CorrectRRRArmorPrefabName(itemName).GetStableHashCode() == hash)
                    {
                        boneIndexes.AddRange(BodypartSystem.bodypartSettingsAsBones[itemName].ToArray());
                    }
                }
            }
            Util.LogMessage("Hiding " + boneIndexes.Count.ToString() + " bones");
            if (boneIndexes.Count == 0)
            {
                viseq.m_models[viseq.GetModelIndex()].m_mesh = originalModels[viseq.GetModelIndex()].m_mesh;
                return;
            }
            Mesh freshBody = originalModels[viseq.GetModelIndex()].m_mesh;
            Mesh amputatedBody = Amputate(UnityEngine.Object.Instantiate(freshBody), boneIndexes.ToArray());
            amputatedBody.name = freshBody.name;
            viseq.m_models[viseq.GetModelIndex()].m_mesh = amputatedBody;
        }

        private Mesh Amputate(Mesh body, int[] bonesToHide)
        {
            const float minWeightToHide = 0.9f;
            const int vertexThreshold = 1;

            List<int> tris;
            BoneWeight[] weights = body.boneWeights;

            for (int subM = 0; subM < body.subMeshCount; subM++)
            {
                tris = new List<int>( body.GetTriangles(subM) );
                bool toHide;
                int tri = 0;
                while (tri < tris.Count)
                {
                    toHide = false;
                    int detectedVerts = 0;

                    for (int vert = 0; vert < 2; vert++)
                    {
                        if (toHide) break;

                        BoneWeight weight = weights[tris[tri + vert]];
                        float highestWeight = Mathf.Max(weight.weight0, weight.weight1, weight.weight2, weight.weight3);

                        for (int bone = 0; bone < 4; bone++)
                        {
                            int boneIndex = GetBoneIndex(weight, bone);
                            foreach (int boneToHide in bonesToHide)
                            {
                                if (toHide) break;

                                if (boneIndex != boneToHide) continue;
                                float value = GetBoneWeight(weight, bone);
                                if ((value / highestWeight) > minWeightToHide)
                                {
                                    if (++detectedVerts == vertexThreshold)
                                    {
                                        toHide = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    if (toHide)
                    {
                        tris.RemoveAt(tri);
                        tris.RemoveAt(tri);
                        tris.RemoveAt(tri);
                    }
                    else
                    {
                        tri += 3;
                    }
                }

                body.SetTriangles(tris.ToArray(), subM);
            }

            return body;
        }

        int GetBoneIndex(BoneWeight boneWeight, int bone)
        {
            if (bone == 0) return boneWeight.boneIndex0;
            if (bone == 1) return boneWeight.boneIndex1;
            if (bone == 2) return boneWeight.boneIndex2;
            if (bone == 3) return boneWeight.boneIndex3;
            return -1;
        }

        float GetBoneWeight(BoneWeight boneWeight, int bone)
        {
            if (bone == 0) return boneWeight.weight0;
            if (bone == 1) return boneWeight.weight1;
            if (bone == 2) return boneWeight.weight2;
            if (bone == 3) return boneWeight.weight3;
            return 0f;
        }

        public void Setup(VisEquipment _viseq)
        {
            viseq = _viseq;

            SaveOriginalModels();

            UpdateBodyModel();

            Util.LogMessage("bodypart controller attached to " + viseq.name, BepInEx.Logging.LogLevel.Message);
        }

        void SaveOriginalModels()
        {
            for (int i = 0; i < viseq.m_models.Length; i++)
            {
                VisEquipment.PlayerModel model = viseq.m_models[i];

                if (model.m_baseMaterial == null) Util.LogMessage("mat null");
                Material mat = new Material(model.m_baseMaterial);
                mat.name = model.m_baseMaterial.name;

                if (model.m_mesh == null) Util.LogMessage("mesh null");
                Mesh mesh = UnityEngine.Object.Instantiate(model.m_mesh);
                mesh.name = model.m_mesh.name;

                originalModels.Add(new VisEquipment.PlayerModel() { m_baseMaterial = mat, m_mesh = mesh });
            }
        }
    }
}
