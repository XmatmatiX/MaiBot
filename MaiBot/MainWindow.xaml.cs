using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.IO;

namespace MaiBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        SpeechRecognitionEngine _recognizer = new SpeechRecognitionEngine();
        SpeechSynthesizer Mai = new SpeechSynthesizer();
        SpeechRecognitionEngine startListening = new SpeechRecognitionEngine();
        int RecTimeOut = 0;
        System.Windows.Threading.DispatcherTimer Timer = new System.Windows.Threading.DispatcherTimer();
        

        public MainWindow()
        {
            InitializeComponent();
            Timer.Tick += Timer_Tick;
            Timer.Interval = new TimeSpan(0,0,1);
        }



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _recognizer.SetInputToDefaultAudioDevice();
            _recognizer.LoadGrammarAsync(new Grammar(
                new GrammarBuilder(
                    new Choices(
                        File.ReadAllLines(@"DefaultCommands.txt")))));
            _recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(Default_SpeechRecognized);
            _recognizer.SpeechDetected += new EventHandler<SpeechDetectedEventArgs>(_recognizer_SpeechRecognized);
            _recognizer.RecognizeAsync(RecognizeMode.Multiple);

            startListening.SetInputToDefaultAudioDevice();
            startListening.LoadGrammarAsync(new Grammar(
                new GrammarBuilder(new Choices(
                    File.ReadAllLines(@"StartCommands.txt")))));
            startListening.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(startListening_SpeechRecognized);

            Timer.Start();
            
        }

        private void Default_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string speech = e.Result.Text;


            if (speech == "Hello")
            {
                Mai.SpeakAsync("Hello, I am here");
            }
            if (speech == "What time is it")
            {
                Mai.SpeakAsync("It is " + DateTime.Now.Hour.ToString() + " " + DateTime.Now.Minute.ToString());
            }
            if (speech == "Can you open blender")
            {
                Mai.SpeakAsync("Your blender is opening");
                Process process = Process.Start(@"C:\Program Files\Blender Foundation\Blender 2.93\blender"); //Path to blender program

            }
            if (speech == "Can you open browser")
            {
                Mai.SpeakAsync("Your browser is opening");
                Process process = Process.Start(@"C:\Users\Legion\AppData\Local\Programs\Opera GX\launcher.exe"); //Path to Your browser 

            }
            if (speech == "Can you play music for me")
            {
                Mai.SpeakAsync("Sure, no problem ");
                Process process = Process.Start(@"C:\Program Files (x86)\Windows Media Player\wmplayer.exe"); //Path to music or mp3 Player
            }
            if (speech == "Silent please")
            {
                Mai.SpeakAsyncCancelAll();
                Mai.SpeakAsync("ok ");
            }
            if (speech == "Stop listening")
            {
                Mai.SpeakAsync("I will be waiting");
                _recognizer.RecognizeAsyncCancel();
                startListening.RecognizeAsync(RecognizeMode.Multiple);
            }
            if (speech == "Close program")
            {
                Environment.Exit(1);
            }
            if (speech == "Show commands")
            {
                string[] commands = (File.ReadAllLines(@"defaultCommands.txt"));
                LBCommands.Items.Clear();
                LBCommands.SelectionMode = SelectionMode.Single;
                LBCommands.Visibility = Visibility.Visible;
                foreach (string item in commands)
                {
                    LBCommands.Items.Add(item);
                }
            }

            if (speech == "Hide commands")
            {
                LBCommands.Visibility = Visibility.Hidden;
            }
        }

        private void _recognizer_SpeechRecognized(object sender, SpeechDetectedEventArgs e)
        {
            RecTimeOut = 0;
        }

        private void startListening_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string speech = e.Result.Text;

            if (speech == "Wake up")
            {
                startListening.RecognizeAsyncCancel();
                Mai.SpeakAsync("I am ready to work");
                _recognizer.RecognizeAsync(RecognizeMode.Multiple);
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (RecTimeOut == 10)
            {
                _recognizer.RecognizeAsyncCancel();
            }
            else if (RecTimeOut == 11)
            {
                Timer.Stop();
                startListening.RecognizeAsync(RecognizeMode.Multiple);
                RecTimeOut = 0;
            }
        }
    }
}
