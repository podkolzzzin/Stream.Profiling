using MoreLinq;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Stream.Profiling.SQLite.Sorters;

namespace Stream.Profiling
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var sw = Stopwatch.StartNew();
            var fileName = new Generator().Generate(20_000_000);
            Console.WriteLine("Generated");

            // var s = Stopwatch.StartNew();
            // var loader = new SimpleStupidSQLiteLoader();
            // loader.InsertLines(fileName);
            // s.Stop();
            // Console.WriteLine($"Inserted in {s.Elapsed}");
            // var loaders = new SimpleStupidSQLiteLoader[]
            // {
            //     //new SimpleStupidSQLiteLoader(),
            //     
            //     new TransactionedSQLiteLoader(),
            //     new WithPragmaSQLiteLoader(),
            //     new PreparedQuerySQLiteLoader(),
            //     new MiltiInsertSQLiteLoader(),
            // };
            //
            // foreach (var loader in loaders)
            // {
            //     using (loader)
            //     {
            //         var s = Stopwatch.StartNew();
            //         loader.InsertLines(fileName);
            //         s.Stop();
            //         Console.WriteLine($"{loader.GetType().Name} Inserted in {s.Elapsed}");
            //     }
            // }

            SQLiteApproach(fileName);
            await CustomApproach(fileName);

            UltimateApproach(fileName);
            UltimateApproachTuned(fileName);
            
            UltimateApproach3(fileName);
            UltimateApproach3Tuned(fileName);

            // await new Sorter().Sort(fileName, 2);
            // sw.Stop();
            // Console.WriteLine($"Execution took: {sw.Elapsed}");

            // int[] chunkSizes = { 4096, 8192, 65536 };
            // foreach (var chunkSize in chunkSizes)
            // {
            //     sw = Stopwatch.StartNew();
            //     using var liteDbLoader = new LiteDb.LiteDbLoader(chunkSize);
            //     liteDbLoader.InsertLines(fileName);
            //     sw.Stop();
            //     Console.WriteLine($"LiteDb {chunkSize}: {sw.Elapsed}");
            // }

        }

        private static void UltimateApproach(string fileName)
        {
            var custom = Stopwatch.StartNew();
            var s = new Sorter2();
            s.Sort(fileName);
            Console.WriteLine($"Ultimate solution: {custom.Elapsed}");
        }
        
        private static void UltimateApproachTuned(string fileName)
        {
            var custom = Stopwatch.StartNew();
            var s = new Sorter2Tuned();
            s.Sort(fileName);
            Console.WriteLine($"Ultimate solution tuned: {custom.Elapsed}");
        }

        private static void UltimateApproach3(string fileName)
        {
            var custom = Stopwatch.StartNew();
            var s = new Sorter3();
            s.Sort(fileName);
            Console.WriteLine($"Ultimate solution 3: {custom.Elapsed}");
        }
        
        private static void UltimateApproach3Tuned(string fileName)
        {
            var custom = Stopwatch.StartNew();
            var s = new Sorter3();
            s.Sort(fileName);
            Console.WriteLine($"Ultimate solution 3 Tuned: {custom.Elapsed}");
        }

        
        private static async Task CustomApproach(string fileName)
        {
            var custom = Stopwatch.StartNew();
            await new Sorter().Sort(fileName, 1000_000);
            custom.Stop();
            Console.WriteLine($"Custom solution: {custom.Elapsed}");
        }

        private static void SQLiteApproach(string fileName)
        {
            var total = Stopwatch.StartNew();
            var insert = Stopwatch.StartNew();
            var loader = new MiltiInsertSQLiteLoader();
            using (loader)
            {
                loader.InsertLines(fileName);
                insert.Stop();
                Console.WriteLine($">>Insert took: {insert.Elapsed}");
            }

            var sort = Stopwatch.StartNew();
            ISorter sorter = new SimpleStupidSorter(loader.DbName);
            sorter.Sort();
            sort.Stop();
            Console.WriteLine($">>Sorting Elapsed took: {sort.Elapsed}");
            total.Stop();
            Console.WriteLine($"Total using SQLite: {total.Elapsed}");
        }
    }
}
