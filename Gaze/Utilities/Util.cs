using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace Gaze.Utilities
{
    class Util
    {
        public static string HashSHA1(string input)
        {
            var hash = (new System.Security.Cryptography.SHA1Managed()).ComputeHash(Encoding.UTF8.GetBytes(input));
            return string.Join("", hash.Select(b => b.ToString("x2")).ToArray());
        }

        public static void Speak(String message, VoiceGender gender = VoiceGender.Male, int age = 30)
        {
            // Initialize a new instance of the SpeechSynthesizer.
            SpeechSynthesizer synth = new SpeechSynthesizer();
            synth.SelectVoiceByHints(gender, DetermineAge(age));
            // Configure the audio output. 
            synth.SetOutputToDefaultAudioDevice();
            // Speak a string.
            synth.SpeakAsync(message);
        }

        public static VoiceAge DetermineAge(int age)
        {
            if(age < 11)
            {
                return VoiceAge.Child;
            }
            if(age < 16)
            {
                return VoiceAge.Teen;
            }
            if(age < 45)
            {
                return VoiceAge.Adult;
            }
            return VoiceAge.Senior;
        }
    }
}
