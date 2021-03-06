﻿using Eocron.Core.FinitieStateAutomaton;

namespace Eocron.Core.Parse
{
    public sealed class ORegexCompiler<TValue>
    {
        private readonly ORegexParser<TValue> _parser;
        private readonly FSAFactory<TValue> _stb;
        public ORegexCompiler()
        {
            _parser = new ORegexParser<TValue>();
            _stb = new FSAFactory<TValue>();
        }

        public IFSA<TValue> Build(string input, PredicateTable<TValue> predicateTable, ORegexOptions options)
        {
            var ast = _parser.Parse(input, predicateTable);
            var fa = _stb.Create(ast, options);
            return fa;
        }
    }
}
