using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Pluralsight.TextToSpeech
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var client = new HttpClient())
            {
                HttpRequestMessage authRequest = 
                    new HttpRequestMessage(HttpMethod.Post, 
                        "https://eastus.api.cognitive.microsoft.com/sts/v1.0/issuetoken");
                
                authRequest.Headers.Add("Ocp-Apim-Subscription-Key","{{ Your Subscription Key }}");

                var authResult = client.SendAsync(authRequest).Result;

                var bearerToken = "Bearer " + authResult.Content.ReadAsStringAsync().Result;
                
                var ssml = "<speak version='1.0' xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang='en-US'>" +
                           "<voice  name='Microsoft Server Speech Text to Speech Voice (en-US, Jessa24kRUS)'>" +
                                "Welcome to Microsoft Cognitive Services <break time=\"100ms\" /> Text-to-Speech API." +
                                "</voice> </speak>";
                
                HttpRequestMessage audioRequest = new HttpRequestMessage(HttpMethod.Post, 
                    "https://eastus.tts.speech.microsoft.com/cognitiveservices/v1");
                
                audioRequest.Content = new StringContent(ssml);
                audioRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/ssml+xml");
                audioRequest.Headers.Authorization = AuthenticationHeaderValue.Parse(bearerToken);
                audioRequest.Headers.UserAgent.Add(new ProductInfoHeaderValue("PluralsightDemo","1.0"));
                audioRequest.Headers.Add("X-Microsoft-OutputFormat","audio-24khz-48kbitrate-mono-mp3");

                var audioResult = client.SendAsync(audioRequest).Result;

                using (var fs = File.Open("speech.mpga",FileMode.Create))
                {
                    audioResult.Content.ReadAsStreamAsync().Result.CopyTo(fs);
                }
            }
        }
    }
}