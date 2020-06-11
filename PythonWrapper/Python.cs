using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace PythonWrapper
{
    public class Python
    {
        public const string DefaultPythonPath = "python";

        #region Private Members

        private string _pythonPath;

        #endregion

        #region Constructors

        public Python(string pythonPath = DefaultPythonPath)
        {
            _pythonPath = pythonPath;
        }

        #endregion

        #region Public Methods

        public string ExecuteFile(string filePath, params object[] arguments)
        {
            return RunCommand($"{filePath} {string.Join(" ", arguments)}");
        }

        public string ExecuteFunction(string filePath, string functionName, params object[] arguments)
        {
            var path = Path.GetDirectoryName(filePath);
            var moduleName = Path.GetFileNameWithoutExtension(filePath);

            return RunCommand($"-c \"import {moduleName}; print({moduleName}.{functionName}({string.Join(", ", arguments)}))\"", path);
        }

        public T ExecuteFunction<T>(string filePath, string functionName, params object[] arguments) where T : IConvertible
        {
            var result = ExecuteFunction(filePath, functionName, arguments);
            var lines = result.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            return (T)Convert.ChangeType(lines.Last(), typeof(T), CultureInfo.InvariantCulture);
        }

        public ICollection<string> ParseCollection(string arrayString)
        {
            return ParseCollection<string>(arrayString);
        }

        public ICollection<T> ParseCollection<T>(string arrayString) where T : IConvertible
        {
            if (!arrayString.StartsWith("[")) throw new PythonException($"Could not parse value ({arrayString}) as list.");

            arrayString = arrayString.Substring(1, arrayString.Length - 2);
            var arrayElements = arrayString.Split(", ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            return arrayElements.Select(el => (T) Convert.ChangeType(el, typeof(T), CultureInfo.InvariantCulture)).ToList();
        }

        #endregion

        #region Private Methods

        internal string RunCommand(string command, string workingDirectory = null)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = _pythonPath;
            start.Arguments = command;
            start.UseShellExecute = false;
            start.CreateNoWindow = true;
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;

            if (workingDirectory != null)
                start.WorkingDirectory = workingDirectory;

            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string error = process.StandardError.ReadToEnd();

                    if (error.Length > 0)
                        throw new PythonException(error);

                    string result = reader.ReadToEnd();
                    return result;
                }
            }
        }

        #endregion

    }
}
