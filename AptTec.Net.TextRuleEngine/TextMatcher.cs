using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Data;
using System.Text;
using System.Linq;

namespace AptTec.Net.TextRuleEngine
{
    public enum OperatorType
    {
        StartsWith,
        EndsWith,
        Contains,
        Regex,
        Equals,
        Wildcard,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
        Soundex
    }

    public class Rule
    {
        public OperatorType OperatorType { get; set; }
        public string Pattern { get; set; }
        public bool IsNot { get; set; }
    }
    public class TextMatcher
    {
        private StringComparison comparisonType;

        public TextMatcher(StringComparison comparisonType)
        {
            this.comparisonType = comparisonType;
        }

        public bool Match(string input, List<Rule> rules)
        {
            foreach (var rule in rules)
            {
                bool result;

                switch (rule.OperatorType)
                {
                    case OperatorType.StartsWith:
                        result = input.StartsWith(rule.Pattern, comparisonType);
                        break;
                    case OperatorType.EndsWith:
                        result = input.EndsWith(rule.Pattern, comparisonType);
                        break;
                    case OperatorType.Contains:
                        result = input.IndexOf(rule.Pattern, comparisonType) >= 0;
                        break;
                    case OperatorType.Regex:
                        Regex regex = new Regex(rule.Pattern, RegexOptions.IgnoreCase);
                        result = regex.IsMatch(input);
                        break;
                    case OperatorType.Equals:
                        result = input.Equals(rule.Pattern, comparisonType);
                        break;
                    case OperatorType.Wildcard:
                        //result = LikeString(input, rule.Pattern, '?', '*', '#');
                        //break;
                     
                        Regex wildcardRegex = new Regex(WildcardToRegex(rule.Pattern));
                        result = wildcardRegex.IsMatch(input);
                        break;

                    case OperatorType.GreaterThan:
                    case OperatorType.GreaterThanOrEqual:
                    case OperatorType.LessThan:
                    case OperatorType.LessThanOrEqual:
                        decimal inputDecimal, patternDecimal;
                        if (decimal.TryParse(input, out inputDecimal) && decimal.TryParse(rule.Pattern, out patternDecimal))
                        {
                            switch (rule.OperatorType)
                            {
                                case OperatorType.GreaterThan:
                                    result = inputDecimal > patternDecimal;
                                    break;
                                case OperatorType.GreaterThanOrEqual:
                                    result = inputDecimal >= patternDecimal;
                                    break;
                                case OperatorType.LessThan:
                                    result = inputDecimal < patternDecimal;
                                    break;
                                case OperatorType.LessThanOrEqual:
                                    result = inputDecimal <= patternDecimal;
                                    break;
                                default:
                                    throw new ArgumentException("Invalid operator type");
                            }
                        }
                        else
                        {
                            throw new ArgumentException("Input or pattern is not a valid decimal");
                        }
                        break;
                    case OperatorType.Soundex:
                        result = GetSoundex(input) == GetSoundex(rule.Pattern);
                        break;
                    default:
                        throw new ArgumentException("Invalid operator type");
                }

                if (rule.IsNot)
                    result = !result;

                if (result)
                    return true;
            }
            return false;
        }
        private string WildcardToRegex(string pattern)
        {
            return "^" + Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", ".") + "$";
        }

        //private bool LikeString(string str, string pattern, char singleWildcard, char multipleWildcard, char escapeCharacter)
        //{
        //    return new DataTable().Compute($"'{str}' LIKE '{pattern}'", null).Equals(true);
        //} 


        /// <summary>
        /// This code implements the Soundex algorithm, which is a phonetic algorithm for indexing names by sound, 
        /// as pronounced in English. The goal is for homophones to be encoded to the same representation 
        /// so that they can be matched despite minor differences in spelling. 
        /// The algorithm mainly encodes consonants; a vowel will not be encoded unless it is the first letter. 
        /// The Soundex code for a name consists of a letter followed by three numerical digits: 
        /// the letter is the first letter of the name, and the digits encode the remaining consonants. 
        /// Consonants at a similar place of articulation share the same digit so, 
        /// for example, the labial consonants B, F, P, and V are each encoded as the number 1.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string GetSoundex(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            // Define replacement rules
            /*
            a, e, i, o, u, h, w, y → ""
            b, f, p, v → “1”
            c, g, j, k, q, s, x, z → “2”
            d, t → “3”
            l → “4”
            m, n → “5”
            r → “6”
            */
            string[] mapping = new string[26]
            {
                "", "1", "2", "3", "", "1", "2", "", "", "2",
                "2", "4", "5", "5", "", "1", "2", "6", "2", "3",
                "", "1", "", "2", "", "2"
            };


            // Convert text to uppercase and remove non-alphabetic characters
            string cleanText = Regex.Replace(text.ToUpper(), "[^A-Z]", "");

            // Initialize variables for leading code and last code
            char leadingCode = cleanText[0];
            char lastCode = mapping[leadingCode - 'A'][0];

            // Apply Soundex rules
            StringBuilder soundex = new StringBuilder();
            soundex.Append(leadingCode);
            for (int i = 1; i < cleanText.Length; i++)
            {
                int codeIndex = cleanText[i] - 'A';
                string mappingCode = mapping[codeIndex];

                // Check if code is not empty and differs from last code
                if (mappingCode.Length > 0 && mappingCode[0] != lastCode)
                {
                    // Append code and update last code
                    soundex.Append(mappingCode[0]);
                    lastCode = mappingCode[0];
                }
            }

            // Pad or truncate to 4 characters
            while (soundex.Length < 4)
                soundex.Append('0');
            if (soundex.Length > 4)
                soundex.Length = 4;
            return soundex.ToString();
        }


    }
}