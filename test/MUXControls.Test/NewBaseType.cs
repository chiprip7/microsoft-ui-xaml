// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Common;

#if USING_TAEF
using WEX.TestExecution;
using WEX.TestExecution.Markup;
using WEX.Logging.Interop;
#else
#endif

namespace MUXTestInfra.Shared.Infra
{
    public class NewBaseType
    {
        public static IScanner AxeScanner
        {
            get
            {
                if (GetScanner() == null)
                {
                    LoadScanner();
                }
                return GetScanner();
            }
        }
    }

    public class NewBaseType : NewBaseType
    {
        private static IScanner scanner = null;

        internal static IScanner Scanner
        {
            get
            {
                return scanner;
            }

            set
            {
                scanner = value;
            }
        }

        internal static IScanner GetScanner()
        {
            return scanner;
        }

        internal static void SetScanner(IScanner value)
        {
            scanner = value;
        }

        public static void TestForAxeIssues()
        {
            var result = AxeScanner.Scan();

            foreach (var error in result.Errors)
            {
                Log.Error($"{error.ToString()} - {error.Element.ToString()} - {error.Rule.ToString()} - {error.Rule.HowToFix}");
            }

            Verify.AreEqual(0, result.ErrorCount, "Found " + result.ErrorCount + " Axe errors.");
        }

        private static void LoadScanner()
        {
            var processes = Process.GetProcessesByName("MUXControlsTestApp");
            Verify.IsTrue(processes.Length > 0);

            string directory = Environment.GetEnvironmentVariable("TEMP") + @"\"; // For instance C:\Users\TDPUser\AppData\Local\Temp\
            var config = Config.Builder.ForProcessId(processes[0].Id).WithOutputFileFormat(OutputFileFormat.A11yTest).WithOutputDirectory(directory).Build();
            SetScanner(ScannerFactory.CreateScanner(config));
        }
    }
}
