using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace XorBrute {
	class Program {
		public static byte[] strToBA(string hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
        public static string BAtoStr(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }

        public static Dictionary<byte[], double> HighConfidence = new Dictionary<byte[], double>();

        public static string[] commonWords = File.ReadAllLines("wordlist.txt");

        public static string basePass(int entry, bool mode){
        	int lenBase = 0;
        	if (mode) {
        		lenBase = Az09Alphabet.Length;
        	} else {
        		lenBase = Az09Alphabet.Length - 10;
        	}

        	if (entry == 0){
        		return Az09Alphabet[0].ToString();
        	}

        	List<char> res = new List<char>();
        	while (entry != 0){
        		res.Add(Az09Alphabet[entry % lenBase]);
        		entry = entry / lenBase;
        	}

        	res.Reverse();

        	string result = "";
        	foreach (char cc in res){
        		result += cc;
        	}

        	return result;
        }

        public static KeyValuePair<byte[], double> CurrentBest = new KeyValuePair<byte[], double>();

        public static char[] Az09Alphabet = new char[] {'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
    'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};
		
		public static void CheckPassword(string currPass, byte[] encCont, bool useWords, double threshold){
			byte[] key = Encoding.UTF8.GetBytes(currPass);
			byte[] output = new byte[encCont.Length];
			int totalAlphaNum = 0;
			for (int a = 0; a<output.Length; a++){
				output[a] = (byte)(encCont[a] ^ key[a % key.Length]);
				if ((output[a] >= 0x41 && output[a] <= 0x5a) ||
					(output[a] >= 0x61 && output[a] <= 0x7a) ||
					(output[a] == 0x20 || output[a] == 0x21 ||
					output[a] == 0x22 || output[a] == 0x27 || output[a] == 0x3f)){
					totalAlphaNum++;
				}
			}

			double confidence = ((double)totalAlphaNum / output.Length) * 100;
			if (useWords && confidence > 75){
				string result = Encoding.UTF8.GetString(output);
				string[] words = result.Split(' ');

				for (int z = 0; z<words.Length; z++){
					if (commonWords.Contains(words[z])){
						confidence += 25;
					}
				}
			}
			if (confidence >= threshold){
				HighConfidence.Add(key, confidence);
			}
			if (confidence >= CurrentBest.Value){
				CurrentBest = new KeyValuePair<byte[], double>(key, confidence);
			}
			if (confidence >= 120){
				Console.WriteLine("High confidence key : "+currPass + " | "+(Math.Round(confidence, 2)+" units"));
			}
		}

		public static void Main(string[] args){
			Console.WriteLine("Preparing the wordlist...");
			for (int b = 0; b<commonWords.Length; b++){
				commonWords[b] = commonWords[b].ToLower();
			}
			Console.WriteLine("Done!");
			Console.Write("Enter encrypted text (HEX), or a path starting with '=' to load the file from:");
			string achoice = Console.ReadLine();
			byte[] encCont;
			if (achoice.StartsWith("=")){
				encCont = File.ReadAllBytes(achoice.Substring(1));
			} else {
				encCont = strToBA(achoice);
			}
			Console.WriteLine("Select mode:");
			Console.WriteLine("1. A-z");
			Console.WriteLine("2. A-z 0-9");
			Console.Write(">");
			bool mode = Console.ReadLine() == "2";
			Console.WriteLine("Enter the maximal length of the key");
			Console.Write(">");
			int maxLen = int.Parse(Console.ReadLine());
			Console.WriteLine("Enter the A-z threshold for key testing (Around 85 is fine. Usually >150 confidence is for the real key)");
			Console.Write(">");
			double threshold = double.Parse(Console.ReadLine());
			Console.WriteLine("Do you want us to use wordlist to increase accuracy of key guesses? It is slower, but not using it may lead to incorrect, but similar keys. (Y/n)");
			Console.Write(">");
			bool useWList = Console.ReadLine().ToLower() == "y";
			Console.WriteLine("Preparing for brute...");
			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();
			
			bool flagdone = false;
			new Thread(() =>
			{
					Thread.CurrentThread.IsBackground = true;
					Thread.Sleep(500);
					while (!flagdone){
						if (HighConfidence.Count == 0) {
							Thread.Sleep(500); continue;
						}
						Console.ForegroundColor = ConsoleColor.DarkGreen;
						Console.WriteLine("Current best guess : "+CurrentBest.Value+" confidence. "+Encoding.UTF8.GetString(CurrentBest.Key));
						Console.ResetColor();
						Thread.Sleep(2000);
					}
			}).Start();

			for (int i = 1; i<=maxLen; i++){
				long amountOfCombinations = 0;
				if (mode) {
					amountOfCombinations = (long)Math.Round(Math.Pow(Az09Alphabet.Length, i));
				} else {
					amountOfCombinations = (long)Math.Round(Math.Pow(Az09Alphabet.Length-10, i));
				}
				Console.WriteLine("Bruteforcing for "+i+" characters... Approx. "+amountOfCombinations+" combinations.");
				for (int o = i != 1 ? (int)Math.Round(Math.Pow(Az09Alphabet.Length, i-1)) : 0; o<amountOfCombinations; o++){

					string currPass = basePass(o, mode);
					
					CheckPassword(currPass, encCont, useWList, threshold);
				}
			}
			flagdone = true;
			Console.WriteLine("Bruteforce is done. Found "+HighConfidence.Count+" good entries!");
			Console.WriteLine("Sorting...");

			var resultConf = HighConfidence.OrderBy(obj => obj.Value).ToArray().Reverse().ToDictionary(x => x.Key, x => x.Value);

			var resArray = resultConf.ToArray();

			Console.WriteLine("Done!");
			stopWatch.Stop();
			Console.WriteLine("Time taken: "+stopWatch.Elapsed.TotalSeconds+" seconds.");
			byte[] KeyQueue = null;
			while (true){
				Console.WriteLine("- TOP 20 Best matches -");
				for (int i = 0; i<20; i++) {
					Console.WriteLine("["+(i+1)+"] "+Encoding.UTF8.GetString(resArray[i].Key)+" : Confidence: "+resArray[i].Value);
				}
				if (KeyQueue != null){
					Console.WriteLine("------------");
					Console.WriteLine("KEY: "+Encoding.UTF8.GetString(KeyQueue));
					Console.WriteLine("Confidence: "+resultConf[KeyQueue]);
					string resEnc = "";
					for (int i = 0; i<encCont.Length; i++){
						resEnc += (char)(encCont[i] ^ KeyQueue[i % KeyQueue.Length]);
					}
					Console.WriteLine("Result: "+resEnc);
				}
				Console.WriteLine("------------");
				Console.Write("Enter ID to reveal the decryption or enter 'OUT' to save to output.txt:");
				string answ = Console.ReadLine();
				if (answ.ToUpper() != "OUT"){
					KeyQueue = resArray[int.Parse(answ) - 1].Key;
				} else {
					Console.Write("Please, enter confidence threshold to use (or enter "+threshold+"):");
					double conf = double.Parse(Console.ReadLine());
					Console.WriteLine("Saving... please wait");
					FileStream fs = File.Create("output.txt");
					for (int i = 0; i<resArray.Length; i++){
						
						if (resArray[i].Value < conf) continue;

						AddText(fs, i+":"+Encoding.UTF8.GetString(resArray[i].Key)+":"+resArray[i].Value+":");
						string resEnc = "";
						for (int b = 0; b<encCont.Length; b++){
							resEnc += (char)(encCont[b] ^ resArray[i].Key[b % resArray[i].Key.Length]);
						}
						AddText(fs, resEnc+"\r\n");
					}
					fs.Flush();
					fs.Close();
					Console.WriteLine("Success!");
				}
				Console.WriteLine();
			}
		}
		private static void AddText(FileStream fs, string value)
    	{
        	byte[] info = Encoding.UTF8.GetBytes(value);
        	fs.Write(info, 0, info.Length);
    	}
	}
}