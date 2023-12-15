namespace AptTec.Net.TextRuleEngine.xUnit
{
    public class TextMatcherUnitTest
    {
        private TextMatcher matcher = new TextMatcher(StringComparison.OrdinalIgnoreCase);

/*
* Fact: A Fact in xUnit is a test case that does not take any external input. It is a test method that always behaves in the same way and has the same result. If a Fact is executed with the same initial conditions, it always checks the same conditions and always behaves the same way.

* Theory: A Theory in xUnit is a test case that takes external input. It is a parameterized test method that is executed multiple times with different input arguments. Theories allow you to run a single test method with different data, verifying that the method behaves correctly with various inputs.
*/
        /// <summary>
        /// xUnit uses [Fact] for test cases and [Theory] for parameterized test cases.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pattern"></param>
        /// <param name="isNot"></param>
        /// <param name="operatorType"></param>
        [Theory]
        [InlineData("Hello, World!", "Hello", false, OperatorType.StartsWith)]
        [InlineData("Hello, World!", "World!", false, OperatorType.EndsWith)]
        [InlineData("Hello, World!", ", ", false, OperatorType.Contains)]
        [InlineData("Hello, World!", @"^\w+, \w+!$", false, OperatorType.Regex)]
        [InlineData("Hello, World!", "Hello, World!", false, OperatorType.Equals)]
        [InlineData("Hello, World!", "H*o, W*d!", false, OperatorType.Wildcard)]

        [InlineData("Robert", "Rupert", false, OperatorType.Soundex)]
        [InlineData("Knight", "kngt", false, OperatorType.Soundex)]
        [InlineData("Colour", "Color", false, OperatorType.Soundex)]

        [InlineData("123.45", "100", false, OperatorType.GreaterThan)]
        [InlineData("123.45", "200", false, OperatorType.LessThan)]
        [InlineData("123.45", "123.45", false, OperatorType.GreaterThanOrEqual)]
        [InlineData("123.45", "123.45", false, OperatorType.LessThanOrEqual)]
        public void TestOperators(string input, string pattern, bool isNot, OperatorType operatorType)
        {
            List<Rule> rules = new List<Rule>
        {
            new Rule { OperatorType = operatorType, Pattern = pattern, IsNot = isNot }
        };

            Assert.True(matcher.Match(input, rules));
        }
    }
}