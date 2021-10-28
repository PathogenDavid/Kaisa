using System;
using System.IO;
using System.Text;

namespace Kaisa.Sample
{
    public sealed partial class FancyConsoleWriter : TextWriter
    {
        private const int IndentSize = 2;
        private int IndentLevel { get; set; } = 0;
        private bool OnNewLine = true;
        private string? LinePrefix = null;
        private bool NoSeparationNeeded = true;
        private readonly StringBuilder OutputAccumulator = new();

        private readonly TextWriter Output;
        public override Encoding Encoding => Output.Encoding;

        public FancyConsoleWriter(TextWriter output)
            => Output = output;

        public void WriteLineLeftAdjusted(string value)
        {
            if (!OnNewLine)
            { throw new InvalidOperationException("Cannot write a left-adjusted line when the current line already contains text."); }

            int oldIndentLevel = IndentLevel;
            try
            {
                IndentLevel = 0;
                WriteLine(value);
            }
            finally
            { IndentLevel = oldIndentLevel; }
        }

        public void WriteLineIndented(string value)
        {
            if (!OnNewLine)
            { throw new InvalidOperationException("Cannot write an indented line when the current line already contains text."); }

            using (Indent())
            { WriteLine(value); }
        }

        public void NoSeparationNeededBeforeNextLine()
            => NoSeparationNeeded = true;

        public void EnsureSeparation()
        {
            if (NoSeparationNeeded)
            { return; }

            WriteLine();
            NoSeparationNeeded = true;
        }

        public sealed override void Write(char value)
        {
            NoSeparationNeeded = false;

            // Write out indent if we are starting a new line, but only if the line isn't empty
            // (This assumes a carriage return never appears outside of a newline, which is a safe assumption for any valid files.)
            if (OnNewLine && value != '\r' && value != '\n')
            {
                OnNewLine = false;

                for (int i = 0; i < IndentLevel * IndentSize; i++)
                { Write(' '); }

                if (LinePrefix is not null)
                { Write(LinePrefix); }
            }

            // Write out the actual content with the underlying writer
            OutputAccumulator.Append(value);

            // If this character started a newline, update onNewLine so we know to indent the next line
            if (value == '\n')
            {
                OnNewLine = true;
                Output.Write(OutputAccumulator);
                OutputAccumulator.Clear();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Output.Write(OutputAccumulator);
                OutputAccumulator.Clear();
            }

            base.Dispose(disposing);
        }
    }
}
