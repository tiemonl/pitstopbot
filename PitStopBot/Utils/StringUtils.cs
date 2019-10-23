using System;
namespace PitStopBot.Utils {
    public class StringUtils {
        public StringUtils() {
        }

        public static string RenameType(string type) {
            return type == "wheels" ? "Wheels" : type == "casing" ? "Body" : type == "spoiler" ? "Rear" : "Front";
        }
    }
}
