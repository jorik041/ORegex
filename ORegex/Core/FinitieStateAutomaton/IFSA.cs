﻿using System.Collections.Generic;
using ORegex.Core.Ast;

namespace ORegex.Core.FinitieStateAutomaton
{
    public interface IFSA<TValue>
    {
        string Name { get; }

        Range Run(TValue[] values, int startIndex);

        IEnumerable<IFSATransition<TValue>> Transitions { get; }

        bool IsFinal(int state);
    }
}