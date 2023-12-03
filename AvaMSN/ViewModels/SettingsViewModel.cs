﻿using ReactiveUI;
using System;
using System.Reactive;
using System.Threading.Tasks;

namespace AvaMSN.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }
    public ReactiveCommand<Unit, Unit> BackCommand { get; }

    public SettingsManager SettingsManager { get; set; } = new SettingsManager();
    public string Server { get; set; } = string.Empty;
    public bool SaveMessages { get; set; }

    private string resultText = string.Empty;

    public string ResultText
    {
        get => resultText;
        set => this.RaiseAndSetIfChanged(ref resultText, value);
    }

    public event EventHandler? BackButtonPressed;

    public SettingsViewModel()
    {
        SaveCommand = ReactiveCommand.CreateFromTask(Save);
        BackCommand = ReactiveCommand.Create(Back);
        Server = SettingsManager.Settings.Server;
        SaveMessages = SettingsManager.Settings.SaveMessagingHistory;
    }

    /// <summary>
    /// Assigns input values to model and calls save to settings file function.
    /// </summary>
    private async Task Save()
    {
        if (Server != string.Empty)
            SettingsManager.Settings.Server = Server;

        SettingsManager.Settings.SaveMessagingHistory = SaveMessages;
        SettingsManager.SaveToFile();

        ResultText = "Saved successfully!";
        await Task.Delay(2000);
        ResultText = string.Empty;
    }

    private void Back()
    {
        BackButtonPressed?.Invoke(this, EventArgs.Empty);
    }
}
