﻿// Copyright © Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Text;
using System.Text.RegularExpressions;
using fitnesse.mtee.engine;
using fitnesse.mtee.model;

namespace fitnesse.slim.operators {
    public class ParseSymbol: ParseOperator<string> {
        private static readonly Regex symbolPattern = new Regex("(\\$[a-zA-Z]\\w*)");

        public bool TryParse(Processor<string> processor, Type type, Tree<string> parameters, ref object result) {
            string decodedInput = ReplaceSymbols(parameters.Value, processor);
            if (parameters.Value == decodedInput) return false;
            result = processor.Parse(type, decodedInput);
            return true;
        }

        private static string ReplaceSymbols(string input, Processor<string> processor) {
            var result = new StringBuilder();
            int lastMatch = 0;
            for (Match symbolMatch = symbolPattern.Match(input); symbolMatch.Success; symbolMatch = symbolMatch.NextMatch()) {
                string symbolName = symbolMatch.Groups[1].Value;
                if (symbolMatch.Index > lastMatch) result.Append(input.Substring(lastMatch, symbolMatch.Index - lastMatch));
                result.Append(processor.Contains(new Symbol(symbolName)) ? processor.Load(new Symbol(symbolName)).Instance : symbolName);
                lastMatch = symbolMatch.Index + symbolMatch.Length;
            }
            if (lastMatch < input.Length) result.Append(input.Substring(lastMatch, input.Length - lastMatch));
            return result.ToString();
        }

    }
}
