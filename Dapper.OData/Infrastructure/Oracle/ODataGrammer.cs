﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.OData.Infrastructure.Oracle
{
    internal class ODataGrammer
    {
        private readonly Dictionary<string, string> _grammer;
        public ODataGrammer()
        {
            _grammer = new Dictionary<string, string>();
            _grammer.Add("NOT", "!");
            _grammer.Add("EQ", "=");
            _grammer.Add("NE", "!=");
            _grammer.Add("GT", ">");
            _grammer.Add("LT", "<");
            _grammer.Add("GE", ">=");
            _grammer.Add("LE", "<=");
            _grammer.Add("BTW", "BETWEEN");
            _grammer.Add("LK", "LIKE");
        }
        public Dictionary<string, string> GetGrammer()
        {
            return _grammer;
        }
        public string Replace(string word)
        {
            bool isInGrammer = _grammer.TryGetValue(word, out string value);
            return isInGrammer ? value : word;
        }
        public string ReplaceExpression(string expression)
        {
            List<string> newExpression = new List<string>();
            int index = -1;
            foreach (var item in expression.Split(" "))
            {
                index++;
                if (!_grammer.ContainsValue(item.ToUpper()))
                {
                    var trimmed = item.Trim();
                    if (!(item.StartsWith("'") && item.EndsWith("'")))
                    {
                        trimmed = trimmed.ToUpper();
                        newExpression.Add(Replace(trimmed));
                    }
                    else
                    {
                        if (newExpression[index - 1] == _grammer["BTW"] || newExpression[index - 1].ToUpper().Trim() == "AND")
                        {
                            newExpression.Add($"TO_DATE({trimmed}, 'YYYY-MM-DD HH24:MI:SS')");
                        }
                        else
                        {
                            newExpression.Add(trimmed);
                        }
                    }
                }
                else
                {
                    newExpression.Add("$$$");
                }
            }
            return string.Join(" ", newExpression);
        }
    }
}
