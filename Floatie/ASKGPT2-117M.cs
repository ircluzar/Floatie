using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace Floatie
{

	public enum BotColoring
    {
		NEUTRAL,
		HAPPY,
		TOXIC
    }
	public static class AskGPT2_117M
	{
		//This Parses GPT-2 by simulating the form of this page:
		//https://minimaxir.com/apps/gpt2-small/

		public static BotColoring behavior = BotColoring.NEUTRAL;
		public static string toxicColoring = "holy fucking shit";
		public static string happyColoring = "i'm so happy right now";

		//public static string coloring = "What a wonderful day"; // coloring for the query, makes the answers more toxic or inject a topic
		
		public static string coloring
        {
            get
            {
				switch(behavior)
                {
					default:
					case BotColoring.NEUTRAL:
						return "";
					case BotColoring.HAPPY:
						return happyColoring;
					case BotColoring.TOXIC:
						return toxicColoring;
                }
            }
        }
		
		public static string fallback = "i'm really happy today"; //default value if no seed is specified

		public static string length = "100";
		public static string temperature = "0.8";
		public static string top_k = "60";


		public static string GetMessage(string seed)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(seed))
					seed = fallback;

				seed = seed.Replace("?", "");

				string prefix = coloring + $" {seed.Trim().Replace(".", " ")}. ";

				//String postData = "prefix=" + prefix + "&length=" + length + "&temperature=" + temperature + "&top_k=" + top_k;
				String postData = "{\"prefix\":\"" + prefix + "\",\"length\":\"" + length + "\",\"temperature\":\"" + temperature + "\",\"top_k\":\"" + top_k + "\"}";

				WebRequest request = WebRequest.Create("https://gpt2-default-dstdu4u23a-uc.a.run.app/");
				request.Method = "POST";
				(request as HttpWebRequest).Referer = @"https://minimaxir.com/apps/gpt2-small/";
				byte[] byteArray = Encoding.UTF8.GetBytes(postData);
				request.ContentType = "application/x-www-form-urlencoded";
				request.ContentLength = byteArray.Length;
				Stream dataStream = request.GetRequestStream();
				dataStream.Write(byteArray, 0, byteArray.Length);
				// Close the Stream object.
				dataStream.Close();
				// Get the response.
				WebResponse response = request.GetResponse();
				// Display the status.
				string stat = ((HttpWebResponse)response).StatusDescription;

				if (!stat.Contains("OK"))
					return "I don't know";

				//Console.WriteLine(((HttpWebResponse)response).StatusDescription);
				// Get the stream containing content returned by the server.
				dataStream = response.GetResponseStream();
				// Open the stream using a StreamReader for easy access.
				StreamReader reader = new StreamReader(dataStream);
				// Read the content.
				string responseFromServer = reader.ReadToEnd();
				// Display the content.

				var decoded = JObject.Parse(responseFromServer);

				string returntext = decoded.SelectToken("text").Value<string>();

				string onlyresponse;
				try
				{
					onlyresponse = returntext.Substring(returntext.IndexOf(".") + 1).Trim();
				}
				catch
				{
					onlyresponse = returntext.Trim();
				}

				string choppedresponse;
				//try
				//{
				//	choppedresponse = onlyresponse.Substring(0, onlyresponse.IndexOf("."));
				//}
				//catch
				//{
				//	choppedresponse = onlyresponse.Trim();
				//}
				choppedresponse = onlyresponse.Trim();

				string purify = choppedresponse;
				purify = purify.Replace("<|endoftext|>", "");

				return purify;

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				return "I don't know";
			}
		}

	}
}
