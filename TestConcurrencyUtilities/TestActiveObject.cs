using System;
using ConcurrencyUtilities;
using Colorizer = AnsiColor.AnsiColor;

namespace TestConcurrencyUtilities
{
	public static class TestActiveObject
	{
		public static void Run() {
			Channel<string> words = new Channel<string>();
			Channel<string> wordsReversed = new Channel<string>();

			TestActiveObjectWordGenerator wordGenerator = new TestActiveObjectWordGenerator(words);
			TestActiveObjectWordReverser wordReverser = new TestActiveObjectWordReverser(words, wordsReversed);
			TestActiveObjectWordOutputter wordOutputter = new TestActiveObjectWordOutputter(wordsReversed);

			wordOutputter.Start();
			wordReverser.Start();
			wordGenerator.Start();
		}
	}

	class TestActiveObjectWordGenerator: ActiveObjectOutput<string>
	{
		string[] _words;
		int _numWordsOutputted;

		public TestActiveObjectWordGenerator(Channel<string> output): base(output) {
			string phrase = "Hello world! I am a word reversing and printing program.";
			_words = phrase.Split(' ');
			_numWordsOutputted = 0;
		}
		
		protected override string Process() {
			string word = "";
			if (_numWordsOutputted < _words.Length) {
				word = _words[_numWordsOutputted];
				_numWordsOutputted++;
			} else {
				Console.WriteLine(Colorizer.Colorize("{black}TestActiveObjectWordGenerator finished sending words " +
				                                     "to its output channel; Stopping ActiveObject"));
//				Stop();
				(new Semaphore(0)).Acquire();
			}
			return word;
		}
	}

	class TestActiveObjectWordReverser: ActiveObjectInputOutput<string,string>
	{
		public TestActiveObjectWordReverser(Channel<string> input, Channel<string> output): base(input, output) {}

		protected override string Process(string item) {
			char[] charArray = item.ToString().ToCharArray();
			Array.Reverse(charArray);
			return new string(charArray);
		}
	}

	class TestActiveObjectWordOutputter: ActiveObjectInput<string>
	{
		public TestActiveObjectWordOutputter(Channel<string> input): base(input) {}

		protected override void Process(string item) {
			Console.WriteLine(Colorizer.Colorize("{green}" + item));
		}
	}
}