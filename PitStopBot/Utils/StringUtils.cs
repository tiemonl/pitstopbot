namespace PitStopBot.Utils {
    public class StringUtils {

        public static string RenameType(string type) {
            return type == "wheels" ? "Wheels" :
                type == "casing" ? "Body" :
                type == "spoiler" ? "Rear" : "Front";
        }


        //highlight bigger value
        public static string ComparisonFormatterGreaterThan(int a, int b) {
            return a > b ? $"***{a}***\n{b}" : (a == b) ? $"{a}\n{b}" : $"{a}\n***{b}***";
        }
        //highlight lower value
        public static string ComparisonFormatterLesserThan(int a, int b) {
            return a < b ? $"***{a}***\n{b}" : (a == b) ? $"{a}\n{b}" : $"{a}\n***{b}***";
        }
    }
}
