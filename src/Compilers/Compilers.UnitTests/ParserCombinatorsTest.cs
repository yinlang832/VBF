﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using VBF.Compilers.Parsers.Combinators;
using VBF.Compilers.Scanners;
using RE = VBF.Compilers.Scanners.RegularExpression;
using VBF.Compilers;
using System.IO;

namespace Compilers.UnitTests
{
    [TestFixture]
    public class ParserCombinatorsTest
    {
        [Test]
        public void ParserFuncTest()
        {
            Lexicon test = new Lexicon();

            var ID = test.Lexer.DefineToken(RE.Range('a', 'z').Concat(
                (RE.Range('a', 'z') | RE.Range('0', '9')).Many()));
            var NUM = test.Lexer.DefineToken(RE.Range('0', '9').Many1());
            var GREATER = test.Lexer.DefineToken(RE.Symbol('>'));

            var WHITESPACE = test.Lexer.DefineToken(RE.Symbol(' ').Union(RE.Symbol('\t')));

            var p1 = from i in ID
                     from g in GREATER
                     from g2 in GREATER.Where(l => l.PrefixTrivia.Count == 0)
                     from n in NUM
                     select "A";

            var p2 = from i in ID
                     from g in GREATER
                     from g2 in GREATER
                     from n in NUM
                     select "B";

            var parser1 = p1 | p2;


            var info = test.CreateScannerInfo();
            ForkableScannerBuilder builder = new ForkableScannerBuilder(info);
            builder.SetTriviaTokens(WHITESPACE.Index);

            var errorManager = new CompilationErrorManager();
            var context = new ParserContext(errorManager, 1, 2);
            context.DefineDefaultCompilationErrorInfo(0);

            ParserRunner<string> runner = new ParserRunner<string>(parser1, context);

            string source1 = "abc >> 123";
            var sr1 = new SourceReader(new StringReader(source1));

            ForkableScanner scanner1 = builder.Create(sr1);

            var result1 = runner.Run(scanner1);

            Assert.AreEqual("A", result1);
            Assert.AreEqual(0, errorManager.Errors.Count);

            string source2 = "abc > > 123";
            var sr2 = new SourceReader(new StringReader(source2));

            ForkableScanner scanner2 = builder.Create(sr2);

            var result2 = runner.Run(scanner2);
            Assert.AreEqual("B", result2);
            Assert.AreEqual(0, errorManager.Errors.Count);

        }

        [Test]
        public void ParserConvertTest()
        {
            Lexicon test = new Lexicon();

            var ID = test.Lexer.DefineToken(RE.Range('a', 'z').Concat(
                (RE.Range('a', 'z') | RE.Range('0', '9')).Many()));
            var NUM = test.Lexer.DefineToken(RE.Range('0', '9').Many1());
            var GREATER = test.Lexer.DefineToken(RE.Symbol('>'));

            var WHITESPACE = test.Lexer.DefineToken(RE.Symbol(' ').Union(RE.Symbol('\t')));

            var p1 = from i in ID
                     from g in GREATER
                     from g2 in GREATER
                     from n in NUM
                     select 1;

            var parser1 = p1.Convert<float>();

            var info = test.CreateScannerInfo();
            ForkableScannerBuilder builder = new ForkableScannerBuilder(info);
            builder.SetTriviaTokens(WHITESPACE.Index);

            var errorManager = new CompilationErrorManager();
            var context = new ParserContext(errorManager, 1, 2);
            context.DefineDefaultCompilationErrorInfo(0);

            ParserRunner<float> runner = new ParserRunner<float>(parser1, context);

            string source1 = "abc >> 123";
            var sr1 = new SourceReader(new StringReader(source1));

            ForkableScanner scanner1 = builder.Create(sr1);

            var result1 = runner.Run(scanner1);

            Assert.AreEqual(1.0f, result1);
            Assert.AreEqual(0, errorManager.Errors.Count);
        }

        [Test]
        public void ParserCastTest()
        {
            Lexicon test = new Lexicon();

            var ID = test.Lexer.DefineToken(RE.Range('a', 'z').Concat(
                (RE.Range('a', 'z') | RE.Range('0', '9')).Many()));
            var NUM = test.Lexer.DefineToken(RE.Range('0', '9').Many1());
            var GREATER = test.Lexer.DefineToken(RE.Symbol('>'));

            var WHITESPACE = test.Lexer.DefineToken(RE.Symbol(' ').Union(RE.Symbol('\t')));

            var p1 = from i in ID
                     from g in GREATER
                     from g2 in GREATER
                     from n in NUM
                     select "hello";

            var parser1 = p1.TryCast<object>();

            var info = test.CreateScannerInfo();
            ForkableScannerBuilder builder = new ForkableScannerBuilder(info);
            builder.SetTriviaTokens(WHITESPACE.Index);

            var errorManager = new CompilationErrorManager();
            var context = new ParserContext(errorManager, 1, 2);
            context.DefineDefaultCompilationErrorInfo(0);

            ParserRunner<object> runner = new ParserRunner<object>(parser1, context);

            string source1 = "abc >> 123";
            var sr1 = new SourceReader(new StringReader(source1));

            ForkableScanner scanner1 = builder.Create(sr1);

            var result1 = runner.Run(scanner1);

            Assert.AreEqual("hello", result1);
            Assert.AreEqual(0, errorManager.Errors.Count);
        }
    }
}
