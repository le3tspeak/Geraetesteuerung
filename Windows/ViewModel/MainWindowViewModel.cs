using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Timers;
using System.Reflection;
using Übung_Gerät.Windows.Models;

namespace Übung_Gerät.Windows.ViewModel;

internal class MainWindowViewModel : ViewModelBase
{

    //Fields
    private int puls;
    private int lastPuls;
    private int pulseProgress;
    private int power;
    private int time;
    private int timeRemaining;
    private int workoutTime;
    private int workoutTimeRemaining;
    private double progress;
    private string message;
    private Visibility displayMessage;
    private Visibility pulseRefresh;
    private bool isRunning;

    //Properties
    public int Puls
    {
        get => puls;
        set
        {
            puls = value;
            OnPropertyChanged();
        }
    }
    public int LastPuls
    {
        get => lastPuls;
        set
        {
            lastPuls = value;
            OnPropertyChanged();
        }
    }
    public int PulseProgress
    {
        get => pulseProgress;
        set
        {
            pulseProgress = value;
            OnPropertyChanged();
        }
    }
    public int Power
    {
        get => power;
        set
        {
            power = value;
            OnPropertyChanged();
        }
    }
    public int Time
    {
        get => time;
        set
        {
            if (value <= 4) value = 4;
            time = value;
            OnPropertyChanged();
        }
    }
    public int TimeRemaining
    {
        get => timeRemaining;
        set
        {
            timeRemaining = value;
            OnPropertyChanged();
        }
    }
    public int WorkoutTime
    {
        get => workoutTime;
        set
        {
            workoutTime = value;
            OnPropertyChanged();
        }
    }
    public int WorkoutTimeRemaining
    {
        get => workoutTimeRemaining;
        set
        {
            workoutTimeRemaining = value;
            OnPropertyChanged();
        }
    }
    public double Progress
    {
        get => progress;
        set
        {
            progress = value;
            OnPropertyChanged();
        }
    }
    public string Message
    {
        get => message;
        set
        {
            message = value;
            OnPropertyChanged();
        }
    }
    public bool IsRunning
    {
        get => isRunning;
        set
        {
            isRunning = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsNotRunning));
        }
    }
    public bool IsNotRunning
    {
        get => !IsRunning;
        set
        {
            IsRunning = !value;
            OnPropertyChanged();
        }
    }

    //Timer
    private System.Timers.Timer _timerWorkout;
    private System.Timers.Timer _timerPulse;
    private System.Timers.Timer _timerPowerLevel;
    private System.Threading.Timer _intervall;

    //Visibility
    public Visibility Trainingprogress
    {
        get => WorkoutTimeRemaining > 0 ? Visibility.Visible : Visibility.Collapsed;
        set => OnPropertyChanged();
    }
    public Visibility DisplayMessage
    {
        get => displayMessage;
        set
        {
            displayMessage = value;
            OnPropertyChanged();
        }
    }
    public Visibility PulseRefresh
    {
        get => pulseRefresh;
        set
        {
            pulseRefresh = value;
            OnPropertyChanged();
        }
    }

    //Power Level Control
    private Leistungsregelung powerLevel;

    //Pulse Generator only for testing purposes
    private readonly PulseGenerator pulseGenerator;
    private CancellationTokenSource cancellationTokenSource;
    public bool PulseIsRunning => cancellationTokenSource != null;
    //Pulse Generator only for testing purposes

    //Commands
    public ICommand StartCommand
    {
        get;
    }
    public ICommand StopCommand
    {
        get;
    }
    public ICommand ResetCommand
    {
        get;
    }
    public ICommand ManipulateWorkoutTimeCommand
    {
        get;
    }
    public ICommand MinimizeAPPCommand
    {
        get;
    }
    public ICommand CloseAPPCommand
    {
        get;
    }
    public ICommand StartCommandPulse
    {
        get;
    }
    public ICommand StopCommandPulse
    {
        get;
    }

    //Constructor
    public MainWindowViewModel()
    {
        // Set Default Values for the Properties
        Time = 10;
        Puls = 0;
        PulseProgress = 0;
        Power = 0;

        // Set Default Values for the Visibility
        DisplayMessage = Visibility.Collapsed;
        Trainingprogress = Visibility.Collapsed;
        PulseRefresh = Visibility.Collapsed;

        // Power Level Control
        powerLevel = new Leistungsregelung();

        //Start and Stop Commands
        StartCommand = new ViewModelCommand(ExecuteStartCommand);
        StopCommand = new ViewModelCommand(ExecuteStopCommand);

        // Time Commands
        ManipulateWorkoutTimeCommand = new ViewModelCommand(ExecuteManipulateWorkoutTime);

        // App Commands
        MinimizeAPPCommand = new ViewModelCommand(ExecuteMinimizeAPPCommand);
        CloseAPPCommand = new ViewModelCommand(ExecuteCloseAPPCommand);

        // Start Pulse Generation only for testing purposes
        pulseGenerator = new PulseGenerator();        
    }

    //Timer Methods

    // Start Timer for Workout
    private void StartTimerWorkout()
    {        
        _timerWorkout = new System.Timers.Timer(WorkoutTimeRemaining);
        _timerWorkout.Elapsed += TimerWorkoutElapsed;
        _timerWorkout.Start();
    }
    // Event for Timer Workout
    private void TimerWorkoutElapsed(object? sender, ElapsedEventArgs e)
    {
        Debug.WriteLine("Workout beendet");
        DisplayMessage = Visibility.Visible;
        Message = "Workout beendet";
        Reset();
    }

    // Start Timer for Interval
    private void StartTimerInterval()
    {
        // Start a new task for the timer
        Task.Run(async () =>
        {
            _intervall = new System.Threading.Timer(async (e) => await IntervalTick(), null, 0, 1000);
        });
    }
    // Event for Timer Interval
    private async Task IntervalTick()
    {
        // Update the Pulse Refresh Progress
        if (PulseProgress <= 75)
        {
            PulseProgress += 25;
        }
        else
        {
            PulseProgress = 0;
        }

        // Update the remaining time and progress
        WorkoutTimeRemaining -= 1000;
        Progress = Models.Zeitregelung.ChangeProgress(WorkoutTime, WorkoutTimeRemaining);
    }
    public void StopTimerInterval()
    {
        _intervall?.Change(Timeout.Infinite, 0);
        _intervall?.Dispose();
    }

    // Start Timer for Pulse
    private void StartTimerPuls()
    {
        _timerPulse = new System.Timers.Timer(5000);
        _timerPulse.Elapsed += TimerPulsElapsed;
        _timerPulse.Start();
    }
    // Event for Timer Pulse
    private async void TimerPulsElapsed(object? sender, ElapsedEventArgs e)
    {
        Debug.WriteLine("Puls aktualisiert");
        // Set new Pulse Value from Pulse Generator
        Puls = pulseGenerator.Puls;
    }

    // Start Timer for Power Level
    private void StartTimerPowerLevel()
    {
        _timerPowerLevel = new System.Timers.Timer(1000 * 60); // 1 Minute
        _timerPowerLevel.Elapsed += TimerPowerLevelElapsed;
        _timerPowerLevel.Start();
    }
    // Event for Timer Power Level
    private void TimerPowerLevelElapsed(object? sender, ElapsedEventArgs e)
    {
        Power = Models.Leistungsregelung.UpdatePowerLevel(Puls, LastPuls);
    }

    //Methods

    // Close Application
    private void ExecuteCloseAPPCommand(object obj) => Application.Current.Shutdown();
    // Minimize Application
    private void ExecuteMinimizeAPPCommand(object obj) => Application.Current.MainWindow.WindowState = WindowState.Minimized;


    // Start Workout
    private void ExecuteStartCommand(object obj)
    {  
        Debug.WriteLine($"Starte Workout mit {Time}min");
        // Reset Progress
        Progress = 100;
        // Set Time Remaining
        WorkoutTimeRemaining = Time * 60 * 1000;
        // Set Workout Time
        WorkoutTime = WorkoutTimeRemaining;
        Power = 1;
        // Reset Pulse Progress
        PulseProgress = 0;
        // Set Visibility 
        Trainingprogress = Visibility.Visible;
        DisplayMessage = Visibility.Collapsed;
        PulseRefresh = Visibility.Visible;
        // Start Timers
        IsRunning = true;
        StartTimerWorkout();
        StartTimerPuls();
        StartTimerInterval();
        StartTimerPowerLevel();

        _ = StartPulseGeneration();

        Puls = pulseGenerator.Puls;
        LastPuls = Puls;
    }

    // Stop Workout
    private void ExecuteStopCommand(object obj)
    {
        Debug.WriteLine("Stoppe Workout");
        DisplayMessage = Visibility.Visible;
        Message = "Workout gestoppt";
        Reset();

        // Stop Pulse Generation only for testing purposes
        StopPulseGeneration();
    }

    // Reset Workout after Stop or End
    private void Reset()
    {
        // Reset Timers
        _timerWorkout.Stop();
        _timerWorkout.Dispose();
        _timerWorkout.Close();
        _timerPulse.Stop();
        _timerPulse.Dispose();
        _timerPulse.Close();
        StopTimerInterval();

        // Reset Values
        WorkoutTimeRemaining = 0;
        Progress = 0;
        PulseProgress = 0;
        Time = 10;
        Puls = 0;
        Power = 0;
        Trainingprogress = Visibility.Collapsed;
        PulseRefresh = Visibility.Collapsed;
        IsRunning = false;

        // Stop Pulse Generation only for testing purposes
        StopPulseGeneration();
    }

    // Change Time 
    private void ExecuteManipulateWorkoutTime(object parameter) => Time = Models.Zeitregelung.ChangeTime(parameter, Time);


    // Pulse Generator only for testing purposes
    private async Task StartPulseGeneration()
    {
        cancellationTokenSource = new CancellationTokenSource();
        OnPropertyChanged(nameof(PulseIsRunning));
        try
        {
            await pulseGenerator.GenerateRandomPulseAsync(WorkoutTime, cancellationTokenSource.Token);
        }
        catch (TaskCanceledException)
        {
            Debug.WriteLine("Pulse generation was cancelled.");
        }
        finally
        {
            Puls = pulseGenerator.Puls;
            cancellationTokenSource = null;
            OnPropertyChanged(nameof(PulseIsRunning));
        }
    }
    private void StopPulseGeneration()
    {
        cancellationTokenSource?.Cancel();
    }
}
