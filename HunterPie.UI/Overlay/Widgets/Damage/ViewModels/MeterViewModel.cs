﻿using HunterPie.Core.Client.Configuration.Enums;
using HunterPie.Core.Client.Configuration.Overlay;
using HunterPie.Core.Extensions;
using HunterPie.UI.Architecture;
using LiveCharts;
using System;
using System.Collections.ObjectModel;

namespace HunterPie.UI.Overlay.Widgets.Damage.ViewModels
{
    public class MeterViewModel : ViewModel
    {
        public DamageMeterWidgetConfig Settings { get; }
        private double _timeElapsed = 1;
        private int _deaths;
        private bool _inHuntingZone;
        private Func<double, string> _damageFormatter;

        public Func<double, string> TimeFormatter { get; set;  } = 
            new Func<double, string>((value) => TimeSpan.FromSeconds(value).ToString("mm\\:ss"));

        public Func<double, string> DamageFormatter { get => _damageFormatter; set { SetValue(ref _damageFormatter, value); } }

        public SeriesCollection Series { get; protected set; } = new();

        public ObservableCollection<PlayerViewModel> Players { get; } = new();

        public PetsViewModel Pets { get; }

        public double TimeElapsed
        {
            get => _timeElapsed;
            set { SetValue(ref _timeElapsed, value); }
        }

        public int Deaths
        {
            get => _deaths;
            set { SetValue(ref _deaths, value); }
        }

        public bool InHuntingZone { get => _inHuntingZone; set { SetValue(ref _inHuntingZone, value); } }

        public MeterViewModel(DamageMeterWidgetConfig config)
        {
            Settings = config;
            Pets = new(config);
            SetupFormatters();
        }

        private void SetupFormatters()
        {
            DamageFormatter = new Func<double, string>((value) => FormatDamageByStrategy(value));
        }

        private string FormatDamageByStrategy(double damage)
        {
            return (Settings.DamagePlotStrategy.Value) switch
            {
                DamagePlotStrategy.TotalDamage => damage.ToString(),
                DamagePlotStrategy.DamagePerSecond => $"{damage:0.00}/s",
                _ => throw new NotImplementedException()
            };
        }

        public void ToggleHighlight() => Settings.ShouldHighlightMyself.Value = !Settings.ShouldHighlightMyself;
        public void ToggleBlur() => Settings.ShouldBlurNames.Value = !Settings.ShouldBlurNames;

        public void SortMembers()
        {
            Players.SortInPlace(player => player.Damage);
            Pets.Damages.SortInPlace(pet => pet.Percentage);
        }
    }
}
