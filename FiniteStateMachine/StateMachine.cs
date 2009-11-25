using System;
using System.Collections.Generic;

namespace FiniteStateMachine
{
    internal class match
    {
        public string input;
        public string replay;
    }

    internal class rule
    {
        internal rule(match i, string o)
        {
            input = i.input;
            replay = i.replay;
            output = o;
        }

        internal string input;
        internal string output;
        internal string replay;
    }

    internal class ruleset
    {
        internal Queue<rule> rules;

        internal ruleset()
        {
            rules = new Queue<rule>();
        }

        internal void Add(rule r)
        {
            rules.Enqueue(r);
        }
    }

    public class StateMachine
    {
        private StateMachine(ruleset set)
        {
            Queue<rule> pending_rule = set.rules;
            state_factory factory = new state_factory();
            init = factory.state();

            while (pending_rule.Count > 0)
            {
                rule r = pending_rule.Dequeue();
                state state = init;
                if (r.replay != null)
                {
                    foreach (char c in r.replay)
                    {
                        action check_action = state.action[c];
                        if (check_action == null)
                        {
                            pending_rule.Enqueue(r);
                            goto END;
                        }
                        state = check_action.new_state;
                    }
                }
                state replay = state;

                for (int i = 0; i < r.input.Length; i++)
                {
                    char c = r.input[i];
                    if (state.action.ContainsKey(c))
                    {
                        if (i != r.input.Length - 1)
                        {
                            state = state.action[c].new_state;
                            continue;
                        }
                        else
                        {
                            throw new Exception("Conflict rule: " + r.replay + '.' + r.input + ':' + r.output);
                        }
                    }
                    if (i != r.input.Length - 1)
                    {
                        state new_state = factory.state();
                        state.action[c] = new action(c, new_state, null);
                        state = new_state;
                    }
                    else
                    {
                        state.action[c] = new action(c, replay, r.output);
                    }
                }
            END:
                continue;
            }
        }

        public static StateMachine Compile(System.IO.StreamReader s)
        {
            return new StateMachine(lex.Parse(s));
        }

        public void GenerateCode(System.IO.StreamWriter o)
        {
            Queue<state> pending_state = new Queue<state>();
            HashSet<state> processed_state = new HashSet<state>();
            pending_state.Enqueue(init);
            while (pending_state.Count > 0)
            {
                state s = pending_state.Dequeue();
                processed_state.Add(s);
                foreach (KeyValuePair<char, FiniteStateMachine.StateMachine.action> a in s.action)
                {
                    if (a.Value.output == null)
                    {
                        o.WriteLine("{ " + s.state_id + ", '" + a.Key + "', " + a.Value.new_state.state_id + ", nil },");
                    }
                    else
                    {
                        o.WriteLine("{ " + s.state_id + ", '" + (a.Key == '\'' ? "\\'" : a.Key.ToString()) + "', " + a.Value.new_state.state_id + ", \"" + a.Value.output + "\" },");
                    }
                    if (!processed_state.Contains(a.Value.new_state))
                    {
                        pending_state.Enqueue(a.Value.new_state);
                    }
                }
            }
        }

        private class action
        {
            public action(char c, state s, string o)
            {
                new_state = s;
                output = o;
            }
            public state new_state;
            public string output;
        }

        private class state
        {
            public state(int id)
            {
                state_id = id;
                action = new Dictionary<char, action>();
            }
            public int state_id;
            public IDictionary<char, action> action;
        }

        private class state_factory
        {
            public state_factory()
            {
                id = 0;
            }

            public state state()
            {
                return new state(id++);
            }
            private int id;
        }

        private state init;
    }
}
