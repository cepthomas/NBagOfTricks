using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json.Serialization;
using System.Drawing;
using System.Diagnostics;
using Ephemera.NBagOfTricks;
using Ephemera.NBagOfTricks.PNUT;


namespace Ephemera.NBagOfTricks.Test
{
    public class UTILS_EXTENSIONS : TestSuite
    {
        public override void RunSuite()
        {
            UT_INFO("Tests various util functions.");

            string input = "12345 \"I HAVE SPACES\" aaa bbb  \"me too\" ccc ddd \"  and the last  \"";
            // Output: 12345,I HAVE SPACES,aaa,bbb,me too,ccc,ddd,and the last

            var splits = input.SplitQuotedString();

            UT_EQUAL(splits.Count, 8);
            UT_EQUAL(splits[0], "12345");
            UT_EQUAL(splits[1], "I HAVE SPACES");
            UT_EQUAL(splits[2], "aaa");
            UT_EQUAL(splits[3], "bbb");
            UT_EQUAL(splits[4], "me too");
            UT_EQUAL(splits[5], "ccc");
            UT_EQUAL(splits[6], "ddd");
            UT_EQUAL(splits[7], "  and the last  ");

            input = " \"aaa ttt uuu\" 84ss \"  dangling quote  ";
            splits = input.SplitQuotedString();

            UT_EQUAL(splits.Count, 3);
            UT_EQUAL(splits[0], "aaa ttt uuu");
            UT_EQUAL(splits[1], "84ss");
            UT_EQUAL(splits[2], "  dangling quote  ");
        }
    }

    public class UTILS_SETTINGS : TestSuite
    {
        public class TestSettings : SettingsCore
        {
            public List<string> TestList { get; set; } = new();

            [JsonConverter(typeof(JsonColorConverter))]
            public Color? TestColor { get; set; } = Color.Salmon;

            public string TestString { get; set; } = "Just a test";
        }

        public override void RunSuite()
        {
            UT_INFO("Tests SettingsCore.");

            const string TEST_FN = "set-test.json";

            File.Delete(TEST_FN);

            TestSettings set = (TestSettings)TestSettings.Load(".", typeof(TestSettings), TEST_FN);

            UT_TRUE(!File.Exists(TEST_FN));

            UT_TRUE(set.TestColor.ToString()!.Contains("Salmon"));

            set.FormGeometry = new Rectangle(20, 50, 100, 200);
            set.TestString = "hoohaa";
            set.TestList.Add("item1");
            set.TestList.Add("item2");
            set.TestList.Add("item3");

            set.UpdateMru(Path.GetFullPath("Ephemera.NBagOfTricks.dll"));
            set.UpdateMru(Path.GetFullPath("Ephemera.NBagOfTricks.xml"));
            set.UpdateMru(@"C:\bad\path\file.xyz");

            UT_EQUAL(set.RecentFiles.Count, 2);

            set.Save();
            UT_TRUE(File.Exists(TEST_FN));

            // Check recent file list.
            File.ReadAllLines(TEST_FN).ForEach(l => Debug.WriteLine(l));
        }
    }

    public class UTILS_MISC : TestSuite
    {
        public override void RunSuite()
        {
            UT_INFO("Tests misc utils.");

            var dir = MiscUtils.GetAppDataDir("Foo");
            UT_EQUAL(dir, @"C:\Users\cepth\AppData\Local\Foo");

            dir = MiscUtils.GetAppDataDir("Bar", "CCCC");
            UT_EQUAL(dir, @"C:\Users\cepth\AppData\Local\CCCC\Bar");

            MiscUtils.ShowReadme("NBagOfTricks");
        }
    }

    public class UTILS_WATCHER : TestSuite
    {
        public override void RunSuite()
        {
            UT_INFO("Tests MultiFileWatcher.");

            int iters = 3;

            // The watcher is slow so we have to wait a bit.
            int delay = 200;

            List<string> filesTouched = new(); // capture
            MultiFileWatcher _watcher = new();
            _watcher.FileChangeEvent += (object? sender, MultiFileWatcher.FileChangeEventArgs e) => { filesTouched.AddRange(e.FileNames); };

            // Create fake files.
            List<string> testFilesToWatch = new();
            for (int i = 0; i < iters; i++)
            {
                string fn = $@"..\..\out\test_{i + 1}.txt";
                testFilesToWatch.Add(fn);
                _watcher.Add(fn);
            }

            System.Threading.Thread.Sleep(delay);

            UT_EQUAL(_watcher.WatchedFiles.Count, iters);
            UT_EQUAL(filesTouched.Count, 0);

            // Touch the files.
            testFilesToWatch.ForEach(fn => File.WriteAllText(fn, "AAAAAA"));
            System.Threading.Thread.Sleep(delay);

            UT_EQUAL(_watcher.WatchedFiles.Count, iters);
            UT_EQUAL(filesTouched.Count, iters);

            _watcher.Clear();
            System.Threading.Thread.Sleep(delay);

            UT_EQUAL(_watcher.WatchedFiles.Count, 0);

            // Post clear should still work.
            filesTouched.Clear();
            testFilesToWatch.ForEach(fn => { _watcher.Add(fn); File.WriteAllText(fn, "BBBBBB"); });
            System.Threading.Thread.Sleep(delay);

            UT_EQUAL(_watcher.WatchedFiles.Count, iters);
            UT_EQUAL(filesTouched.Count, iters);
        }
    }
}
