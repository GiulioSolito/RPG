using System.Collections.Generic;
using RPG.Stats;
using UnityEngine;
using Sirenix.OdinInspector;

namespace RPG.Core
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Progression/New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [TableList]
        [SerializeField] private ProgressionCharacterClass[] characterClasses = null;

        Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable;
        
        public float GetStat(Stat stat, CharacterClass characterClass, int level)
        {
            BuildLookup();

            float[] levels = lookupTable[characterClass][stat];
            
            if (levels.Length < level) return 0;

            return levels[level - 1];
        }

        public int GetLevels(Stat stat, CharacterClass characterClass)
        {
            BuildLookup();
            
            float[] levels = lookupTable[characterClass][stat];
            return levels.Length;
        }

        void BuildLookup()
        {
            if (lookupTable != null) return;

            lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

            foreach (ProgressionCharacterClass progressionClass in characterClasses)
            {
                var statLookupTable=new Dictionary<Stat, float[]>();

                foreach (ProgressionStat progressionStat in progressionClass.stats)
                {
                    statLookupTable[progressionStat.stat] = progressionStat.levels;
                }
                
                lookupTable[progressionClass.characterClass] = statLookupTable;
            }
        }

        [System.Serializable]
        class ProgressionCharacterClass
        {
            [TableColumnWidth(100, Resizable = false)]
            public CharacterClass characterClass;
            [TableList]
            public ProgressionStat[] stats;
        }
    }

    [System.Serializable]
    class ProgressionStat
    {
        public Stat stat;
        [VerticalGroup("Levels")]
        public float[] levels;
    }
}