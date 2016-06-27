﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using Plethora.Managers.EarthquakeManager.Contracts.Data.DTO;
using Prism.Mvvm;

namespace Plethora
{
    public class MainWindowViewModel : BindableBase
    {

        public MainWindowViewModel()
        {
            InitializeEarthquakeData();
        }

        #region Initialization

        /// <summary>
        /// Initialize Earthquake Data when the app is loaded
        /// </summary>
        private void InitializeEarthquakeData()
        {
            var earthquakeData = EarthquakeServiceHelper.GetEarthuakeData();
            EarthquakeDataCollection = new ObservableCollection<EarthquakeDataModel>();
            PopulateEarthquakeData(earthquakeData, false);
            Title = "Earthquake Activity since " + EarthquakeServiceHelper.StartTime.ToLocalTime();
            SetTimer(5);
        }

        #endregion

        #region Properties

        public ObservableCollection<EarthquakeDataModel> EarthquakeDataCollection
        {
            get { return _earthquakeDataCollection; }
            set { SetProperty(ref _earthquakeDataCollection, value); }
        }

        private ObservableCollection<EarthquakeDataModel> _earthquakeDataCollection;

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        private string _title;

        #endregion

        #region Events

        /// <summary>
        /// Call the service helper to update the earthquake data
        /// </summary>
        private void UpdateEarthquakeData(object sender, EventArgs e)
        {
            var earthquakeData = EarthquakeServiceHelper.UpdateEarthquakeData();
            PopulateEarthquakeData(earthquakeData, true);
        }

        /// <summary>
        /// Populate the View with the earthquake data
        /// </summary>
        /// <param name="earthquakeData"></param>
        /// <param name="insertAtTop"></param>
        private void PopulateEarthquakeData(List<EarthquakeData> earthquakeData, bool insertAtTop)
        {
            foreach (var earthquake in earthquakeData)
            {
                var earthquakeDataModel = new EarthquakeDataModel
                {
                    DateTime = earthquake.DateTime,
                    Magnitude = earthquake.Magnitude,
                    Latitude = earthquake.Latitude,
                    Longitude = earthquake.Longitude,
                    ClosestPlaces = earthquake.AffectedCities[0] + ", " + earthquake.AffectedCities[1] + ", " + earthquake.AffectedCities[2]
                };
                Application.Current.Dispatcher.Invoke(delegate
                {
                    if (insertAtTop)
                    {
                        EarthquakeDataCollection.Insert(0, earthquakeDataModel);
                    }
                    else
                    {
                        EarthquakeDataCollection.Add(earthquakeDataModel);
                    }
                });
            }
        }

        /// <summary>
        /// Sets update timer
        /// </summary>
        /// <param name="minutes"></param>
        private void SetTimer(int minutes)
        {
            var dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += UpdateEarthquakeData;
            dispatcherTimer.Interval = new TimeSpan(0, minutes, 0);
            dispatcherTimer.Start();
        }

        #endregion
    }
}