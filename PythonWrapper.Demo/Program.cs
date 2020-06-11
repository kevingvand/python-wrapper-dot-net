using System;

namespace PythonWrapper.Demo
{
    class Program
    {
        private static Python python;

        private static readonly string pythonPath = @"C:\Users\Asus\AppData\Local\Programs\Python\Python37-32\python.exe";


        static void Main(string[] args)
        {
            string testScript1 = @"..\..\..\PythonWrapper\Examples\testOne.py";
            string testScript2 = @"..\..\..\PythonWrapper\Examples\testTwo.py";

            //string testScript1 = @"C:\Users\Asus\Documents\Development\PythonWrapper\test.py";
            //string testScript2 = @"C:\Users\Asus\Documents\Development\PythonWrapper\testTwo.py";
            //string testScript3 = @"C:\Users\Asus\Documents\Development\PythonWrapper\doesnotexist.py";

            python = new Python();

            var funcOneRes = python.ExecuteFunction(testScript1, "funcOne"); // Get the full output - including all prints
            var funcTwoRes = python.ExecuteFunction(testScript1, "funcTwo", 5, 10); // Pass parameters
            var funcThreeRes = python.ExecuteFunction<string>(testScript1, "funcThree"); // Retrieve the result as a string if you want to read out arrays.
            var funcThreeResCollection = python.ParseCollection<int>(funcThreeRes); // You can use the parseCollection to parse the string result.
            var funcFourRes = python.ExecuteFunction<double>(testScript1, "funcFour"); // You can also pass the type to force conversion to that type.

            Console.WriteLine($"Func 1: {funcOneRes}");
            Console.WriteLine($"Func 2: {funcTwoRes}");
            Console.WriteLine($"Func 3: {funcThreeRes} - {funcThreeResCollection}");
            Console.WriteLine($"Func 4: {funcFourRes}");

            var fileResult = python.ExecuteFile(testScript2, 1, 2); // Full scripts can also be executed like this.
            Console.WriteLine($"Test script 2 result: {fileResult}");

            try
            {
                python.ExecuteFunction("doesnotexist.py", "none");
            } catch (PythonException ex)
            {
                Console.WriteLine($"Python errors or exceptions can be handled like this. The exception is: {ex.Message}");
            }

            Console.ReadKey();
        }
    }
}
