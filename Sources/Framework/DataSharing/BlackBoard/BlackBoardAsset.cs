using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityTools.Atom
{

    /// <summary>
    ///  A class allowing to store an array of variables by entities. 
    ///  This allows information to be transmitted through multiple components by reducing their coupling.
    ///  Useful for purpose such as: 
    ///  - Finite State machine: to transit information through state without referencing different components or object.
    ///  - Indepedant monobehaviours: you can design very small behaviour without needed interaction with other, you just needed a blackboard holding this data. (i.e. get the direction; know if a character is landing, dead...)
    /// </summary>
    [CreateAssetMenu(menuName = "Unity Tools/Atom/Asset/BlackBoard", fileName = "[BlackBoard]-", order = 0)]
    public class BlackBoardAsset : ScriptableObject_AdvPostProcessing
    {
        #region CLASS
        [System.Serializable]
        private struct ParametersBoard
        {
            [SerializeField]
            private Dictionary<string, InternalBlackboardParameter> ParametersMap;
            public Dictionary<string, InternalBlackboardParameter> Parameters { get => ParametersMap; }

            public ParametersBoard(ParametersBoard board)
            {
                List<InternalBlackboardParameter> bbl = new List<InternalBlackboardParameter>(board.ParametersMap.Values);
                ParametersMap = new Dictionary<string, InternalBlackboardParameter>(bbl.Count);
                Build(bbl);
            }

            public void Build(List<InternalBlackboardParameter> bbp)
            {
                if (ParametersMap != null)
                {
                    ParametersMap.Clear();
                }
                else
                {
                    ParametersMap = new Dictionary<string, InternalBlackboardParameter>(bbp.Count);
                }

                for (int i = 0, c = bbp.Count; i < c; ++i)
                {
                    InternalBlackboardParameter cur = bbp[i].Clone() as InternalBlackboardParameter;
                    cur.Init();
                    ParametersMap.Add(cur.Name, cur);
                }
            }

            public bool SetReference<T, TAsset, TVariable>(string entryName, ref TAsset var)
                where TAsset : TAsset<T>
                where TVariable : TVariable<T, TAsset>
            {
                if (ParametersMap.ContainsKey(entryName))
                {
                    InternalBlackboardParameter<T, TAsset, TVariable> b = ParametersMap[entryName] as InternalBlackboardParameter<T, TAsset, TVariable>;
                    b.Value = var;
                    return b != null ? b.Value : null;
                }

                return false;
            }

            public TAsset GetVariable<T, TAsset, TVariable>(string entryName)
                where TAsset : TAsset<T>
                where TVariable : TVariable<T, TAsset>
            {
                if (entryName != null && entryName != "" && ParametersMap.ContainsKey(entryName))
                {
                    InternalBlackboardParameter<T, TAsset, TVariable> b = ParametersMap[entryName] as InternalBlackboardParameter<T, TAsset, TVariable>;
                    return b != null ? b.Value : null;
                }

                return null;
            }

            public string Scan()
            {
                string Ah = "";
                foreach (var v in ParametersMap.Keys)
                {
                    Ah += "- \"" + v + "\"" + ParametersMap[v] + "\n";
                }
                return Ah;
            }
        }
        #endregion


        #region PROPERTIES
        [HideInInspector]
        public List<InternalBlackboardParameter> BlackboardParameters = new List<InternalBlackboardParameter>();

        // The template blackboard.
        private ParametersBoard ParametersMap;

        // The map holding blackboard per object.
        private Dictionary<GameObject, ParametersBoard> m_EntitiesMap = null;
        private Dictionary<GameObject, ParametersBoard> EntitiesMap
        {
            get
            {
                if (m_EntitiesMap == null)
                {
                    //if (ParametersMap == null)
                    //{
                    //    ParametersMap = new ParametersBoard();
                    //}
                    ParametersMap.Build(BlackboardParameters);
                    m_EntitiesMap = new Dictionary<GameObject, ParametersBoard>(3);
                }
                return m_EntitiesMap;
            }
        }

#if UNITY_EDITOR
        [ReadOnly]
        public bool NeedRefresh = false;
#endif
        #endregion


        #region CONSTRUCTOR
        public void Awake()
        {
            //if(ParametersMap == null)
            //{
            //    ParametersMap = new ParametersBoard();
            //}
            ParametersMap.Build(BlackboardParameters);
        }
        #endregion


        #region PUBLIC METHODS - TEMPLATE
        public bool SetReference<T, TAsset, TVariable>(BlackboardTargetParameter target, ref TAsset var)
            where TAsset : TAsset<T>
            where TVariable : TVariable<T, TAsset>
        {
            if (EntitiesMap.ContainsKey(target.Target))
            {
                return m_EntitiesMap[target.Target].SetReference<T, TAsset, TVariable>(target.EntryName, ref var);
            }
            else
            {
                m_EntitiesMap.Add(target.Target, ParametersMap);
                target.Initialize(this);
                return SetReference<T, TAsset, TVariable>(target, ref var);
            }
        }

        public TAsset GetVariable<T, TAsset, TVariable>(BlackboardTargetParameter target)
            where TAsset : TAsset<T>
            where TVariable : TVariable<T, TAsset>
        {
            if (EntitiesMap.ContainsKey(target.Target))
            {
                if (target.EntryName != null && target.EntryName == "")
                {
                    return null;
                }
                return m_EntitiesMap[target.Target].GetVariable<T, TAsset, TVariable>(target.EntryName);
            }
            else
            {
                m_EntitiesMap.Add(target.Target, new ParametersBoard(ParametersMap));
                target.Initialize(this);
                return GetVariable<T, TAsset, TVariable>(target);
            }
        }

        public void SetValue<T>(BlackboardTargetParameter target, T var)
        {
            if (EntitiesMap.ContainsKey(target.Target))
            {
                TAsset<T> val = GetVariable<T, TAsset<T>, TVariable<T, TAsset<T>>>(target);
                if (val)
                {
                    val.Value = var;
                }
            }
            else
            {
                m_EntitiesMap.Add(target.Target, new ParametersBoard(ParametersMap));
                target.Initialize(this);
                SetValue<T>(target, var);
            }
        }

        public bool SetReference<T, TAsset, TVariable>(GameObject target, string entryName, ref TAsset var)
            where TAsset : TAsset<T>
            where TVariable : TVariable<T, TAsset>
        {
            if (EntitiesMap.ContainsKey(target))
            {
                return m_EntitiesMap[target].SetReference<T, TAsset, TVariable>(entryName, ref var);
            }
            else
            {
                m_EntitiesMap.Add(target, new ParametersBoard(ParametersMap));
                return SetReference<T, TAsset, TVariable>(target, entryName, ref var);
            }
        }

        public TAsset GetVariable<T, TAsset, TVariable>(GameObject target, string entryName)
            where TAsset : TAsset<T>
            where TVariable : TVariable<T, TAsset>
        {
            if (EntitiesMap.ContainsKey(target))
            {
                if (entryName != null && entryName == "")
                {
                    return null;
                }
                return m_EntitiesMap[target].GetVariable<T, TAsset, TVariable>(entryName);
            }
            else
            {
                m_EntitiesMap.Add(target, new ParametersBoard(ParametersMap));
                return GetVariable<T, TAsset, TVariable>(target, entryName);
            }
        }

        public void SetValue<T, TAsset, TVariable>(GameObject target, string entryName, T var)
            where TAsset : TAsset<T>
            where TVariable : TVariable<T, TAsset>
        {
            if (EntitiesMap.ContainsKey(target))
            {
                TAsset val = GetVariable<T, TAsset, TVariable>(target, entryName);
                if (val)
                {
                    val.Value = var;
                }
            }
            else
            {
                m_EntitiesMap.Add(target, new ParametersBoard(ParametersMap));
                SetValue<T, TAsset, TVariable>(target, entryName, var);
            }
        }
        #endregion



        #region PUBLIC METHODS
        public void ScanParameters(GameObject target)
        {
            if (EntitiesMap.ContainsKey(target))
            {
                Debug.Log(this + "[" + target + "] params:\n" + m_EntitiesMap[target].Scan());
            }
            else
            {
                Debug.LogWarning(this + " doesn't contain the key '" + target + "'!");
            }
        }

        public void InitializeTarget(GameObject target)
        {
            if (!EntitiesMap.ContainsKey(target))
            {
                m_EntitiesMap.Add(target, new ParametersBoard(ParametersMap));
            }
        }


#if UNITY_EDITOR
        /// <summary>
        /// Call this when adding or removing a blackboard value.
        /// </summary>
        public void RefreshBlackBoard()
        {
            if(m_EntitiesMap == null)
            {
                return;
            }

            if(NeedRefresh)
            {
                NeedRefresh = false;

                ParametersMap.Build(BlackboardParameters);

                List<GameObject> gaos = new List<GameObject>(m_EntitiesMap.Keys);
                for(int i = 0, c = gaos.Count; i < c; ++i)
                {
                    m_EntitiesMap[gaos[i]] = new ParametersBoard(ParametersMap);
                }
            }
        }

        public Dictionary<System.Type, List<string>> GetAllBlackboardValueOfTarget(GameObject target)
        {
            RefreshBlackBoard();

            if (EntitiesMap.ContainsKey(target))
            {
                Dictionary<System.Type, List<string>> map = new Dictionary<System.Type, List<string>>();
                foreach (string key in EntitiesMap[target].Parameters.Keys)
                {
                    System.Type type = EntitiesMap[target].Parameters[key].Editor_GetBasicType();
                    
                    if (!map.ContainsKey(type))
                    {
                        map.Add(type, new List<string>());
                    }
                    
                    map[type].Add(key);
                }

                return map;
            }

            return null;
        }
#endif

        // BOOLEAN
        public BoolAsset GetBooleanVariable(BlackboardTargetParameter target)
        {
            return GetVariable<bool, BoolAsset, BoolVariable>(target);
        }

        public void SetBooleanValue(BlackboardTargetParameter target, bool value)
        {
            SetValue(target, value);
        }

        public bool SetBooleanReference(BlackboardTargetParameter target, ref BoolAsset var)
        {
            return SetReference<bool, BoolAsset, BoolVariable>(target, ref var);
        }


        // INTEGER
        public IntegerAsset GetIntegerVariable(BlackboardTargetParameter target)
        {
            return GetVariable<int, IntegerAsset, IntegerVariable>(target);
        }

        public void SetIntegerValue(BlackboardTargetParameter target, int value)
        {
            SetValue(target, value);
        }

        public bool SetIntegerReference(BlackboardTargetParameter target, ref IntegerAsset var)
        {
            return SetReference<int, IntegerAsset, IntegerVariable>(target, ref var);
        }


        // FLOAT
        public FloatAsset GetFloatVariable(BlackboardTargetParameter target)
        {
            return GetVariable<float, FloatAsset, FloatVariable>(target);
        }

        public void SetFloatValue(BlackboardTargetParameter target, float value)
        {
            SetValue(target, value);
        }

        public bool SetFloatReference(BlackboardTargetParameter target, ref FloatAsset var)
        {
            return SetReference<float, FloatAsset, FloatVariable>(target, ref var);
        }


        // VECTOR2
        public Vector2Asset GetVector2Variable(BlackboardTargetParameter target)
        {
            return GetVariable<Vector2, Vector2Asset, Vector2Variable>(target);
        }

        public void SetVector2Value(BlackboardTargetParameter target, Vector2 value)
        {
            SetValue(target, value);
        }

        public bool SetVector2Reference(BlackboardTargetParameter target, ref Vector2Asset var)
        {
            return SetReference<Vector2, Vector2Asset, Vector2Variable>(target, ref var);
        }


        // VECTOR3
        public Vector3Asset GetVector3Variable(BlackboardTargetParameter target)
        {
            return GetVariable<Vector3, Vector3Asset, Vector3Variable>(target);
        }

        public void SetVector3Value(BlackboardTargetParameter target, Vector3 value)
        {
            SetValue(target, value);
        }

        public bool SetVector3Reference(BlackboardTargetParameter target, ref Vector3Asset var)
        {
            return SetReference<Vector3, Vector3Asset, Vector3Variable>(target, ref var);
        }


        // GAMEOBJECT
        public GameObjectAsset GetGameObjectVariable(BlackboardTargetParameter target)
        {
            return GetVariable<GameObject, GameObjectAsset, GameObjectVariable>(target);
        }

        public void SetGameObjectValue(BlackboardTargetParameter target, GameObject value)
        {
            SetValue(target, value);
        }

        public bool SetGameObjectReference(BlackboardTargetParameter target, ref GameObjectAsset var)
        {
            return SetReference<GameObject, GameObjectAsset, GameObjectVariable>(target, ref var);
        }
#endregion
    }
}