using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace DreamLU
{
    public enum StatisticsType
    {
        Score,
        DamageCaused,
        NumberEnemiesKilled,
        WeaponID,
        RunTime,
    }
    
    public class StatisticsPanel : MonoBehaviour
    {
        [SerializeField] private List<StatisticsType> statisticsTypes = new List<StatisticsType>();
        [SerializeField] private StatisticsValue statisticsValuePrefab;
        [SerializeField] private Transform contentStatistics;

        private IAggregateDataProvider _aggregateDataProvider;

        private void Awake()
        {
            _aggregateDataProvider = CoreLifetimeScope.SharedContainer.Resolve<IAggregateDataProvider>();
        }

        private void OnEnable()
        {
            InitStatistics();
        }
        
        private void InitStatistics()
        {
            HelperUtilities.ClearChildren(contentStatistics, true);

            var data = _aggregateDataProvider.AggregateData;
            foreach (var type in statisticsTypes)
            {
                var statisticsValue = Instantiate(statisticsValuePrefab, contentStatistics);
                var statistic = Resolve(type, data);
                statisticsValue.SetName(statistic.Item1);
                statisticsValue.SetValue(statistic.Item2);
            }
        }
        
        private (string, string) Resolve(StatisticsType type, AggregateData aggregateData)
        {
            (string, string) result = ("", "");

            if (type == StatisticsType.Score)
            {
                result.Item1 = "Score";
                result.Item2 = aggregateData.score.ToString();
            }
            else if (type == StatisticsType.DamageCaused)
            {
                result.Item1 = "Damage Caused";
                result.Item2 = aggregateData.damageCaused.ToString();
            }
            else if (type == StatisticsType.NumberEnemiesKilled)
            {
                result.Item1 = "Number Enemies Killed";
                result.Item2 = aggregateData.numberEnemiesKilled.ToString();
            }
            else if (type == StatisticsType.RunTime)
            {
                result.Item1 = "Run Time";
                result.Item2 = $"{aggregateData.time / 60:00}:{aggregateData.time % 60:00}";
            }

            return result == ("", "") ? ("None", "0") : result;
        }
    }
}