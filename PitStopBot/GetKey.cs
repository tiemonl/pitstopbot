using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace PitStopBot {
    public class GetKey {
        public string Get(string bot) {
            string keyPath = null;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                keyPath = Path.Combine("..", "..", "..", "Tokens", $"{bot}.token");
            else {
                keyPath = Path.Combine("Tokens", $"{bot}.token");
            }
            return ReadFile(keyPath);
        }

        public string GetAPI(string api) {
            string keyPath = null;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                keyPath = Path.Combine("..", "..", "..", "Tokens", $"{api}.key");
            else {
                keyPath = Path.Combine("Tokens", $"{api}.key");
            }
            return ReadFile(keyPath);
        }

        private string ReadFile(string keyPath) {
            if (File.Exists(keyPath)) {
                using (StreamReader sr = new StreamReader(keyPath, Encoding.UTF8)) {
                    string key = sr.ReadToEnd();
                    return key;
                }
            }
            return null;
        }
    }
}
