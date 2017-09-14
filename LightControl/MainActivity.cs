using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Speech;
using Android.Widget;
using System;
using System.Collections.Generic;

namespace LightControl
{
    [Activity(Label = "Lights Control")]
    public class MainActivity : Activity, IRecognitionListener
    {

        //private Button buttonVoice;
        private ImageButton buttonMic;
        private TextView textMessage;
        private SpeechRecognizer messageRecognizer = null;
        private SeekBar seekBarIntensidade;
        private Intent recoIntent;
        private List<String> commands = new List<String>{ "acender cozinha", "apagar cozinha", "acender sala", "apagar sala", "acender quarto", "apagar quarto", "acender banheiro", "apagar banheiro", "acender todos", "apagar todos" };

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            //buttonVoice = FindViewById<Button>(Resource.Id.buttonVoice);
            //buttonVoice.Touch += ButtonVoice_Touch;
            buttonMic = FindViewById<ImageButton>(Resource.Id.imageButtonMic);
            buttonMic.Touch += ButtonMic_Touch;

            textMessage = FindViewById<TextView>(Resource.Id.textViewSaida);

            seekBarIntensidade = FindViewById<SeekBar>(Resource.Id.seekBarIntensidade);
            seekBarIntensidade.ProgressChanged += SeekBarIntensidade_ProgressChanged;

            recoIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            recoIntent.PutExtra(RecognizerIntent.ExtraLanguage, "pt");
            recoIntent.PutExtra(RecognizerIntent.ExtraLanguagePreference, "pt");
            recoIntent.PutExtra(RecognizerIntent.ExtraOnlyReturnLanguagePreference, "pt");
            recoIntent.PutExtra(RecognizerIntent.ExtraCallingPackage, this.PackageName);
            recoIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelWebSearch);

            messageRecognizer = SpeechRecognizer.CreateSpeechRecognizer(this);
            messageRecognizer.SetRecognitionListener(this);

            Android.Graphics.Typeface font = Android.Graphics.Typeface.CreateFromAsset(this.Assets, "NexaLight.otf");
            var text = FindViewById<TextView>(Resource.Id.textViewAppTitle);
            text.Typeface = font;
        }

        private void SeekBarIntensidade_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            textMessage.Text = "Intensidade: " + seekBarIntensidade.Progress + "%";
        }

        private void ButtonMic_Touch(object sender, Android.Views.View.TouchEventArgs e)
        {
            if (e.Event.Action == Android.Views.MotionEventActions.Down)
            {
                buttonMic.SetImageResource(Resource.Drawable.mic_clicked);
                messageRecognizer.StartListening(recoIntent);
            }
            else if (e.Event.Action == Android.Views.MotionEventActions.Up)
            {
                buttonMic.SetImageResource(Resource.Drawable.mic_unclicked);
                messageRecognizer.StopListening();
            }
        }
        /*
        private void ButtonVoice_Touch(object sender, Android.Views.View.TouchEventArgs e)
        {
            if (e.Event.Action == Android.Views.MotionEventActions.Down)
            {
                buttonVoice.Text = "Parar de ouvir";
                messageRecognizer.StartListening(recoIntent);
            }
            else if (e.Event.Action == Android.Views.MotionEventActions.Up)
            {
                buttonVoice.Text = "Ouvir";
                messageRecognizer.StopListening();          
            }
        }*/

        public void OnBeginningOfSpeech()
        {
            //throw new NotImplementedException();
        }

        public void OnBufferReceived(byte[] buffer)
        {
            //throw new NotImplementedException();
        }

        public void OnEndOfSpeech()
        {
            //throw new NotImplementedException();
        }

        public void OnError([GeneratedEnum] SpeechRecognizerError error)
        {
           // throw new NotImplementedException();
        }

        public void OnEvent(int eventType, Bundle @params)
        {
            //throw new NotImplementedException();
        }

        public void OnPartialResults(Bundle partialResults)
        {
        }

        public void OnReadyForSpeech(Bundle @params)
        {
            //throw new NotImplementedException();
        }

        public void OnResults(Bundle results)
        {
            var arrayCommands = results.GetStringArrayList(SpeechRecognizer.ResultsRecognition);

            try
            {
                string commandToSend = RecognizeCommand(arrayCommands);
                Bluetooth.SendMessage(commandToSend + "/" + seekBarIntensidade.Progress);
            }
            catch (Exception exception)
            {
                if (exception is ConnectionClosedException)
                {
                    Toast.MakeText(this, exception.Message + "\nTentando conectar-se novamente...", ToastLength.Long).Show();
                    try
                    {
                        Bluetooth.RetryConnection();
                    }
                    catch (Exception)
                    {
                        Toast toast = Toast.MakeText(this, "Falha ao se conectar. O app será fechado.", ToastLength.Long);
                        toast.Show();
                    }
                } else if (exception is NoSuchCommandException)
                {
                    Toast.MakeText(this, exception.Message + "\nPor favor, tente falar novamente.", ToastLength.Long).Show();
                }
            }
        }

        public void OnRmsChanged(float rmsdB)
        {
            //throw new NotImplementedException();
        }

        public String RecognizeCommand(IList<string> possibleResults)
        {
            foreach(string possibleCommand in possibleResults)
            {
                string pCommand = possibleCommand.ToLower();
                foreach(string command in commands)
                {
                    if (pCommand.Equals(command))
                    {
                        Toast.MakeText(this, "Comando reconhecido: " + command, ToastLength.Long).Show();
                        return command;
                    }
                }
            }

            throw new NoSuchCommandException();
        }
    }
}

