﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;

namespace ORegex.FSM
{
    public class DFSM<TValue>
    {
        public readonly List<string> Q = new List<string>();
        public readonly List<Func<TValue,bool>> Sigma = new List<Func<TValue, bool>>();
        public readonly List<Transition<TValue>> Delta = new List<Transition<TValue>>();
        public List<string> Q0 = new List<string>();
        public readonly List<string> F = new List<string>();

        public DFSM(IEnumerable<string> q, IEnumerable<Func<TValue, bool>> sigma, IEnumerable<Transition<TValue>> delta,
            IEnumerable<string> q0, IEnumerable<string> f)
        {
            Q = q.ToList();
            Sigma = sigma.ToList();
            AddTransitions(delta);
            AddInitialStates(q0);
            AddFinalStates(f);
        }

        private void AddTransitions(IEnumerable<Transition<TValue>> transitions)
        {
            foreach (var transition in transitions.Where(ValidTransition))
            {
                Delta.Add(transition);
            }
        }

        private bool ValidTransition(Transition<TValue> transition)
        {
            return Q.Contains(transition.StartState) &&
                   Q.Contains(transition.EndState) &&
                   Sigma.Contains(transition.Symbol) &&
                   !TransitionAlreadyDefined(transition);
        }

        private bool TransitionAlreadyDefined(Transition<TValue> transition)
        {
            return Delta.Any(t => t.StartState == transition.StartState &&
                                  t.Symbol == transition.Symbol);
        }

        private void AddInitialStates(IEnumerable<string> q0)
        {
            foreach (var startingState in q0.Where(q => q != null && Q.Contains(q)))
            {
                Q0.Add(startingState);
            }
        }

        private void AddFinalStates(IEnumerable<string> finalStates)
        {
            foreach (var finalState in finalStates.Where(finalState => Q.Contains(finalState)))
            {
                F.Add(finalState);
            }
        }

        public void Accepts(TValue[] input)
        {
            ConsoleWriter.Success("Trying to parse: " + input);
            if (InvalidInputOrFSM(input))
            {
                return;
            }
            foreach (var q0 in Q0)
            {
                var currentState = q0;
                var steps = new StringBuilder();
                foreach (var symbol in input)
                {
                    var transition = Delta.Find(t => t.StartState == currentState &&
                                                     t.Symbol(symbol));
                    if (transition == null)
                    {
                        ConsoleWriter.Failure("No transitions for current state and symbol");
                        ConsoleWriter.Failure(steps.ToString());
                        continue;
                    }
                    currentState = transition.EndState;
                    steps.Append(transition + "\n");
                }
                if (F.Contains(currentState))
                {
                    ConsoleWriter.Success("Accepted the input with steps:\n" + steps);
                    return;
                }
                ConsoleWriter.Failure("Stopped in state " + currentState +
                                      " which is not a final state.");
                ConsoleWriter.Failure(steps.ToString());
            }
        }

        private bool InvalidInputOrFSM(TValue[] input)
        {
            if (InputContainsNotDefinedSymbols(input))
            {
                return true;
            }
            if (InitialStateNotSet())
            {
                ConsoleWriter.Failure("No initial state has been set");
                return true;
            }
            if (NoFinalStates())
            {
                ConsoleWriter.Failure("No final states have been set");
                return true;
            }
            return false;
        }

        private bool InputContainsNotDefinedSymbols(TValue[] input)
        {
            foreach (var symbol in input.Where(symbol => !Sigma.Any(x => x(symbol))))
            {
                ConsoleWriter.Failure("Could not accept the input since the symbol " + symbol +
                                      " is not part of the alphabet");
                return true;
            }
            return false;
        }

        private bool InitialStateNotSet()
        {
            return Q0.Count == 0;
        }

        private bool NoFinalStates()
        {
            return F.Count == 0;
        }

        public void RemoveUnreachableStates()
        {
            var reachableStates = new HashSet<string>(Q0);
            var newStates = new HashSet<string>(Q0);
            do
            {
                var temp = new HashSet<string>();
                foreach (var q in newStates)
                {
                    var reachableFromQ = Delta.FindAll(t => t.StartState == q).Select(t => t.EndState);
                    temp.UnionWith(reachableFromQ);
                }
                temp.ExceptWith(reachableStates);
                newStates = temp;
                reachableStates.UnionWith(newStates);
            } while (newStates.Count > 0);
            var unreachableStates = Q.Where(q => !reachableStates.Contains(q));
            for (int i = Delta.Count - 1; i > 0; i--)
            {
                var transition = Delta[i];
                if (unreachableStates.Contains(transition.EndState) ||
                    unreachableStates.Contains(transition.StartState))
                {
                    Delta.Remove(transition);
                }
            }
            foreach (var unrechableState in unreachableStates)
            {
                Q.Remove(unrechableState);
            }
        }
    }
}